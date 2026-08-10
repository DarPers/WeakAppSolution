/// <reference types="vitest/config" />
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    // Proxy GraphQL and SignalR so the browser stays same-origin (no CORS issues in local dev).
    proxy: {
      '/graphql': {
        target: 'http://localhost:6062',
        changeOrigin: true,
      },
      '/hubs': {
        target: 'http://localhost:2000',
        changeOrigin: true,
        ws: true,
      },
    },
  },
  test: {
    environment: 'happy-dom',
    globals: true,
    setupFiles: './src/test/setup.ts',
  },
})
