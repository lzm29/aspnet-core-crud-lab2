import { defineConfig } from 'vite';

// Browser talks to Vite /api; Vite forwards to ASP.NET Core in the same Codespace.
// This also works on Windows without any CORS changes.
export default defineConfig({
  server: {
    host: '0.0.0.0',
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://127.0.0.1:5050',
        changeOrigin: true,
      },
    },
  },
});
