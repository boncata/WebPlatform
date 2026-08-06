/// <reference types="vitest/config" />
import path from "node:path"
import { fileURLToPath } from "node:url"
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    // Mirrors the "@/*" path alias in tsconfig.app.json — TypeScript's
    // "paths" only affects type-checking, so Vite's own bundler needs this
    // separate alias to actually resolve "@/..." imports at build/dev time.
    // This alias itself works correctly (confirmed via normal builds and
    // the dev server); it just isn't enough on its own for `shadcn add` —
    // see README.md "Known Issues" for a CLI bug that requires passing
    // --path explicitly when adding components.
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  test: {
    environment: "jsdom",
    setupFiles: "./src/test/setup.ts"
  }
})
