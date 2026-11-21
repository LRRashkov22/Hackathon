import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // The API listens on http://localhost:5000 by default (see src/Passalong.Api launchSettings).
      '/api': process.env.VITE_API_PROXY ?? 'http://localhost:5000',
    },
  },
})
