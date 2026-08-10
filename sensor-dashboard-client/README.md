# Sensor Event Dashboard (React + Vite + TypeScript)

SPA that talks to GraphQL Gateway and Notification Service (SignalR).

## Features

- Latest sensor events table (Apollo Client + HotChocolate filters/sorting)
- Timeline chart (Recharts)
- Aggregations by type / location (`statsByType`, `statsByLocation`)
- Live updates via `@microsoft/signalr`
- Responsive Tailwind UI with loading and error states
- Optional filters: type, location/name, date range

## Local development

Prerequisites: GraphQL Gateway on `localhost:6062`, Notification Service on `localhost:2000`.

```bash
npm install
npm run dev
```

Vite proxies `/graphql` → gateway and `/hubs` → notification service.

## Scripts

| Script | Description |
|--------|-------------|
| `npm run dev` | Vite dev server |
| `npm run build` | Typecheck + production build |
| `npm run lint` | ESLint |
| `npm test` | Vitest unit tests |
| `npm run preview` | Preview production build |

## Docker

```bash
docker build -t sensor-dashboard-client .
docker run --rm -p 5173:80 sensor-dashboard-client
```

Or use the `sensor-dashboard` service in the root `docker-compose.yml`.

Optional build-time overrides:

- `VITE_GRAPHQL_URL` (default `/graphql`)
- `VITE_SIGNALR_URL` (default `/hubs/sensors`)
