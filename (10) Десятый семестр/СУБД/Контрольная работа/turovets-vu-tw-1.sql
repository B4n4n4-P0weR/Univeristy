-- Задача: получить точки графика время и количество самолетов в небе
-- Шаг сетки: 5 минут
--
-- Ключевые правила:
-- 1) Учитываем только фактические вылеты (actual_departure), а не плановые.
-- 2) Для прилета (-1) учитываем только рейсы, у которых есть и фактический вылет, и фактический прилет.
-- 3) Значение считаем только в точках 5-минутной сетки (15:00, 15:05, ...),
--    без "поминутного" вывода внутри интервала.
-- 4) По условию можно пропускать точки, где значение не меняется:
--    в финальном SELECT оставлен такой фильтр.


-- Необязательные индексы для ускорения на больших данных

CREATE INDEX IF NOT EXISTS flights_actual_departure_idx
    ON bookings.flights (actual_departure)
    WHERE actual_departure IS NOT NULL;

CREATE INDEX IF NOT EXISTS flights_actual_arrival_idx
    ON bookings.flights (actual_arrival)
    WHERE actual_departure IS NOT NULL
      AND actual_arrival   IS NOT NULL;

-- Основной запрос
WITH
-- 1) Границы наблюдения по фактическим вылетам
bounds AS (
    SELECT
        min(actual_departure) AS min_t,
        max(COALESCE(actual_arrival, actual_departure)) AS max_t
    FROM bookings.flights
    WHERE actual_departure IS NOT NULL
),

-- 2) События: +1 в момент фактического вылета, -1 в момент фактического прилета
raw_events AS (
    SELECT actual_departure AS ts,  1 AS delta
    FROM bookings.flights
    WHERE actual_departure IS NOT NULL

    UNION ALL

    SELECT actual_arrival   AS ts, -1 AS delta
    FROM bookings.flights
    WHERE actual_departure IS NOT NULL
      AND actual_arrival   IS NOT NULL
),

-- 3) Схлопываем события с одинаковым временем
agg_events AS (
    SELECT ts, SUM(delta) AS delta
    FROM raw_events
    GROUP BY ts
    HAVING SUM(delta) <> 0
),

-- 4) Генерируем только точки 5-минутной сетки
--    anchor фиксирован, чтобы биннинг был стабильным
buckets AS (
    SELECT gs AS ts
    FROM bounds b,
         generate_series(
             date_bin(INTERVAL '5 minutes', b.min_t, TIMESTAMPTZ '2000-01-01 00:00:00+00'),
             date_bin(INTERVAL '5 minutes', b.max_t, TIMESTAMPTZ '2000-01-01 00:00:00+00'),
             INTERVAL '5 minutes'
         ) AS gs
),

-- 5) Объединяем реальную временную ось событий с точками выборки
--    is_bucket=true означает точка графика, false — реальное событие
merged_timeline AS (
    SELECT ts, delta, false AS is_bucket FROM agg_events
    UNION ALL
    SELECT ts, 0     AS delta, true  AS is_bucket FROM buckets
),

-- 6) Накопительная сумма по оси времени
--    Важно: при одинаковом ts сначала применяем события, потом берем bucket-точку
bucket_values AS (
    SELECT
        ts AS time_5min,
        SUM(delta) OVER (
            ORDER BY ts, is_bucket
            ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
        ) AS planes_in_sky,
        is_bucket
    FROM merged_timeline
),

-- 7) Берем только точки сетки и подготавливаем LAG для сжатия
prepared AS (
    SELECT
        time_5min,
        GREATEST(planes_in_sky, 0) AS planes_in_sky,
        LAG(GREATEST(planes_in_sky, 0)) OVER (ORDER BY time_5min) AS prev_planes
    FROM bucket_values
    WHERE is_bucket
)

-- 8) Финал: точки графика
SELECT
    time_5min AS time,
    planes_in_sky
FROM prepared
-- Если нужно оставить ВСЕ точки по 5 минут, закомментируй следующую строку
WHERE prev_planes IS DISTINCT FROM planes_in_sky OR prev_planes IS NULL
ORDER BY time;
