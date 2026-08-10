import { describe, expect, it } from 'vitest'
import { buildEventWhere, emptyFilters } from '../utils/filters'
import { extractNumericValue, formatPayload } from '../utils/format'

describe('extractNumericValue', () => {
  it('reads value from payload object', () => {
    expect(extractNumericValue({ value: 42 })).toBe(42)
  })

  it('returns null for non-numeric payloads', () => {
    expect(extractNumericValue({ status: 'ok' })).toBeNull()
  })
})

describe('formatPayload', () => {
  it('stringifies objects', () => {
    expect(formatPayload({ value: 1 })).toBe('{"value":1}')
  })
})

describe('buildEventWhere', () => {
  it('returns undefined when filters are empty', () => {
    expect(buildEventWhere(emptyFilters)).toBeUndefined()
  })

  it('builds and-conditions for filled filters', () => {
    const where = buildEventWhere({
      type: 'temperature',
      location: 'sensor-1',
      from: '2026-01-01T00:00',
      to: '',
    })

    expect(where).toEqual({
      and: [
        { type: { contains: 'temperature' } },
        { name: { contains: 'sensor-1' } },
        { receivedAt: { gte: new Date('2026-01-01T00:00').toISOString() } },
      ],
    })
  })
})
