WITH RECURSIVE edges AS (
SELECT DISTINCT
departure_airport::text AS dep,
arrival_airport::text AS arr
FROM bookings.routes
),

-- Все старты (вся база)
paths_all AS (
SELECT
dep AS start_airport,
arr AS current_airport,
ARRAY[dep, arr]::text[] AS route,
1 AS legs
FROM edges
WHERE dep <> arr

UNION ALL

SELECT
p.start_airport,
e.arr,
p.route || e.arr,
p.legs + 1
FROM paths_all p
JOIN edges e ON e.dep = p.current_airport
WHERE NOT (e.arr = ANY (p.route))
-- ограничение
AND p.legs < 5
-- ограничение
),

-- Старт только из CJC
paths_cjc AS (
SELECT
'CJC'::text AS start_airport,
e.arr AS current_airport,
ARRAY['CJC'::text, e.arr]::text[] AS route,
1 AS legs
FROM edges e
WHERE e.dep = 'CJC'
AND e.arr <> 'CJC'

UNION ALL

SELECT
p.start_airport,
e.arr,
p.route || e.arr,
p.legs + 1
FROM paths_cjc p
JOIN edges e ON e.dep = p.current_airport
WHERE NOT (e.arr = ANY (p.route))
-- ограничение
AND p.legs < 5
-- ограничение
),

best_all AS (
SELECT start_airport, legs, route
FROM paths_all
ORDER BY legs DESC, route::text
LIMIT 1
),

best_cjc AS (
SELECT start_airport, legs, route
FROM paths_cjc
ORDER BY legs DESC, route::text
LIMIT 1
)

SELECT
'1) longest_from_all' AS result_type,
start_airport,
legs,
route
FROM best_all

UNION ALL

SELECT
'2) longest_from_CJC' AS result_type,
start_airport,
legs,
route
FROM best_cjc;