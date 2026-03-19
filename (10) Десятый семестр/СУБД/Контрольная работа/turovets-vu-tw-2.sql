-- Полное решение для задания 2:
-- 1) Пересоздание базы данных employee_hierarchy
-- 2) Построение схемы с ограничениями и триггерами
-- 3) Генерация ~100 сотрудников с глубиной иерархии = 4
-- 4) Функция проверки: руководит ли X сотрудником Y в момент времени T
-- 5) Функция вывода дерева подчиненности на момент T (отступы + число подчиненных)


\connect postgres

DROP DATABASE IF EXISTS employee_hierarchy WITH (FORCE);
CREATE DATABASE employee_hierarchy;

\connect employee_hierarchy

CREATE EXTENSION IF NOT EXISTS btree_gist;

-- 1. Основные таблицы

CREATE TABLE employee (
    employee_id   BIGSERIAL PRIMARY KEY,
    employee_no   TEXT NOT NULL UNIQUE,
    last_name     TEXT NOT NULL,
    first_name    TEXT NOT NULL,
    middle_name   TEXT,
    hired_at      TIMESTAMPTZ NOT NULL,
    fired_at      TIMESTAMPTZ,
    CHECK (fired_at IS NULL OR fired_at > hired_at)
);

CREATE TABLE reporting_line (
    reporting_id  BIGSERIAL PRIMARY KEY,
    employee_id   BIGINT NOT NULL REFERENCES employee(employee_id),
    manager_id    BIGINT NOT NULL REFERENCES employee(employee_id),
    valid_from    TIMESTAMPTZ NOT NULL,
    valid_to      TIMESTAMPTZ NOT NULL,
    period        TSTZRANGE GENERATED ALWAYS AS (tstzrange(valid_from, valid_to, '[)')) STORED,
    CHECK (employee_id <> manager_id),
    CHECK (valid_to > valid_from)
);

CREATE TABLE vacation (
    vacation_id   BIGSERIAL PRIMARY KEY,
    employee_id   BIGINT NOT NULL REFERENCES employee(employee_id),
    vac_from      TIMESTAMPTZ NOT NULL,
    vac_to        TIMESTAMPTZ NOT NULL,
    period        TSTZRANGE GENERATED ALWAYS AS (tstzrange(vac_from, vac_to, '[)')) STORED,
    CHECK (vac_to > vac_from)
);

CREATE TABLE delegation (
    delegation_id         BIGSERIAL PRIMARY KEY,
    replaced_employee_id  BIGINT NOT NULL REFERENCES employee(employee_id),
    delegate_employee_id  BIGINT NOT NULL REFERENCES employee(employee_id),
    valid_from            TIMESTAMPTZ NOT NULL,
    valid_to              TIMESTAMPTZ NOT NULL,
    period                TSTZRANGE GENERATED ALWAYS AS (tstzrange(valid_from, valid_to, '[)')) STORED,
    CHECK (replaced_employee_id <> delegate_employee_id),
    CHECK (valid_to > valid_from)
);

-- 2. Ограничения временной целостности (с учетом условий задачи)

-- Для одной пары сотрудник->руководитель интервалы не должны пересекаться
ALTER TABLE reporting_line
ADD CONSTRAINT reporting_no_overlap_same_pair
EXCLUDE USING gist (
    employee_id WITH =,
    manager_id  WITH =,
    period      WITH &&
);

-- Отпуска одного сотрудника не должны пересекаться
ALTER TABLE vacation
ADD CONSTRAINT vacation_no_overlap
EXCLUDE USING gist (
    employee_id WITH =,
    period      WITH &&
);

-- Один замещаемый сотрудник не может иметь двух заместителей одновременно
ALTER TABLE delegation
ADD CONSTRAINT delegation_no_overlap_for_replaced
EXCLUDE USING gist (
    replaced_employee_id WITH =,
    period               WITH &&
);

