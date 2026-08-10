import type { SensorEvent } from '../types/sensor'
import { formatDateTime, formatPayload } from '../utils/format'

type LatestEventsTableProps = {
  events: SensorEvent[]
  totalCount?: number | null
}

/** Table of the most recent sensor events from GraphQL. */
export function LatestEventsTable({ events, totalCount }: LatestEventsTableProps) {
  if (events.length === 0) {
    return (
      <p className="rounded-lg border border-dashed border-slate-300 bg-slate-50 px-4 py-8 text-center text-sm text-slate-500">
        No sensor events match the current filters.
      </p>
    )
  }

  return (
    <div className="overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm">
      <div className="flex items-center justify-between border-b border-slate-100 px-4 py-3">
        <h2 className="text-sm font-semibold text-slate-800">Latest values</h2>
        {typeof totalCount === 'number' ? (
          <span className="text-xs text-slate-500">{totalCount} total</span>
        ) : null}
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full text-left text-sm">
          <thead className="bg-slate-50 text-xs uppercase tracking-wide text-slate-500">
            <tr>
              <th className="px-4 py-3 font-medium">Received</th>
              <th className="px-4 py-3 font-medium">Type</th>
              <th className="px-4 py-3 font-medium">Location</th>
              <th className="px-4 py-3 font-medium">Payload</th>
            </tr>
          </thead>
          <tbody>
            {events.map((event) => (
              <tr key={event.id} className="border-t border-slate-100 hover:bg-slate-50/80">
                <td className="whitespace-nowrap px-4 py-3 text-slate-700">
                  {formatDateTime(event.receivedAt)}
                </td>
                <td className="px-4 py-3">
                  <span className="rounded bg-teal-50 px-2 py-0.5 text-xs font-medium text-teal-800">
                    {event.type}
                  </span>
                </td>
                <td className="px-4 py-3 font-medium text-slate-800">{event.name}</td>
                <td className="max-w-xs truncate px-4 py-3 font-mono text-xs text-slate-600">
                  {formatPayload(event.payload)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}
