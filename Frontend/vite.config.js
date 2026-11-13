import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  build: {
    outDir: '../Backend/HemCompetition.API/wwwroot',
    emptyOutDir: true,
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
    target: 'https://localhost:7190',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})