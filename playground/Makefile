start:
	docker compose up lab -d;

stop:
	docker compose down -v;

logs:
	docker compose logs lab -f;

shell:
	make start;
	docker compose exec -it lab bash;