-- Один заместитель может замещать нескольких сотрудников одновременно

-- Индексы для ускорения запросов
CREATE INDEX reporting_employee_idx   ON reporting_line(employee_id);
CREATE INDEX reporting_manager_idx    ON reporting_line(manager_id);
CREATE INDEX reporting_period_gist    ON reporting_line USING gist(period);

CREATE INDEX vacation_employee_idx    ON vacation(employee_id);
CREATE INDEX vacation_period_gist     ON vacation USING gist(period);

CREATE INDEX delegation_replaced_idx  ON delegation(replaced_employee_id);
CREATE INDEX delegation_delegate_idx  ON delegation(delegate_employee_id);
CREATE INDEX delegation_period_gist   ON delegation USING gist(period);

-- 3. Функции валидации и триггеры

-- Вспомогательная функция: период должен попадать в интервал занятости сотрудника
CREATE OR REPLACE FUNCTION assert_employee_active_period(
    p_employee_id BIGINT,
    p_from TIMESTAMPTZ,
    p_to   TIMESTAMPTZ
) RETURNS VOID
LANGUAGE plpgsql AS
$$
DECLARE
    v_hired TIMESTAMPTZ;
    v_fired TIMESTAMPTZ;
BEGIN
    SELECT hired_at, fired_at
      INTO v_hired, v_fired
      FROM employee
     WHERE employee_id = p_employee_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Employee % does not exist', p_employee_id;
    END IF;

    IF p_from < v_hired THEN
        RAISE EXCEPTION 'Period starts before hire date for employee %', p_employee_id;
    END IF;

    IF v_fired IS NOT NULL AND p_to > v_fired THEN
        RAISE EXCEPTION 'Period ends after fired date for employee %', p_employee_id;
    END IF;
END;
$$;

-- reporting_line: проверка активного интервала + защита от циклов
CREATE OR REPLACE FUNCTION trg_reporting_check()
RETURNS TRIGGER
LANGUAGE plpgsql AS
$$
BEGIN
    PERFORM assert_employee_active_period(NEW.employee_id, NEW.valid_from, NEW.valid_to);
    PERFORM assert_employee_active_period(NEW.manager_id,  NEW.valid_from, NEW.valid_to);

    -- Detect cycle at NEW.valid_from snapshot
    IF EXISTS (
        WITH RECURSIVE up_chain AS (
            SELECT NEW.manager_id AS emp_id
            UNION ALL
            SELECT r.manager_id
            FROM reporting_line r
            JOIN up_chain c ON r.employee_id = c.emp_id
            WHERE NEW.valid_from >= r.valid_from
              AND NEW.valid_from <  r.valid_to
        )
        SELECT 1 FROM up_chain WHERE emp_id = NEW.employee_id
    ) THEN
        RAISE EXCEPTION 'Hierarchy cycle detected: % -> % creates a loop', NEW.manager_id, NEW.employee_id;
    END IF;

    RETURN NEW;
END;
$$;

CREATE TRIGGER reporting_check_biud
BEFORE INSERT OR UPDATE ON reporting_line
FOR EACH ROW
EXECUTE FUNCTION trg_reporting_check();

-- vacation: проверка активного интервала + запрет отпуска у действующего заместителя
CREATE OR REPLACE FUNCTION trg_vacation_check()
RETURNS TRIGGER
LANGUAGE plpgsql AS
$$
BEGIN
    PERFORM assert_employee_active_period(NEW.employee_id, NEW.vac_from, NEW.vac_to);

    IF EXISTS (
        SELECT 1
        FROM delegation d
        WHERE d.delegate_employee_id = NEW.employee_id
          AND d.period && NEW.period
    ) THEN
        RAISE EXCEPTION 'Employee % cannot vacation while acting as delegate', NEW.employee_id;
    END IF;

    RETURN NEW;
END;
$$;

