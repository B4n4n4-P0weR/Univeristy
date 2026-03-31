-- Построение json-дерева
CREATE OR REPLACE FUNCTION build_tree_json(p_parent INT)
RETURNS jsonb
LANGUAGE sql
AS $$
    SELECT COALESCE(
        jsonb_agg(
            jsonb_build_object(
                'id', t.id,
                'pos', t.pos,
                'title', t.title,
                'parent', t.parent,
                'children', build_tree_json(t.id)
            )
            ORDER BY t.pos
        ),
        '[]'::jsonb
    )
    FROM tree_nodes t
    WHERE t.parent IS NOT DISTINCT FROM p_parent
$$;

SELECT jsonb_pretty(build_tree_json(NULL));

-- Запрет зацикливания дерева
CREATE OR REPLACE FUNCTION prevent_tree_cycle()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    -- Если parent = NULL, это корень, здесь цикла быть не может
    IF NEW.parent IS NULL THEN
        RETURN NEW;
    END IF;

    -- Узел не может быть родителем самому себе
    IF NEW.parent = NEW.id THEN
        RAISE EXCEPTION 'Узел % не может стать своим родителем', NEW.id;
    END IF;

    -- Проверяем, не является ли новый parent потомком самого узла
    IF EXISTS (
        WITH RECURSIVE ancestors AS (
            SELECT id, parent
            FROM tree_nodes
            WHERE id = NEW.parent

            UNION

            SELECT t.id, t.parent
            FROM tree_nodes t
            JOIN ancestors a ON t.id = a.parent
        )
        SELECT 1
        FROM ancestors
        WHERE id = NEW.id
    ) THEN
        RAISE EXCEPTION 'Узел % нельзя поместить внутрь собственного поддерева', NEW.id;
    END IF;

    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS prevent_tree_cycle_trigger ON tree_nodes;

CREATE TRIGGER prevent_tree_cycle_trigger
BEFORE INSERT OR UPDATE OF parent
ON tree_nodes
FOR EACH ROW
EXECUTE FUNCTION prevent_tree_cycle();

SELECT jsonb_pretty(build_tree_json(NULL));