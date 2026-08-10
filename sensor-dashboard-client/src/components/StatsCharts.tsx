import {
  Bar,
  BarChart,
  Cell,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import type { TypeStats } from '../types/sensor'

const COLORS = ['#0f766e', '#0369a1', '#b45309', '#7c3aed', '#be123c', '#15803d']

type StatsChartsProps = {
  byType: TypeStats[]
  byLocation: TypeStats[]
}

/** Bar chart for type aggregations and pie chart for location aggregations. */
export function StatsCharts({ byType, byLocation }: StatsChartsProps) {
  return (
    <div className="grid gap-4 lg:grid-cols-2">
      <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
        <h2 className="mb-1 text-sm font-semibold text-slate-800">By type</h2>
        <p className="mb-4 text-xs text-slate-500">Event counts from GetStatsByType</p>

        {byType.length === 0 ? (
          <EmptyChart />
        ) : (
          <div className="h-64 w-full">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={byType} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
                <XAxis dataKey="type" tick={{ fontSize: 11, fill: '#64748b' }} />
                <YAxis allowDecimals={false} tick={{ fontSize: 11, fill: '#64748b' }} width={36} />
                <Tooltip
                  contentStyle={{
                    borderRadius: 8,
                    borderColor: '#e2e8f0',
                    fontSize: 12,
                  }}
                />
                <Bar dataKey="count" fill="#0f766e" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        )}
      </div>

      <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
        <h2 className="mb-1 text-sm font-semibold text-slate-800">By location</h2>
        <p className="mb-4 text-xs text-slate-500">Event counts from GetStatsByLocation</p>

        {byLocation.length === 0 ? (
          <EmptyChart />
        ) : (
          <div className="h-64 w-full">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={byLocation}
                  dataKey="count"
                  nameKey="type"
                  cx="50%"
                  cy="50%"
                  outerRadius={90}
                  label={({ name, percent }) =>
                    `${name ?? ''} ${((percent ?? 0) * 100).toFixed(0)}%`
                  }
                >
                  {byLocation.map((entry, index) => (
                    <Cell key={entry.type} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip
                  contentStyle={{
                    borderRadius: 8,
                    borderColor: '#e2e8f0',
                    fontSize: 12,
                  }}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        )}
      </div>
    </div>
  )
}

function EmptyChart() {
  return (
    <p className="rounded-lg border border-dashed border-slate-300 bg-slate-50 px-4 py-8 text-center text-sm text-slate-500">
      No aggregation data yet.
    </p>
  )
}
