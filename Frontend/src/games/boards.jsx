import { useEffect, useMemo, useRef, useState } from 'react'

/*
 * One board per game engine. Each board receives the puzzle the server generated and reports the
 * student's current answer upwards with onChange. Boards never know the solution: checking is
 * always done by the server when the answer is submitted.
 */

// ----------------------------------------------------------------------------- Game 24

export function Game24Board({ puzzle, onChange, disabled }) {
  const [tokens, setTokens] = useState([]) // { text, send, numberIndex? }
  const used = tokens.filter((t) => t.numberIndex != null).map((t) => t.numberIndex)
  const last = tokens[tokens.length - 1]

  useEffect(() => { onChange({ expression: tokens.map((t) => t.send).join('') }) }, [tokens, onChange])

  const add = (token) => setTokens((ts) => [...ts, token])
  const OPS = [['+', '+'], ['−', '-'], ['×', '*'], ['÷', '/'], ['(', '('], [')', ')']]

  return (
    <div className="text-center">
      <p className="text-secondary small mb-2">Use all four numbers once to make <strong>{puzzle.target}</strong></p>
      <div className="g24-display border rounded-3 bg-light d-flex align-items-center justify-content-center px-2 mb-3" aria-live="polite" aria-label="Your expression">
        {tokens.length ? tokens.map((t) => t.text).join(' ') : <span className="text-secondary fs-6 fw-normal">Tap numbers and signs</span>}
      </div>
      <div className="d-flex justify-content-center gap-2 gap-sm-3 mb-3">
        {puzzle.numbers.map((n, i) => (
          <button key={i} type="button" className={`btn g24-number ${used.includes(i) ? 'btn-secondary' : 'btn-success'}`}
            // Two numbers in a row would glue into one bigger number, which is not allowed.
            disabled={disabled || used.includes(i) || last?.numberIndex != null} onClick={() => add({ text: String(n), send: String(n), numberIndex: i })}>
            {n}
          </button>
        ))}
      </div>
      <div className="d-flex justify-content-center flex-wrap gap-2 mb-3">
        {OPS.map(([text, send]) => (
          <button key={send} type="button" className="btn btn-outline-dark g24-op" disabled={disabled} onClick={() => add({ text, send })} aria-label={`Sign ${text}`}>{text}</button>
        ))}
      </div>
      <div className="d-flex justify-content-center gap-2">
        <button type="button" className="btn btn-outline-secondary rounded-pill px-3" disabled={disabled || !tokens.length} onClick={() => setTokens((ts) => ts.slice(0, -1))}>
          <i className="bi bi-backspace me-1" aria-hidden="true" />Erase
        </button>
        <button type="button" className="btn btn-outline-secondary rounded-pill px-3" disabled={disabled || !tokens.length} onClick={() => setTokens([])}>
          <i className="bi bi-arrow-counterclockwise me-1" aria-hidden="true" />Clear
        </button>
      </div>
    </div>
  )
}

// ----------------------------------------------------------------------------- shared number grid

function useNumberGrid(size, givens, onChange) {
  const [grid, setGrid] = useState(() => Array.from({ length: size }, (_, r) => Array.from({ length: size }, (_, c) => givens?.[r][c] || 0)))
  useEffect(() => { onChange({ grid }) }, [grid, onChange])
  const refs = useRef({})
  const set = (r, c, raw) => {
    const digit = raw.replace(/\D/g, '').slice(-1)
    const value = digit && Number(digit) >= 1 && Number(digit) <= size ? Number(digit) : 0
    setGrid((g) => g.map((row, ri) => row.map((v, ci) => (ri === r && ci === c ? value : v))))
  }
  const onKeyDown = (r, c) => (e) => {
    const move = { ArrowUp: [-1, 0], ArrowDown: [1, 0], ArrowLeft: [0, -1], ArrowRight: [0, 1] }[e.key]
    if (!move) return
    e.preventDefault()
    for (let step = 1; step <= size; step++) {
      const target = refs.current[`${(r + move[0] * step + size) % size}-${(c + move[1] * step + size) % size}`]
      if (target && !target.disabled) { target.focus(); target.select(); break }
    }
  }
  const input = (r, c, extra = {}) => (
    <input ref={(el) => { refs.current[`${r}-${c}`] = el }} inputMode="numeric" pattern="[0-9]*" maxLength={1} autoComplete="off"
      aria-label={`Row ${r + 1}, column ${c + 1}`} value={grid[r][c] || ''} onChange={(e) => set(r, c, e.target.value)}
      onFocus={(e) => e.target.select()} onKeyDown={onKeyDown(r, c)} {...extra} />
  )
  return { grid, input }
}

