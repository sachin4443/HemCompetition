// Copies the MediaPipe WASM runtime next to the app so face detection is served from our own
// origin (same version as the npm package, no third-party CDN at exam time).
import { cpSync, existsSync, mkdirSync } from 'node:fs'
import { dirname, resolve } from 'node:path'
import { fileURLToPath } from 'node:url'

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..')
const from = resolve(root, 'node_modules/@mediapipe/tasks-vision/wasm')
const to = resolve(root, 'public/mediapipe-wasm')
if (!existsSync(from)) {
  console.error('MediaPipe WASM files not found - run npm install first.')
  process.exit(1)
}
mkdirSync(to, { recursive: true })
cpSync(from, to, { recursive: true })
console.log('MediaPipe WASM copied to public/mediapipe-wasm')
