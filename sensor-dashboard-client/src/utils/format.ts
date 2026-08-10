/**
 * Tries to read a numeric "value" from a sensor payload.
 * Payloads are free-form JSON; the WeakApp samples look like { "value": 42 }.
 */
export function extractNumericValue(payload: unknown): number | null {
  if (payload == null) {
    return null
  }

  if (typeof payload === 'number' && Number.isFinite(payload)) {
    return payload
  }

  if (typeof payload === 'object' && 'value' in payload) {
    const value = (payload as { value: unknown }).value
    if (typeof value === 'number' && Number.isFinite(value)) {
      return value
    }
    if (typeof value === 'string') {
      const parsed = Number(value)
      return Number.isFinite(parsed) ? parsed : null
    }
  }

  return null
}

/** Pretty-print payload for table cells. */
export function formatPayload(payload: unknown): string {
  if (payload == null) {
    return '—'
  }
  if (typeof payload === 'string') {
    return payload
  }
  try {
    return JSON.stringify(payload)
  } catch {
    return String(payload)
  }
}

/** Format an ISO date for compact UI display. */
export function formatDateTime(iso: string): string {
  const date = new Date(iso)
  if (Number.isNaN(date.getTime())) {
    return iso
  }
  return date.toLocaleString()
}