// ----------------------------------------------------------------------------- KenKen

const CAGE_COLORS = ['#fde68a', '#bfdbfe', '#fecaca', '#bbf7d0', '#ddd6fe', '#fed7aa', '#a5f3fc', '#fbcfe8', '#d9f99d', '#e2e8f0', '#fef08a', '#c7d2fe', '#fda4af', '#99f6e4']

export function KenKenBoard({ puzzle, onChange, disabled }) {
  const { size, cageGrid, labelGrid } = puzzle
  const { input } = useNumberGrid(size, null, onChange)
  const color = (id) => CAGE_COLORS[(id.charCodeAt(0) - 65) % CAGE_COLORS.length]
  const edge = (r, c, dr, dc) => {
    const other = cageGrid[r + dr]?.[c + dc]
    return other === cageGrid[r][c] ? '1px solid rgba(0,0,0,.12)' : '3px solid #1e293b'
  }
  return (
    <div className="text-center">
      <p className="text-secondary small mb-3">Fill 1–{size}. No repeats in a row or column. Each coloured cage must make its target.</p>
      <div className="grid-board" style={{ '--n': size, gridTemplateColumns: `repeat(${size}, var(--cell))` }} role="grid" aria-label="KenKen grid">
        {cageGrid.map((row, r) => row.map((id, c) => (
          <div key={`${r}-${c}`} className="grid-cell kenken-cell" role="gridcell"
            style={{ background: color(id), borderTop: edge(r, c, -1, 0), borderBottom: edge(r, c, 1, 0), borderLeft: edge(r, c, 0, -1), borderRight: edge(r, c, 0, 1) }}>
            {labelGrid[r][c] && <span className="kenken-label">{labelGrid[r][c].replace('x', '×').replace('/', '÷').replace('-', '−')}</span>}
            {input(r, c, { disabled })}
          </div>
        )))}
      </div>
    </div>
  )
}

// ----------------------------------------------------------------------------- Futoshiki

export function FutoshikiBoard({ puzzle, onChange, disabled }) {
  const { size, givens, hSigns, vSigns } = puzzle
  const { input } = useNumberGrid(size, givens, onChange)
  const cells = []
  for (let r = 0; r < size; r++) {
    for (let c = 0; c < size; c++) {
      const given = givens[r][c] !== 0
      cells.push(<div key={`c${r}-${c}`} className={`grid-cell futo-cell ${given ? 'given' : ''}`}>{input(r, c, { disabled: disabled || given, readOnly: given })}</div>)
      if (c < size - 1) cells.push(<div key={`h${r}-${c}`} className="futo-sign" aria-hidden={!hSigns[r][c]}>{hSigns[r][c]}</div>)
    }
    if (r < size - 1) {
      for (let c = 0; c < size; c++) {
        // "<" between rows means the upper cell is the smaller one.
        cells.push(<div key={`v${r}-${c}`} className="futo-sign" style={{ width: 'var(--cell)' }} aria-hidden={!vSigns[r][c]}>{vSigns[r][c] === '<' ? '∧' : vSigns[r][c] === '>' ? '∨' : ''}</div>)
        if (c < size - 1) cells.push(<div key={`x${r}-${c}`} />)
      }
    }
  }
  const template = Array.from({ length: size * 2 - 1 }, (_, i) => (i % 2 === 0 ? 'var(--cell)' : 'calc(var(--cell) * .45)')).join(' ')
  return (
    <div className="text-center">
      <p className="text-secondary small mb-3">Fill 1–{size}. No repeats in a row or column. Every &lt; &gt; ∧ ∨ sign must be true (the point faces the smaller number).</p>
      <div className="grid-board align-items-center justify-items-center" style={{ '--n': size, gridTemplateColumns: template, justifyItems: 'center', alignItems: 'center' }} aria-label="Futoshiki grid">
        {cells}
      </div>
    </div>
  )
}

