type ErrorStateProps = {
  title?: string
  message: string
  onRetry?: () => void
}

/** Fallback UI when a GraphQL request fails. */
export function ErrorState({
  title = 'Something went wrong',
  message,
  onRetry,
}: ErrorStateProps) {
  return (
    <div
      className="rounded-lg border border-rose-200 bg-rose-50 px-4 py-5 text-left"
      role="alert"
    >
      <p className="text-sm font-semibold text-rose-800">{title}</p>
      <p className="mt-1 text-sm text-rose-700">{message}</p>
      {onRetry ? (
        <button
          type="button"
          onClick={onRetry}
          className="mt-3 rounded-md bg-rose-700 px-3 py-1.5 text-sm font-medium text-white hover:bg-rose-800"
        >
          Retry
        </button>
      ) : null}
    </div>
  )
}
