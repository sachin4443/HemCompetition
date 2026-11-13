import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// The production build is written into the API's wwwroot and served by it.
// In development the Vite server proxies /api to the API. The default matches the "https" launch
// profile (https://localhost:7190, self-signed certificate accepted); override with VITE_API_TARGET,
// e.g. VITE_API_TARGET=http://localhost:5098 for the "http" profile.
export default defineConfig({
  plugins: [react()],
  build: {
    outDir: '../Backend/HemCompetition.API/wwwroot',
    emptyOutDir: true,
    sourcemap: false,
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: process.env.VITE_API_TARGET || 'https://localhost:7190',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
