# WeakAppSolution

Event-driven platform that polls a legacy meter API, processes sensor events through RabbitMQ, stores them in PostgreSQL, and shows them on a React dashboard (GraphQL + live SignalR updates).

## Architecture

```text
WeakApp ──► DataIngestor ──► RabbitMQ ──► DataProcessor ──► PostgreSQL
                                  │                              │
                                  └──► NotificationService       │
                                            │                    │
                                            ▼                    ▼
                                     SignalR hub          GraphqlGateway
                                            │                    │
                                            └────────┬───────────┘
                                                     ▼
                                          sensor-dashboard-client
```

| Component | Role |
|-----------|------|
| **WeakApp** | Legacy HTTP API (`GET /meters`) |
| **DataIngestorService** | Polls WeakApp, publishes events to RabbitMQ |
| **DataProcessorService** | Consumes events, persists to PostgreSQL |
| **NotificationService** | Broadcasts events to clients via SignalR |
| **GraphqlGateway** | GraphQL API over PostgreSQL |
| **sensor-dashboard-client** | React dashboard (tables, charts, live updates) |

## Tech stack

.NET 10 · MassTransit · RabbitMQ · PostgreSQL · HotChocolate · SignalR · React / Vite / TypeScript · Docker Compose · Serilog · Prometheus / Grafana

## Prerequisites

- Docker & Docker Compose
- .NET 10 SDK and Node.js 20+ (for local development)
- GitHub Packages token with `read:packages` (for private NuGet restore)

```bash
cp .env.example .env
# set NUGET_TOKEN in .env
```

## Quick start

```bash
docker compose up --build -d
```

| What | URL |
|------|-----|
| Dashboard | http://localhost:5173 |
| GraphQL | http://localhost:6062/graphql |
| SignalR hub | http://localhost:2000/hubs/sensors |
| RabbitMQ UI | http://localhost:15672 (`guest` / `guest`) |
| Adminer | http://localhost:5050 |
| Grafana | http://localhost:3000 (`admin` / `admin`) |
| WeakApp | http://localhost:8080 (`X-Api-Key: supersecret`) |

```bash
docker compose down
```

## Local development

```bash
# Backend (example)
dotnet restore
dotnet build
dotnet test
dotnet run --project DataIngestorService/DataIngestorService

# Frontend
cd sensor-dashboard-client
npm install
npm run dev    # proxies /graphql and /hubs
npm test
```

More details for the SPA: [`sensor-dashboard-client/README.md`](sensor-dashboard-client/README.md).

## Repository layout

```text
WeakApp/                     Legacy meter API
DataIngestorService/         Poller + shared contracts + messaging
DataProcessorService/        Consumer + EF Core DAL
NotificationService/         SignalR broadcaster
GraphqlGateway/              GraphQL API
sensor-dashboard-client/     React UI
.github/workflows/           CI per service
docker-compose.yml
```
