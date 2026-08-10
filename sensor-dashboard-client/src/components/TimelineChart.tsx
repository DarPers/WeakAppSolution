import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import type { SensorEvent } from '../types/sensor'
import { extractNumericValue } from '../utils/format'

type TimelineChartProps = {
  events: SensorEvent[]
}

/** Timeline of numeric payload values (falls back to event count markers). */
export function TimelineChart({ events }: TimelineChartProps) {
  // Sort oldest → newest so the line reads left-to-right over time.
  const points = [...events]
    .slice()
    .sort((a, b) => new Date(a.receivedAt).getTime() - new Date(b.receivedAt).getTime())
    .map((event, index) => {
      const numeric = extractNumericValue(event.payload)
      return {
        time: new Date(event.receivedAt).toLocaleTimeString(),
        value: numeric ?? index + 1,
        label: event.name,
        isFallback: numeric == null,
      }
    })

  if (points.length === 0) {
    return (
      <p className="rounded-lg border border-dashed border-slate-300 bg-slate-50 px-4 py-8 text-center text-sm text-slate-500">
        No timeline data available.
      </p>
    )
  }

  return (
    <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
      <h2 className="mb-1 text-sm font-semibold text-slate-800">Timeline</h2>
      <p className="mb-4 text-xs text-slate-500">
        Numeric payload values over time (or event index when value is missing).
      </p>

      <div className="h-64 w-full">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={points} margin={{ top: 8, right: 12, left: 0, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="#e2e8f0" />
            <XAxis dataKey="time" tick={{ fontSize: 11, fill: '#64748b' }} />
            <YAxis tick={{ fontSize: 11, fill: '#64748b' }} width={40} />
            <Tooltip
              contentStyle={{
                borderRadius: 8,
                borderColor: '#e2e8f0',
                fontSize: 12,
              }}
            />
            <Line
              type="monotone"
              dataKey="value"
              stroke="#0f766e"
              strokeWidth={2}
              dot={{ r: 3, fill: '#0f766e' }}
              activeDot={{ r: 5 }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  )
}
