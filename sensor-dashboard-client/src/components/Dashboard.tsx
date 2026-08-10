import { useCallback, useState } from 'react'
import { useApolloClient, useQuery } from '@apollo/client/react'
import { GET_SENSOR_EVENTS, GET_STATS } from '../graphql/queries'
import { useSensorHub } from '../hooks/useSensorHub'
import type { DashboardFilters, SensorEvent, TypeStats } from '../types/sensor'
import { buildEventWhere, emptyFilters } from '../utils/filters'
import { formatPayload } from '../utils/format'
import { normalizeSensorNotification } from '../utils/notification'
import { ConnectionStatus } from './ConnectionStatus'
import { ErrorState } from './ErrorState'
import { FiltersBar } from './FiltersBar'
import { LatestEventsTable } from './LatestEventsTable'
import { LoadingState } from './LoadingState'
import { StatsCharts } from './StatsCharts'
import { TimelineChart } from './TimelineChart'
import { ToastStack, type ToastItem } from './ToastStack'

type SensorEventsQuery = {
  sensorEvents: {
    nodes: SensorEvent[]
    totalCount: number
  }
}

type StatsQuery = {
  statsByType: TypeStats[]
  statsByLocation: TypeStats[]
}

const MAX_TOASTS = 5

/** Main dashboard: latest events, timeline, aggregations, live SignalR updates. */
export function Dashboard() {
  const client = useApolloClient()
  const [filters, setFilters] = useState<DashboardFilters>(emptyFilters)
  const [toasts, setToasts] = useState<ToastItem[]>([])

  const where = buildEventWhere(filters)

  const eventsQuery = useQuery<SensorEventsQuery>(GET_SENSOR_EVENTS, {
    variables: {
      first: 25,
      where,
      order: [{ receivedAt: 'DESC' }],
    },
  })

  const statsQuery = useQuery<StatsQuery>(GET_STATS)

  const dismissToast = useCallback((id: string) => {
    setToasts((current) => current.filter((toast) => toast.id !== id))
  }, [])

  // Refetch GraphQL data and show a right-side toast when new events arrive.
  const onNotification = useCallback(
    (raw: unknown) => {
      void client.refetchQueries({ include: 'active' })

      const notification = normalizeSensorNotification(raw)
      const nextToasts = notification.events.map((event, index) => ({
        id: `${Date.now()}-${index}-${event.name}`,
        title: `New ${event.type}`,
        message: `${event.name}: ${formatPayload(event.payload)}`,
      }))

      if (nextToasts.length === 0) {
        nextToasts.push({
          id: `${Date.now()}-batch`,
          title: 'New sensor data',
          message: 'Live update received from Notification Service',
        })
      }

      setToasts((current) => [...nextToasts, ...current].slice(0, MAX_TOASTS))
    },
    [client],
  )

  const { status } = useSensorHub({ onNotification })

  const events = eventsQuery.data?.sensorEvents.nodes ?? []
  const eventsLoading = eventsQuery.loading && !eventsQuery.data
  const statsLoading = statsQuery.loading && !statsQuery.data

  return (
    <div className="mx-auto flex min-h-svh max-w-6xl flex-col gap-5 px-4 py-6 sm:px-6">
      <ToastStack toasts={toasts} onDismiss={dismissToast} />

      <header className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
        <div className="text-left">
          <p className="text-xs font-semibold uppercase tracking-[0.18em] text-teal-700">
            WeakApp Solution
          </p>
          <h1 className="mt-1 text-2xl font-semibold tracking-tight text-slate-900 sm:text-3xl">
            Sensor Event Dashboard
          </h1>
          <p className="mt-1 text-sm text-slate-600">
            Latest values, charts and live updates from GraphQL Gateway + Notification Service.
          </p>
        </div>
        <ConnectionStatus status={status} />
      </header>

      <FiltersBar
        filters={filters}
        onChange={setFilters}
        onReset={() => setFilters(emptyFilters)}
      />

      <section className="space-y-3">
        {eventsLoading ? <LoadingState label="Loading latest events…" /> : null}
        {eventsQuery.error ? (
          <ErrorState
            title="Failed to load sensor events"
            message={eventsQuery.error.message}
            onRetry={() => void eventsQuery.refetch()}
          />
        ) : null}
        {!eventsLoading && !eventsQuery.error ? (
          <LatestEventsTable
            events={events}
            totalCount={eventsQuery.data?.sensorEvents.totalCount}
          />
        ) : null}
      </section>

      <section>
        {eventsLoading ? <LoadingState label="Loading timeline…" /> : null}
        {!eventsLoading && !eventsQuery.error ? <TimelineChart events={events} /> : null}
      </section>

      <section>
        {statsLoading ? <LoadingState label="Loading aggregations…" /> : null}
        {statsQuery.error ? (
          <ErrorState
            title="Failed to load aggregations"
            message={statsQuery.error.message}
            onRetry={() => void statsQuery.refetch()}
          />
        ) : null}
        {!statsLoading && !statsQuery.error ? (
          <StatsCharts
            byType={statsQuery.data?.statsByType ?? []}
            byLocation={statsQuery.data?.statsByLocation ?? []}
          />
        ) : null}
      </section>
    </div>
  )
}
