import type { DashboardFilters } from '../types/sensor'

type FiltersBarProps = {
  filters: DashboardFilters
  onChange: (next: DashboardFilters) => void
  onReset: () => void
}

/** Optional filters: type, location (sensor name), and time range. */
export function FiltersBar({ filters, onChange, onReset }: FiltersBarProps) {
  const update = (field: keyof DashboardFilters, value: string) => {
    onChange({ ...filters, [field]: value })
  }

  return (
    <section className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
      <div className="mb-3 flex items-center justify-between gap-3">
        <h2 className="text-sm font-semibold text-slate-800">Filters</h2>
        <button
          type="button"
          onClick={onReset}
          className="text-xs font-medium text-teal-700 hover:text-teal-900"
        >
          Reset
        </button>
      </div>

      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
        <label className="block text-left text-xs font-medium text-slate-600">
          Type
          <input
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-800 outline-none focus:border-teal-500 focus:ring-2 focus:ring-teal-100"
            value={filters.type}
            onChange={(e) => update('type', e.target.value)}
            placeholder="e.g. temperature"
          />
        </label>

        <label className="block text-left text-xs font-medium text-slate-600">
          Location / name
          <input
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-800 outline-none focus:border-teal-500 focus:ring-2 focus:ring-teal-100"
            value={filters.location}
            onChange={(e) => update('location', e.target.value)}
            placeholder="e.g. sensor-1"
          />
        </label>

        <label className="block text-left text-xs font-medium text-slate-600">
          From
          <input
            type="datetime-local"
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-800 outline-none focus:border-teal-500 focus:ring-2 focus:ring-teal-100"
            value={filters.from}
            onChange={(e) => update('from', e.target.value)}
          />
        </label>

        <label className="block text-left text-xs font-medium text-slate-600">
          To
          <input
            type="datetime-local"
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-800 outline-none focus:border-teal-500 focus:ring-2 focus:ring-teal-100"
            value={filters.to}
            onChange={(e) => update('to', e.target.value)}
          />
        </label>
      </div>
    </section>
  )
}
