type LoadingStateProps = {
  label?: string
}

/** Simple full-width loading placeholder. */
export function LoadingState({ label = 'Loading…' }: LoadingStateProps) {
  return (
    <div
      className="flex min-h-32 items-center justify-center rounded-lg border border-slate-200 bg-white text-slate-500"
      role="status"
    >
      <span className="animate-pulse text-sm font-medium">{label}</span>
    </div>
  )
}
