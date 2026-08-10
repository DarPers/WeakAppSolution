/** Sensor event returned by GraphQL Gateway. */
export type SensorEvent = {
  id: string
  type: string
  name: string
  payload: unknown
  receivedAt: string
  createdAt: string
}

/** Aggregation row from GetStatsByType / GetStatsByLocation. */
export type TypeStats = {
  type: string
  count: number
}

/** Payload shape used by Notification Service SignalR broadcasts. */
export type SensorNotification = {
  events: Array<{
    type: string
    name: string
    payload: unknown
  }>
  receivedAt: string
}

/** Dashboard filter values controlled by the UI. */
export type DashboardFilters = {
  type: string
  location: string
  from: string
  to: string
}
