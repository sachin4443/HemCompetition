export const GAME_CATEGORIES_DATA = {
  // Game ID 1: Number Game
  1: {
    mainTitle: "Number Games",
    subGames: [
      {
        subGameId: "game24",
        title: "Game 24",
        image: "/game24.png",
        status: "Start",
        rules: [
          "Use the 4 numbers shown to reach the target sum of 24.",
          "You can use basic mathematical operations (+, -, *, /).",
          "Each number must be used exactly once.",
          "Click 'Start Game' to solve the puzzle."
        ]
      },
      {
        subGameId: "futoshiki",
        title: "Futoshiki",
        image: "/phosukhy.png",
        status: "Start",
        rules: [
          "Futoshiki is a game consisting of greater than and less than signs.",
          "In 4x4 grid, numbers 1 to 4 are filled in such a way that they fulfill inequality signs.",
          "Numbers 1, 2, 3 and 4 should not repeat in any row or column.",
          "The hint for filling numbers comes from '<' or '>' signs provided.",
          "Click 'Start Game' to solve the activity."
        ]
      },
      {
        subGameId: "kenken",
        title: "KenKen",
        image: "/kenken.png",
        status: "Start",
        rules: [
          "Fill each grid square with numbers from 1 to N without repeating in rows/columns.",
          "Cages display an target number and operation (+, -, *, /).",
          "Click 'Start Game' to begin solving."
        ]
      }
    ]
  },

  // Game ID 2: Quiz Game
  2: {
    mainTitle: "Quiz",
    subGames: [
      {
        subGameId: "quiz1",
        title: "Quiz Round 1",
        image: "/QuizGame.png",
        status: "Start",
        rules: [
          "Select the correct answer for each multiple choice question.",
          "Each correct question awards 10 points.",
          "Click 'Start Game' to proceed."
        ]
      },
      {
        subGameId: "quiz2",
        title: "Quiz Round 2",
        image: "/QuizGame.png",
        status: "Start",
        rules: [
          "Read questions carefully within the time limit.",
          "Click 'Start Game' when you are ready."
        ]
      },
      {
        subGameId: "quiz3",
        title: "Quiz Round 3",
        image: "/QuizGame.png",
        status: "Start",
        rules: ["Advanced timed Quiz round."]
      },
      {
        subGameId: "quiz4",
        title: "Quiz Round 4",
        image: "/QuizGame.png",
        status: "Start",
        rules: ["Final level quiz challenges."]
      }
    ]
  },

  // Game ID 3: Word Game
  3: {
    mainTitle: "Word Games",
    subGames: [
      {
        subGameId: "puzzle",
        title: "Word Puzzle",
        image: "/WordGame.png",
        status: "Start",
        rules: [
          "Find hidden words inside the puzzle grid.",
          "Drag or tap letters to form valid vocabulary words."
        ]
      },
      {
        subGameId: "scramble",
        title: "Word Scramble",
        image: "/WordGame.png",
        status: "Start",
        rules: [
          "Unscramble the jumbled letters to find the hidden target word."
        ]
      },
      {
        subGameId: "wordgame",
        title: "Word Game Arena",
        image: "/WordGame.png",
        status: "Start",
        rules: [
          "Fill missing letters to complete sentence challenges."
        ]
      }
    ]
  }
};