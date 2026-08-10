import { useEffect } from 'react'

export type ToastItem = {
  id: string
  title: string
  message: string
}

type ToastStackProps = {
  toasts: ToastItem[]
  onDismiss: (id: string) => void
  /** Auto-hide delay in ms. */
  durationMs?: number
}

/** Fixed toast list on the right side of the viewport. */
export function ToastStack({ toasts, onDismiss, durationMs = 5000 }: ToastStackProps) {
  return (
    <div
      className="pointer-events-none fixed top-4 right-4 z-50 flex w-[min(100%-2rem,22rem)] flex-col gap-2"
      aria-live="polite"
    >
      {toasts.map((toast) => (
        <ToastCard
          key={toast.id}
          toast={toast}
          durationMs={durationMs}
          onDismiss={onDismiss}
        />
      ))}
    </div>
  )
}

type ToastCardProps = {
  toast: ToastItem
  durationMs: number
  onDismiss: (id: string) => void
}

function ToastCard({ toast, durationMs, onDismiss }: ToastCardProps) {
  useEffect(() => {
    const timer = window.setTimeout(() => onDismiss(toast.id), durationMs)
    return () => window.clearTimeout(timer)
  }, [toast.id, durationMs, onDismiss])

  return (
    <div className="pointer-events-auto animate-[slide-in_0.25s_ease-out] rounded-lg border border-teal-200 bg-white px-4 py-3 shadow-lg shadow-slate-900/10">
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0 text-left">
          <p className="text-sm font-semibold text-slate-900">{toast.title}</p>
          <p className="mt-0.5 truncate text-xs text-slate-600">{toast.message}</p>
        </div>
        <button
          type="button"
          onClick={() => onDismiss(toast.id)}
          className="shrink-0 text-slate-400 hover:text-slate-700"
          aria-label="Dismiss notification"
        >
          ×
        </button>
      </div>
    </div>
  )
}
