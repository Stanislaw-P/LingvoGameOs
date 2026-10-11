# LingvoGameOs — Рудзынг.рф

Домен проекта: [рудзынг.рф](https://рудзынг.рф)

Резервные адреса: `http://89.223.66.173/`, `http://158.160.104.26:9000`.

## Docker: один вход для локалки и сервера

Базовый `docker-compose.yml` поднимает полный стек: PostgreSQL + Web + WebApi +
семь игр. Игры закреплены в `games/` как Git-сабмодули на конкретных коммитах.
В production они собираются в отдельные образы, а локальный
`docker-compose.local.yml` подмонтирует исходники сабмодулей для быстрой разработки.

### Получение репозитория

Новый checkout сразу со всеми закреплёнными версиями игр:

```bash
git clone --recurse-submodules https://github.com/Stanislaw-P/LingvoGameOs.git
cd LingvoGameOs
```

Для уже существующего checkout достаточно:

```bash
git pull
git submodule update --init --recursive
```

Команды `stack ... up`, `stack ... build` и `stack ... config` сами выполняют
`submodule sync` и `submodule update`, поэтому обычный запуск остаётся одной
командой. На сервере Git должен иметь read-доступ ко всем игровым репозиториям.

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

Серверный `up` синхронизирует сабмодули с зафиксированными SHA, отказывается
деплоить игры с незакоммиченными серверными правками, подтягивает свежие базовые
образы, собирает Web/WebApi, проверяет конфигурацию, затем поднимает стек и ждёт
healthy-состояния. Если на старом сервере уже используется `.env`, скрипт
подхватит его как fallback. Удали из старого `.env` переменную
`GAMES_SOURCE_PATH` либо задай `GAMES_SOURCE_PATH=./games`.

Обычный `git archive` не включает содержимое сабмодулей. Для сервера используй
Git-checkout с доступом к игровым репозиториям, а не прежний плоский `tar.gz`.

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
  запускает `run.sh`. По умолчанию исходники берутся из сабмодулей `./games`;
  путь можно переопределить через `GAMES_SOURCE_PATH`.
- Identity seed добавляет недостающие web-игры по `GameFolderName` даже в уже
  существующую БД, не создавая повторов при следующих запусках.
- Порты игр централизованы: три старые игры `3001`–`3003`, `Who_faster` — `3004`,
  `iron-detective` — `3005`, `osetinckie-rebusi` — `3006`, `ossetian-mazaika` — `3007`.
- EF-миграции и Identity seed запускает Web. WebApi ждёт healthy-состояния Web,
  чтобы два процесса не применяли миграции одновременно на пустой базе.
- Файлы `.env`, `.env.local`, `.env.server` и старый файл `env` игнорируются Git.
  В репозиторий коммитятся только `*.example` без настоящих секретов.

## Обновление отдельной игры

Сабмодуль фиксирует точный коммит, поэтому сервер не получает случайный новый
`main` или `master`. Обновление выполняется явно:

```bash
cd games/<game>
git switch <main-or-master>
git pull --ff-only
# изменить игру, затем commit и push в её собственный репозиторий
cd ../..
git add games/<game>
git commit -m "обновил версию <game>"
```

Не используй `git submodule update --remote` в production: эта команда обходит
зафиксированные версии и делает результат деплоя зависимым от текущего HEAD игры.
