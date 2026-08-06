import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'

// document.getElementById can return null if the element isn't found, so
// TypeScript requires confirming it exists before passing it to createRoot.
// The "!" (non-null assertion) tells TypeScript we're certain #root exists,
// since it's defined directly in index.html.
createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
