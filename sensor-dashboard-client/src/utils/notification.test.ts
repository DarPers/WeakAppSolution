import { describe, expect, it } from 'vitest'
import { normalizeSensorNotification } from './notification'

describe('normalizeSensorNotification', () => {
  it('normalizes PascalCase SignalR payloads', () => {
    const result = normalizeSensorNotification({
      Events: [{ Type: 'temperature', Name: 'sensor-1', Payload: { value: 21 } }],
      ReceivedAt: '2026-01-01T00:00:00Z',
    })

    expect(result.events[0]).toEqual({
      type: 'temperature',
      name: 'sensor-1',
      payload: { value: 21 },
    })
  })
})