CREATE TRIGGER vacation_check_biud
BEFORE INSERT OR UPDATE ON vacation
FOR EACH ROW
EXECUTE FUNCTION trg_vacation_check();

-- delegation: проверка активного интервала + замещаемый должен быть в отпуске
-- + заместитель не должен быть в отпуске.
CREATE OR REPLACE FUNCTION trg_delegation_check()
RETURNS TRIGGER
LANGUAGE plpgsql AS
$$
BEGIN
    PERFORM assert_employee_active_period(NEW.replaced_employee_id, NEW.valid_from, NEW.valid_to);
    PERFORM assert_employee_active_period(NEW.delegate_employee_id, NEW.valid_from, NEW.valid_to);

    IF NOT EXISTS (
        SELECT 1
        FROM vacation v
        WHERE v.employee_id = NEW.replaced_employee_id
          AND v.vac_from <= NEW.valid_from
          AND v.vac_to   >= NEW.valid_to
    ) THEN
        RAISE EXCEPTION 'Delegation period must be fully inside replaced employee vacation';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM vacation v
        WHERE v.employee_id = NEW.delegate_employee_id
          AND v.period && NEW.period
    ) THEN
        RAISE EXCEPTION 'Delegate cannot be on vacation during delegation';
    END IF;

    RETURN NEW;
END;
$$;

CREATE TRIGGER delegation_check_biud
BEFORE INSERT OR UPDATE ON delegation
FOR EACH ROW
EXECUTE FUNCTION trg_delegation_check();

-- 4. Тестовые данные (~100 сотрудников, глубина иерархии = 4)

CREATE OR REPLACE FUNCTION gen_random_string(arg_syllable_count int DEFAULT NULL)
RETURNS text
LANGUAGE SQL AS $$
WITH consonants AS (
    SELECT ARRAY['б','в','г','д','ж','з','й','к','л','м','н','п','р','с','т','ф','х','ц','ч','ш','щ'] AS c
),
vowels AS (
    SELECT ARRAY['а','е','ё','и','о','у','ы','э','ю','я'] AS v
),
params AS (
    SELECT COALESCE(arg_syllable_count, (2 + floor(random() * 3))::int) AS n
)
SELECT string_agg(
           c[1 + floor(random() * array_length(c, 1))::int]
        || v[1 + floor(random() * array_length(v, 1))::int],
        ''
       )
FROM generate_series(1, (SELECT n FROM params))
CROSS JOIN consonants
CROSS JOIN vowels;
$$;

CREATE OR REPLACE FUNCTION gen_random_fullname()
RETURNS text
LANGUAGE SQL AS $$
SELECT INITCAP(gen_random_string(NULL::int) || ' ' || gen_random_string(NULL::int) || ' ' || gen_random_string(NULL::int));
$$;

TRUNCATE TABLE delegation, vacation, reporting_line, employee RESTART IDENTITY CASCADE;

INSERT INTO employee (employee_no, last_name, first_name, middle_name, hired_at, fired_at)
SELECT
    format('EMP%04s', gs) AS employee_no,
    split_part(fn, ' ', 1) AS last_name,
    split_part(fn, ' ', 2) AS first_name,
    split_part(fn, ' ', 3) AS middle_name,
    TIMESTAMPTZ '2023-01-01 09:00:00+00' AS hired_at,
    NULL::timestamptz AS fired_at
FROM (
    SELECT gs, gen_random_fullname() AS fn
    FROM generate_series(1, 100) gs
) t;

