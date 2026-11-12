# C# Assignment

Two ASP.NET Core services. Each folder has its own Dockerfile, Compose file, Makefile, and README.

| Service | What it does | Docs |
| --- | --- | --- |
| `rest-api` | In-memory item list. List, fetch, and create items. | [rest-api/README.md](rest-api/README.md) |
| `websocket` | WebSocket echo server. Sends `connected`, then replies `echo: <message>`. | [websocket/README.md](websocket/README.md) |

From this folder:

```bash
make rest-api
make websocket
```

Stop and remove a container with `make rest-api-down` or `make websocket-down`. The same commands exist inside each service folder as `make up` and `make down`.
