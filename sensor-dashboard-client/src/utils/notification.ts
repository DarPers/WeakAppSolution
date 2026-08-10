import type { SensorNotification } from '../types/sensor'

/**
 * SignalR may send camelCase or PascalCase depending on serializer settings.
 * Normalize to the shape the dashboard expects.
 */
export function normalizeSensorNotification(data: unknown): SensorNotification {
  const raw = (data ?? {}) as Record<string, unknown>
  const eventsRaw = (raw.events ?? raw.Events ?? []) as Array<Record<string, unknown>>
  const receivedAt = String(raw.receivedAt ?? raw.ReceivedAt ?? new Date().toISOString())

  return {
    receivedAt,
    events: eventsRaw.map((event) => ({
      type: String(event.type ?? event.Type ?? 'unknown'),
      name: String(event.name ?? event.Name ?? 'unknown'),
      payload: event.payload ?? event.Payload ?? null,
    })),
  }
}