// ----------------------------------------------------------------------------- Scramble

function LetterInputs({ length, highlight, value, onChange, disabled, label }) {
  const refs = useRef([])
  const chars = Array.from({ length }, (_, i) => value[i] || '')
  const setAt = (i, ch) => {
    const next = chars.slice()
    next[i] = ch
    onChange(next.join(''))
  }
  return (
    <div className="letter-row" role="group" aria-label={label}>
      {chars.map((ch, i) => (
        <input key={i} ref={(el) => { refs.current[i] = el }} className={`letter-box ${i === highlight ? 'highlight' : ''}`} maxLength={1} value={ch === ' ' ? '' : ch}
          disabled={disabled} autoComplete="off" autoCapitalize="characters" spellCheck={false} aria-label={`${label}, letter ${i + 1}${i === highlight ? ' (highlighted)' : ''}`}
          onChange={(e) => {
            const letter = e.target.value.replace(/[^a-zA-Z]/g, '').slice(-1).toUpperCase()
            setAt(i, letter || ' ')
            if (letter) refs.current[i + 1]?.focus()
          }}
          onKeyDown={(e) => {
            if (e.key === 'Backspace' && !chars[i].trim()) refs.current[i - 1]?.focus()
            if (e.key === 'ArrowLeft') refs.current[i - 1]?.focus()
            if (e.key === 'ArrowRight') refs.current[i + 1]?.focus()
          }}
          onFocus={(e) => e.target.select()} />
      ))}
    </div>
  )
}

export function ScrambleBoard({ puzzle, onChange, disabled }) {
  const [words, setWords] = useState(() => puzzle.rows.map(() => ''))
  const [final, setFinal] = useState('')
  useEffect(() => { onChange({ words: words.map((w) => w.replace(/ /g, '')), final: final.replace(/ /g, '') }) }, [words, final, onChange])
  return (
    <div>
      <p className="text-secondary small text-center mb-3">Unscramble each word. Then rearrange the letters in the <span className="px-1 rounded" style={{ background: '#fde68a' }}>yellow</span> boxes to make the final word.</p>
      {puzzle.rows.map((row, r) => (
        <div key={r} className="mb-3 pb-3 border-bottom">
          <div className="d-flex justify-content-center flex-wrap gap-1 mb-2" aria-label={`Jumbled letters for word ${r + 1}`}>
            {row.scrambled.split('').map((ch, i) => <span key={i} className="letter-tile">{ch}</span>)}
          </div>
          <LetterInputs length={row.length} highlight={row.highlight} value={words[r]} disabled={disabled} label={`Word ${r + 1}`}
            onChange={(v) => setWords((ws) => ws.map((w, i) => (i === r ? v : w)))} />
        </div>
      ))}
      <div className="text-center">
        <div className="fw-semibold small mb-2">Final word ({puzzle.finalLength} letters)</div>
        <LetterInputs length={puzzle.finalLength} highlight={-1} value={final} disabled={disabled} label="Final word" onChange={setFinal} />
      </div>
    </div>
  )
}

// ----------------------------------------------------------------------------- Word search ("Puzzle")

const lineCells = (a, b) => {
  const dr = b[0] - a[0], dc = b[1] - a[1]
  if (!(dr === 0 || dc === 0 || Math.abs(dr) === Math.abs(dc))) return null
  const steps = Math.max(Math.abs(dr), Math.abs(dc))
  return Array.from({ length: steps + 1 }, (_, i) => [a[0] + Math.sign(dr) * i, a[1] + Math.sign(dc) * i])
}

