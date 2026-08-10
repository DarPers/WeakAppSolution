import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import { LoadingState } from '../components/LoadingState'
import { ErrorState } from '../components/ErrorState'

describe('LoadingState', () => {
  it('renders the loading label', () => {
    render(<LoadingState label="Loading events…" />)
    expect(screen.getByRole('status')).toHaveTextContent('Loading events…')
  })
})

describe('ErrorState', () => {
  it('renders error message and retry button', () => {
    render(
      <ErrorState
        title="Failed"
        message="Network down"
        onRetry={() => undefined}
      />,
    )

    expect(screen.getByRole('alert')).toHaveTextContent('Network down')
    expect(screen.getByRole('button', { name: 'Retry' })).toBeInTheDocument()
  })
})
