# LingvoGameOs — Рудзынг.рф

Домен проекта: [рудзынг.рф](https://рудзынг.рф)

Резервные адреса: `http://89.223.66.173/`, `http://158.160.104.26:9000`.

## Docker: один вход для локалки и сервера

Базовый `docker-compose.yml` берёт проверенную на сервере схему из пятничного
архива и расширяет её до полного стека: PostgreSQL + Web + WebApi + семь игр.
В production игры собираются в отдельные образы. Локальный
`docker-compose.local.yml` подмонтирует их исходники из соседней папки `../games`.

### Локально, Windows

```powershell
.\stack.ps1 local up
```

Если `.env.local` отсутствует, скрипт использует безопасный локальный шаблон
`.env.local.example`. Сайт откроется на `http://localhost:5086`, API — на
`http://localhost:5147`, игры — на портах `3001`–`3007`.

Чтобы переопределить настройки, скопируй `.env.local.example` в `.env.local`.

### На сервере, Linux

Один раз создай и заполни секреты:

```bash
cp .env.server.example .env.server
```

После этого каждый деплой — одной командой:

```bash
bash ./stack.sh server up
```

Серверный `up` сначала подтягивает свежие базовые образы, собирает Web/WebApi,
проверяет конфигурацию, затем поднимает стек и ждёт healthy-состояния. Если на
старом сервере уже используется `.env`, скрипт подхватит его как fallback.
Для старого пятничного `.env` путь к играм по умолчанию равен `../games`,
`PORTAL_PUBLIC_URL` берётся из `SITE_PUBLIC_HOST`, а ключ Flask — из
`GAMES_API_SECRET`. При первой возможности лучше вынести их в отдельные
значения из `.env.server.example`.

### Управление стеком

Интерфейс у PowerShell- и Bash-скрипта одинаковый:

```text
stack <local|server> <up|down|restart|status|logs|config|build>
```

Примеры:

```powershell
.\stack.ps1 local logs
.\stack.ps1 local down
```

```bash
bash ./stack.sh server status
bash ./stack.sh server logs
bash ./stack.sh server restart
```

`down` не удаляет данные. PostgreSQL остаётся в `POSTGRES_DATA_PATH`
(`./postgres-db` по умолчанию), базы и пользовательский контент игр — в
именованных volumes, логи — в volume `logs`. Production-загрузки портала
хранятся в S3, поэтому статические файлы нового Web-образа не перекрываются
устаревшим volume и обновляются при каждом деплое.

## Важные настройки

- `SITE_PUBLIC_HOST` указывается с протоколом и без завершающего `/`, например
  `http://localhost` или `https://рудзынг.рф`. Порт игры добавляет приложение.
- `SITE_IP=127.0.0.1` оставляет Web/API доступными только reverse proxy на
  сервере. Для прямого доступа снаружи используй `0.0.0.0` и настрой firewall.
- `GAMES_API_SECRET` должен быть одинаковым у Web, WebApi и игр.
- Все семь игр запускаются Compose-ом в отдельных контейнерах; Web не
  запускает `run.sh`. Путь к исходникам задаёт `GAMES_SOURCE_PATH`.
- Identity seed добавляет недостающие web-игры по `GameFolderName` даже в уже
  существующую БД, не создавая повторов при следующих запусках.
- Порты игр централизованы: три старые игры `3001`–`3003`, `Who_faster` — `3004`,
  `iron-detective` — `3005`, `osetinckie-rebusi` — `3006`, `ossetian-mazaika` — `3007`.
- EF-миграции и Identity seed запускает Web. WebApi ждёт healthy-состояния Web,
  чтобы два процесса не применяли миграции одновременно на пустой базе.
- Файлы `.env`, `.env.local`, `.env.server` и старый файл `env` игнорируются Git.
  В репозиторий коммитятся только `*.example` без настоящих секретов.
