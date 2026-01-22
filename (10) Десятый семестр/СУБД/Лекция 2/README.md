
1. Поднять postgresql db (можно через контейнер)
```shell
docker run -d --name pg-demo \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -v pg_demo_data:/var/lib/postgresql/data \
  postgres:15

docker ps --filter name=pg-demo
docker logs -n 20 pg-demo
```

2. Перенести базу данных 
```shell
cd ~/Developer 2>/dev/null || cd ~
curl -L -o demo.sql.gz https://edu.postgrespro.ru/demo-20250901-3m.sql.gz
ls -lh demo.sql.gz
file demo.sql.gz
gunzip -c demo.sql.gz | docker exec -i pg-demo psql -U postgres
docker exec -it pg-demo psql -U postgres -c "\l"
docker exec -it pg-demo psql -U postgres -d demo -c "SELECT bookings.version();"
docker exec -it pg-demo psql -U postgres -d demo -c "\dt bookings.*"
```

3. Подключится (предпочтительнее dbeaver):
• Host: localhost
• Port: 5432
• Database: demo
• User: postgres
• Password: postgres
• SSL: disable (если попросит)

3. Выполнить зарпрос
```sql
with arr as (SELECT arrival_airport, count(*) FROM tickets JOIN segments ON tickets.ticket_no = segments.ticket_no JOIN flights ON segments.flight_id = flights.flight_id JOIN routes ON flights.route_no = routes.route_no GROUP BY arrival_airport), dep as (SELECT departure_airport, count(*) FROM tickets JOIN segments ON tickets.ticket_no = segments.ticket_no JOIN flights ON segments.flight_id = flights.flight_id JOIN routes ON flights.route_no = routes.route_no GROUP BY departure_airport), un as (select departure_airport, count from dep UNION select arrival_airport, count from arr)  select departure_airport as airport, sum(count) from un GROUP BY departure_airport ORDER BY sum DESC LIMIT 5;
```