DROP TABLE IF EXISTS tmp_emp_levels;
CREATE TEMP TABLE tmp_emp_levels AS
SELECT
    employee_id,
    row_number() OVER (ORDER BY employee_id) AS rn,
    CASE
        WHEN row_number() OVER (ORDER BY employee_id) = 1  THEN 1
        WHEN row_number() OVER (ORDER BY employee_id) <= 10 THEN 2
        WHEN row_number() OVER (ORDER BY employee_id) <= 40 THEN 3
        ELSE 4
    END AS lvl,
    CASE
        WHEN row_number() OVER (ORDER BY employee_id) = 1  THEN 1
        WHEN row_number() OVER (ORDER BY employee_id) <= 10 THEN row_number() OVER (ORDER BY employee_id) - 1
        WHEN row_number() OVER (ORDER BY employee_id) <= 40 THEN row_number() OVER (ORDER BY employee_id) - 10
        ELSE row_number() OVER (ORDER BY employee_id) - 40
    END AS pos
FROM employee;

-- Базовая иерархия действует в интервале [2024-01-01, 2026-01-01)
-- уровень 2 -> директор
INSERT INTO reporting_line (employee_id, manager_id, valid_from, valid_to)
SELECT l2.employee_id, root.employee_id,
       TIMESTAMPTZ '2024-01-01 00:00:00+00',
       TIMESTAMPTZ '2026-01-01 00:00:00+00'
FROM tmp_emp_levels l2
CROSS JOIN (SELECT employee_id FROM tmp_emp_levels WHERE lvl = 1) root
WHERE l2.lvl = 2;

-- уровень 3 -> уровень 2 (round-robin)
INSERT INTO reporting_line (employee_id, manager_id, valid_from, valid_to)
SELECT l3.employee_id, l2.employee_id,
       TIMESTAMPTZ '2024-01-01 00:00:00+00',
       TIMESTAMPTZ '2026-01-01 00:00:00+00'
FROM tmp_emp_levels l3
JOIN tmp_emp_levels l2
  ON l2.lvl = 2
 AND l2.pos = ((l3.pos - 1) % 9) + 1
WHERE l3.lvl = 3;

-- уровень 4 -> уровень 3 (round-robin)
INSERT INTO reporting_line (employee_id, manager_id, valid_from, valid_to)
SELECT l4.employee_id, l3.employee_id,
       TIMESTAMPTZ '2024-01-01 00:00:00+00',
       TIMESTAMPTZ '2026-01-01 00:00:00+00'
FROM tmp_emp_levels l4
JOIN tmp_emp_levels l3
  ON l3.lvl = 3
 AND l3.pos = ((l4.pos - 1) % 30) + 1
WHERE l4.lvl = 4;

-- Пример исторического изменения подчиненности в 2025-H1 для 10 сотрудников уровня 4
INSERT INTO reporting_line (employee_id, manager_id, valid_from, valid_to)
SELECT l4.employee_id,
       l3b.employee_id,
       TIMESTAMPTZ '2025-01-01 00:00:00+00',
       TIMESTAMPTZ '2025-07-01 00:00:00+00'
FROM tmp_emp_levels l4
JOIN tmp_emp_levels l3a
  ON l3a.lvl = 3
 AND l3a.pos = ((l4.pos - 1) % 30) + 1
JOIN tmp_emp_levels l3b
  ON l3b.lvl = 3
 AND l3b.pos = CASE WHEN l3a.pos = 30 THEN 1 ELSE l3a.pos + 1 END
WHERE l4.lvl = 4
  AND l4.pos BETWEEN 1 AND 10;

-- Отпуска для 8 руководителей
INSERT INTO vacation (employee_id, vac_from, vac_to)
SELECT employee_id,
       TIMESTAMPTZ '2025-03-01 00:00:00+00' + (pos * INTERVAL '10 days') AS vac_from,
       TIMESTAMPTZ '2025-03-01 00:00:00+00' + (pos * INTERVAL '10 days') + INTERVAL '7 days' AS vac_to
FROM tmp_emp_levels
WHERE lvl = 2
  AND pos BETWEEN 1 AND 8;

-- Замещения на периоды этих отпусков (позиции 1->2, ..., 8->1)
INSERT INTO delegation (replaced_employee_id, delegate_employee_id, valid_from, valid_to)
SELECT r.employee_id,
       d.employee_id,
       v.vac_from,
       v.vac_to
