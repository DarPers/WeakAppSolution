import type { HubStatus } from '../hooks/useSensorHub'

const statusLabel: Record<HubStatus, string> = {
  connecting: 'Connecting…',
  connected: 'Live',
  reconnecting: 'Reconnecting…',
  disconnected: 'Offline',
}

const statusColor: Record<HubStatus, string> = {
  connecting: 'bg-amber-400',
  connected: 'bg-emerald-500',
  reconnecting: 'bg-amber-400',
  disconnected: 'bg-slate-400',
}

type ConnectionStatusProps = {
  status: HubStatus
}

/** Small SignalR connection indicator in the header. */
export function ConnectionStatus({ status }: ConnectionStatusProps) {
  return (
    <div className="inline-flex items-center gap-2 rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-600">
      <span className={`h-2 w-2 rounded-full ${statusColor[status]}`} aria-hidden />
      <span>SignalR: {statusLabel[status]}</span>
    </div>
  )
}
