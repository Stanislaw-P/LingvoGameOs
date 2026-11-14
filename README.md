# LingvoGameOs
тз: https://docs.google.com/document/d/1CkvnNmPx_YqYykxrxbUaPaADyfllAIxK/edit#heading=h.44sinio  
Текст курсовой: https://docs.google.com/document/d/1WD1nIOc34gFmHefq9W7wLmbEb1EckCqG/edit  

Домен проекта: Рудзынг.рф

Ссылка на проект timeWeb: http://89.223.66.173/

Альтернативная ссылка на проект: http://158.160.104.26:9000

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

Сабмодули нужны на машине сборки. Локальные `up` и `build` подготавливают их
автоматически. Сервер запускает готовые образы и не требует Git, исходников,
SDK, pip, NuGet или доступа к Интернету.

### Локально, Windows

Один раз создай локальный `.env` из шаблона:

```powershell
Copy-Item .env.local.example .env
```

После этого запускай стек:

```powershell
.\stack.ps1 local up
```

Локальный запуск всегда использует `.env`. Сайт откроется на
`http://localhost:5086`, API — на `http://localhost:5147`, а игры — на адресах
вида `http://localhost:5086/games/<название>`. Отдельные игровые порты на
хосте не публикуются.

### На сервере, Linux

Один раз создай и заполни секреты:

```bash
cp .env.server.example .env
```

Сначала на машине с Интернетом подготовь релиз (нужны Python 3.11+ и Docker):

```powershell
.\stack.ps1 server export .\artifacts\offline\release-2026-10-25
```

В каталоге появятся `images.tar.gz` со всеми образами, runtime-only Compose,
скрипты запуска, пример `.env`, список точных image ID и контрольные суммы.
Рабочий `.env` и секреты в пакет не копируются. Перенеси весь каталог на сервер,
там создай свой `.env` из `.env.server.example`. Дальше запуск одной командой:

```bash
bash ./stack.sh server up
```

Серверный `up` проверяет наличие точных образов из `IMAGES`, при необходимости
проверяет SHA256 пакета и выполняет `docker load`, затем запускает Compose с
`--no-build --pull never` и ждёт healthy-состояния. Ни сборки, ни скачивания
образов, ни обращения к Git при этом нет. Повторный запуск не импортирует
неизменившиеся образы. Секреты читаются только из рабочего `.env` сервера.

Docker Engine и Compose должны быть установлены на сервере заранее. По умолчанию
экспорт рассчитан на Linux x86-64; для ARM используй `deploy/bundle.py --platform
linux/arm64 --output <каталог>`. Пакет с другой архитектурой запускать нельзя.
Подробности и ограничения офлайна: [docs/deployment.md](docs/deployment.md).

### Управление стеком

Интерфейс у PowerShell- и Bash-скрипта одинаковый:

```text
stack <local|server> <up|down|restart|status|logs|config|build|export|load> [каталог-пакета]
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
при `FILE_STORAGE=local` хранятся в volume `portal-uploads`, отдельно от
встроенной статики. Существующие S3-установки сохраняют прежнее поведение,
если `FILE_STORAGE` не переопределён. Переход на полностью офлайновый сервер
требует отдельно перенести уже загруженные в S3 файлы.

## Играть без аккаунта

Гость запускает игру через `/start?guest=1`. `GameSession` в БД не создаётся,
запросы профиля и начисления баллов не выполняются. Игровая партия работает
обычно; общий рейтинг и история доступны после входа. Гостевой запуск очищает
прежний аккаунт внутри игры. Для вошедшего пользователя остаётся подписанный
контракт `/start?session=<id>&token=<hmac>` с проверкой HMAC и одноразовым итогом.

## Важные настройки

- `SITE_PUBLIC_HOST` указывается с протоколом и без завершающего `/`, например
  `http://localhost` или `https://рудзынг.рф`. Все игры доступны с этого же
  origin через `/games/...`.
- `SITE_IP=127.0.0.1` оставляет Web/API доступными только reverse proxy на
  сервере. Для прямого доступа снаружи используй `0.0.0.0` и настрой firewall.
- `GAMES_API_SECRET` должен быть одинаковым у Web и WebApi. Игры получают
  подпись конкретной сессии, а не этот секрет.
- Все семь игр запускаются Compose-ом в отдельных контейнерах; Web не
  запускает `run.sh`. По умолчанию исходники берутся из сабмодулей `./games`;
  путь можно переопределить через `GAMES_SOURCE_PATH`.
- Identity seed добавляет недостающие web-игры по `GameFolderName` даже в уже
  существующую БД, не создавая повторов при следующих запусках.
- Web проксирует игры по понятным адресам: `/games/piece-by-piece`,
  `/games/crossword`, `/games/puzzles`, `/games/who-faster`,
  `/games/iron-detective`, `/games/rebusi` и `/games/mazaika`. Игровые сервисы
  слушают порты только внутри Docker-сети и снаружи напрямую недоступны.
- EF-миграции и Identity seed запускает Web. WebApi ждёт healthy-состояния Web,
  чтобы два процесса не применяли миграции одновременно на пустой базе.
- Файл `.env` и старый файл `env` игнорируются Git.
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
