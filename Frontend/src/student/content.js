// Presentation text and artwork for the sections and games. Game content itself (puzzles,
// questions, answers) always comes from the server; nothing here is an answer.

export const SECTION_ART = {
  number: { image: '/Numbergame.png', blurb: 'Game 24, Futoshiki and KenKen' },
  quiz: { image: '/QuizGame.png', blurb: 'HEM Mega Quiz' },
  word: { image: '/WordGame.png', blurb: 'Scramble, Puzzle and Word Challenge' },
}

export const GAME_INFO = {
  game24: {
    image: '/game24.png',
    rules: [
      'Use the four numbers and the signs + − × ÷ so that the result is 24.',
      'Each of the four numbers must be used exactly once. Signs can be used any number of times.',
      'Use the brackets to control the order of the calculation.',
    ],
  },
  futoshiki: {
    image: '/phosukhy.png',
    rules: [
      'Fill every cell so that no number repeats in any row or column.',
      'Every < and > sign between two cells must be true.',
      'Numbers already shown are fixed.',
    ],
  },
  kenken: {
    image: '/kenken.png',
    rules: [
      'Fill every cell so that no number repeats in any row or column.',
      'Cells with the same colour form a cage. The numbers in a cage must produce the target shown using the sign shown (+ − × ÷).',
      'A single-cell cage simply shows its number.',
    ],
  },
  scramble: {
    image: '/Scrumble.jpeg',
    rules: [
      'Rearrange each row of jumbled letters into a correct word.',
      'Take the letters from the yellow boxes and rearrange them to form the final word.',
      'All rows and the final word must be right to move on.',
    ],
  },
  puzzle: {
    image: '/puzzle.png',
    rules: [
      'Find each word from the list in the letter grid.',
      'Drag (or tap the first and the last letter) from the start of a word to its end.',
      'When every word is found your answer is saved automatically.',
    ],
  },
  word: {
    image: '/Word.jpeg',
    rules: [
      'Read the clue and type the word it describes.',
      'The word has exactly as many letters as there are boxes.',
    ],
  },
}

export const QUIZ_IMAGES = ['/HS_Quiz_1.png', '/HS_Quiz_2.png', '/HS_Quiz_3.png', '/HS_Quiz_4.png', '/HS_Quiz_5.png']

export const BUTTON_RULES = [
  ['Submit', 'Checks your answer. A wrong answer can be corrected and submitted again.'],
  ['Next', 'Moves to the next question. You cannot come back to the one you leave.'],
  ['Skip', 'Leaves the game completely. You will not be able to rejoin or play it again.'],
  ['Back', 'Returns to the dashboard. Your progress is kept and you can continue later.'],
]

export const statusButton = (status) =>
  status === 'finished'
    ? { label: 'Completed', icon: 'bi-check-circle-fill', className: 'btn-outline-secondary', disabled: true }
    : status === 'in_progress'
      ? { label: 'Continue', icon: 'bi-play-circle-fill', className: 'btn-warning', disabled: false }
      : { label: 'Start', icon: 'bi-play-fill', className: 'btn-success', disabled: false }

/** Combined status of a section from its items. */
export const sectionStatus = (items) => {
  if (!items.length) return 'empty'
  if (items.every((i) => i.status === 'finished')) return 'finished'
  if (items.some((i) => i.status !== 'not_started')) return 'in_progress'
  return 'not_started'
}