export function WordSearchBoard({ puzzle, onChange, onComplete, disabled }) {
  const { grid, words, size } = puzzle
  const [found, setFound] = useState([]) // { word, start, end }
  const [anchor, setAnchor] = useState(null)
  const [hover, setHover] = useState(null)
  const dragging = useRef(false)
  const completed = useRef(false)

  // The parent's callbacks change identity on every render; only a change in `found` should report.
  const callbacks = useRef({ onChange, onComplete })
  useEffect(() => { callbacks.current = { onChange, onComplete } })
  useEffect(() => {
    const answer = { found: found.map(({ start, end }) => ({ start, end })) }
    callbacks.current.onChange(answer)
    if (found.length === words.length && !completed.current) {
      completed.current = true
      callbacks.current.onComplete?.(answer)
    }
  }, [found, words.length])

  const foundCells = useMemo(() => new Set(found.flatMap((f) => lineCells(f.start, f.end).map(([r, c]) => `${r}-${c}`))), [found])
  const selCells = useMemo(() => {
    if (!anchor) return new Set()
    return new Set((lineCells(anchor, hover || anchor) || [anchor]).map(([r, c]) => `${r}-${c}`))
  }, [anchor, hover])

  const tryCommit = (a, b) => {
    const cells = lineCells(a, b)
    if (!cells || cells.length < 2) return false
    const text = cells.map(([r, c]) => grid[r][c]).join('')
    const reversed = text.split('').reverse().join('')
    const word = words.find((w) => (w === text || w === reversed) && !found.some((f) => f.word === w))
    if (!word) return false
    setFound((f) => [...f, { word, start: a, end: b }])
    return true
  }

  const cellAt = (e) => {
    const el = document.elementFromPoint(e.clientX, e.clientY)?.closest?.('[data-cell]')
    return el ? el.dataset.cell.split('-').map(Number) : null
  }
  const down = (e) => {
    if (disabled) return
    const cell = cellAt(e)
    if (!cell) return
    // Second tap of a tap-tap selection
    if (anchor && !dragging.current && (anchor[0] !== cell[0] || anchor[1] !== cell[1])) {
      tryCommit(anchor, cell)
      setAnchor(null); setHover(null)
      return
    }
    dragging.current = true
    setAnchor(cell); setHover(cell)
  }
  const move = (e) => { if (dragging.current) { const cell = cellAt(e); if (cell) setHover(cell) } }
  const up = (e) => {
    if (!dragging.current) return
    dragging.current = false
    const cell = cellAt(e)
    if (cell && anchor && (cell[0] !== anchor[0] || cell[1] !== anchor[1])) {
      tryCommit(anchor, cell)
      setAnchor(null); setHover(null)
    }
    // A press and release on one cell keeps it as the first tap of a tap-tap selection.
  }

  return (
    <div className="text-center">
      <p className="text-secondary small mb-2">Drag from the first letter of a word to its last letter, or tap the first and then the last letter.</p>
      <ul className="list-inline mb-3" aria-label="Words to find">
        {words.map((w) => (
          <li key={w} className={`list-inline-item badge rounded-pill fs-6 fw-semibold mb-1 ws-word ${found.some((f) => f.word === w) ? 'found text-bg-light' : 'text-bg-dark'}`}>{w}</li>
        ))}
      </ul>
      <div className="ws-grid" style={{ '--n': size }} onPointerDown={down} onPointerMove={move} onPointerUp={up} onPointerLeave={() => { dragging.current = false }}
        role="grid" aria-label="Letter grid">
        {grid.map((row, r) => row.split('').map((ch, c) => {
          const key = `${r}-${c}`
          return <div key={key} data-cell={key} role="gridcell" className={`ws-cell ${foundCells.has(key) ? 'found' : ''} ${selCells.has(key) ? 'sel' : ''}`}>{ch}</div>
        }))}
      </div>
      <div className="small text-secondary mt-2" aria-live="polite">{found.length} of {words.length} found</div>
    </div>
  )
}

// ----------------------------------------------------------------------------- Word Challenge

export function WordChallengeBoard({ puzzle, onChange, disabled }) {
  const [answer, setAnswer] = useState('')
  useEffect(() => { onChange({ answer: answer.replace(/ /g, '') }) }, [answer, onChange])
  return (
    <div className="text-center">
      {puzzle.topic && <span className="badge text-bg-light border mb-2">{puzzle.topic}</span>}
      <p className="fs-5 fw-semibold mb-3">{puzzle.question}</p>
      <LetterInputs length={puzzle.length} highlight={-1} value={answer} onChange={setAnswer} disabled={disabled} label="Your answer" />
      <div className="small text-secondary mt-2">{puzzle.length} letters</div>
    </div>
  )
}

export const BOARDS = {
  game24: Game24Board,
  kenken: KenKenBoard,
  futoshiki: FutoshikiBoard,
  scramble: ScrambleBoard,
  puzzle: WordSearchBoard,
  word: WordChallengeBoard,
}
