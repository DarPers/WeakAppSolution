import type { DashboardFilters } from '../types/sensor'

/** Builds a HotChocolate SensorEventFilterInput from UI filter state. */
export function buildEventWhere(filters: DashboardFilters) {
  const and: Record<string, unknown>[] = []

  // Partial match for text filters (HotChocolate StringOperationFilterInput.contains).
  if (filters.type.trim()) {
    and.push({ type: { contains: filters.type.trim() } })
  }

  if (filters.location.trim()) {
    and.push({ name: { contains: filters.location.trim() } })
  }

  if (filters.from) {
    and.push({ receivedAt: { gte: new Date(filters.from).toISOString() } })
  }

  if (filters.to) {
    and.push({ receivedAt: { lte: new Date(filters.to).toISOString() } })
  }

  if (and.length === 0) {
    return undefined
  }

  return { and }
}

export const emptyFilters: DashboardFilters = {
  type: '',
  location: '',
  from: '',
  to: '',
}