FROM tmp_emp_levels r
JOIN vacation v ON v.employee_id = r.employee_id
JOIN tmp_emp_levels d
  ON d.lvl = 2
 AND d.pos = CASE WHEN r.pos = 8 THEN 1 ELSE r.pos + 1 END
WHERE r.lvl = 2
  AND r.pos BETWEEN 1 AND 8;

-- После первичного заполнения включаем проверку единственного директора
-- Проверка "ровно один директор" на контрольных срезах времени,
-- возникающих при изменении таблицы подчиненности
CREATE OR REPLACE FUNCTION trg_assert_single_director()
RETURNS TRIGGER
LANGUAGE plpgsql AS
$$
DECLARE
    v_ts TIMESTAMPTZ;
    v_roots INT;
BEGIN
    -- Для INSERT/UPDATE проверяем начало нового интервала.
    IF TG_OP IN ('INSERT','UPDATE') THEN
        v_ts := NEW.valid_from;
    ELSE
        v_ts := OLD.valid_from;
    END IF;

    WITH active_employees AS (
        SELECT e.employee_id
        FROM employee e
        WHERE v_ts >= e.hired_at
          AND (e.fired_at IS NULL OR v_ts < e.fired_at)
    ), active_edges AS (
        SELECT DISTINCT r.manager_id, r.employee_id
        FROM reporting_line r
        WHERE v_ts >= r.valid_from
          AND v_ts <  r.valid_to
    )
    SELECT count(*) INTO v_roots
    FROM active_employees ae
    WHERE NOT EXISTS (
        SELECT 1 FROM active_edges x WHERE x.employee_id = ae.employee_id
    );

    IF v_roots <> 1 THEN
        RAISE EXCEPTION 'Нарушено правило единственного директора: на % найдено % корней', v_ts, v_roots;
    END IF;

    RETURN COALESCE(NEW, OLD);
END;
$$;

CREATE CONSTRAINT TRIGGER reporting_single_director_aiud
AFTER INSERT OR UPDATE OR DELETE ON reporting_line
DEFERRABLE INITIALLY DEFERRED
FOR EACH ROW
EXECUTE FUNCTION trg_assert_single_director();

-- 5. Функция запроса: руководит ли X сотрудником Y в момент T?

CREATE OR REPLACE FUNCTION manages_employee_at(
    p_manager_id   BIGINT,
    p_employee_id  BIGINT,
    p_ts           TIMESTAMPTZ DEFAULT now()
)
RETURNS BOOLEAN
LANGUAGE sql
STABLE
AS $$
WITH RECURSIVE
active_delegation AS (
    SELECT d.replaced_employee_id, d.delegate_employee_id
    FROM delegation d
    WHERE p_ts >= d.valid_from
      AND p_ts <  d.valid_to
),
effective_edges AS (
    SELECT DISTINCT
        COALESCE(dm.delegate_employee_id, r.manager_id)  AS manager_id,
        COALESCE(de.delegate_employee_id, r.employee_id) AS employee_id
    FROM reporting_line r
    LEFT JOIN active_delegation dm ON dm.replaced_employee_id = r.manager_id
    LEFT JOIN active_delegation de ON de.replaced_employee_id = r.employee_id
    WHERE p_ts >= r.valid_from
      AND p_ts <  r.valid_to
),
walk AS (
    SELECT manager_id, employee_id
    FROM effective_edges
    WHERE manager_id = p_manager_id

    UNION

    SELECT w.manager_id, e.employee_id
    FROM walk w
    JOIN effective_edges e ON e.manager_id = w.employee_id
)
SELECT EXISTS (
    SELECT 1 FROM walk WHERE employee_id = p_employee_id
);
$$;

-- 6. Функция запроса: дерево подчиненности на момент T

