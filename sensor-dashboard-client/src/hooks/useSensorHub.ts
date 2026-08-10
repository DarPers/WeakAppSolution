import { useEffect, useState } from 'react'
import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr'

export type HubStatus = 'connecting' | 'connected' | 'reconnecting' | 'disconnected'

type UseSensorHubOptions = {
  /** Called whenever Notification Service pushes new sensor data. */
  onNotification?: (notification: unknown) => void
}

/**
 * Connects to Notification Service via SignalR and listens for SensorDataUpdated.
 * Hub URL defaults to same-origin /hubs/sensors (proxied in Vite and nginx).
 */
export function useSensorHub({ onNotification }: UseSensorHubOptions = {}) {
  const [status, setStatus] = useState<HubStatus>('connecting')

  useEffect(() => {
    const hubUrl = import.meta.env.VITE_SIGNALR_URL ?? '/hubs/sensors'

    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('SensorDataUpdated', (data: unknown) => {
      onNotification?.(data)
    })

    connection.onreconnecting(() => setStatus('reconnecting'))
    connection.onreconnected(() => setStatus('connected'))
    connection.onclose(() => setStatus('disconnected'))

    let cancelled = false

    const start = async () => {
      try {
        setStatus('connecting')
        await connection.start()
        if (!cancelled && connection.state === HubConnectionState.Connected) {
          setStatus('connected')
        }
      } catch {
        if (!cancelled) {
          setStatus('disconnected')
        }
      }
    }

    void start()

    return () => {
      cancelled = true
      connection.off('SensorDataUpdated')
      void connection.stop()
    }
    // Re-subscribe only when the callback identity changes.
  }, [onNotification])

  return { status }
}