CREATE OR REPLACE FUNCTION hierarchy_tree_at(
    p_ts TIMESTAMPTZ
)
RETURNS TABLE (
    employee_tree_name TEXT,
    total_subordinates BIGINT
)
LANGUAGE sql
STABLE
AS $$
WITH RECURSIVE
active_delegation AS (
    SELECT d.replaced_employee_id, d.delegate_employee_id
    FROM delegation d
    WHERE p_ts >= d.valid_from
      AND p_ts <  d.valid_to
),
effective_edges AS (
    SELECT DISTINCT
        COALESCE(dm.delegate_employee_id, r.manager_id)  AS manager_id,
        COALESCE(de.delegate_employee_id, r.employee_id) AS employee_id
    FROM reporting_line r
    LEFT JOIN active_delegation dm ON dm.replaced_employee_id = r.manager_id
    LEFT JOIN active_delegation de ON de.replaced_employee_id = r.employee_id
    WHERE p_ts >= r.valid_from
      AND p_ts <  r.valid_to
),
roots AS (
    SELECT e.employee_id
    FROM employee e
    WHERE NOT EXISTS (
        SELECT 1 FROM effective_edges x WHERE x.employee_id = e.employee_id
    )
),
tree AS (
    SELECT
        r.employee_id,
        0 AS lvl,
        lpad(r.employee_id::text, 10, '0') AS path
    FROM roots r

    UNION ALL

    SELECT
        c.employee_id,
        t.lvl + 1 AS lvl,
        t.path || '.' || lpad(c.employee_id::text, 10, '0') AS path
    FROM tree t
    JOIN effective_edges c ON c.manager_id = t.employee_id
),
closure AS (
    SELECT manager_id AS ancestor_id, employee_id AS descendant_id
    FROM effective_edges
    UNION ALL
    SELECT c.ancestor_id, e.employee_id
    FROM closure c
    JOIN effective_edges e ON e.manager_id = c.descendant_id
),
sub_counts AS (
    SELECT ancestor_id AS employee_id, COUNT(DISTINCT descendant_id) AS total_subordinates
    FROM closure
    GROUP BY ancestor_id
)
SELECT
    repeat('    ', t.lvl)
    || emp.last_name || ' ' || left(emp.first_name, 1) || '. ' || left(coalesce(emp.middle_name, ''), 1) || '.'
        AS employee_tree_name,
    COALESCE(sc.total_subordinates, 0)::bigint AS total_subordinates
FROM tree t
JOIN employee emp ON emp.employee_id = t.employee_id
LEFT JOIN sub_counts sc ON sc.employee_id = t.employee_id
ORDER BY t.path;
$$;

-- 7. Контрольные проверки / примеры

-- Проверка объема данных
SELECT 'employees' AS metric, count(*)::text AS value FROM employee
UNION ALL
SELECT 'reporting_line rows', count(*)::text FROM reporting_line
UNION ALL
SELECT 'vacation rows', count(*)::text FROM vacation
UNION ALL
SELECT 'delegation rows', count(*)::text FROM delegation;

-- Для данного среза данных должен быть один корень (директор)
WITH effective_edges AS (
    SELECT manager_id, employee_id
    FROM reporting_line
    WHERE TIMESTAMPTZ '2025-06-01 00:00:00+00' >= valid_from
      AND TIMESTAMPTZ '2025-06-01 00:00:00+00' <  valid_to
)
SELECT count(*) AS director_candidates
FROM employee e
WHERE NOT EXISTS (
    SELECT 1 FROM effective_edges x WHERE x.employee_id = e.employee_id
);

-- SELECT manages_employee_at(1, 50, TIMESTAMPTZ '2025-06-01 12:00:00+00');
-- SELECT manages_employee_at(1, 50); -- текущее время
-- SELECT * FROM hierarchy_tree_at(TIMESTAMPTZ '2025-06-01 12:00:00+00');
