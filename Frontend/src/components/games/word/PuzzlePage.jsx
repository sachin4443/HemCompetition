import { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../../services/api";

export default function PuzzlePage() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [loadStatus, setLoadStatus] = useState("Preparing puzzle for you...");

  // State Management
  const [allWords, setAllWords] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [isNextLoading, setIsNextLoading] = useState(false);

  // Custom Inline Alerts & Modal States
  const [showSkipModal, setShowSkipModal] = useState(false);
  const [showCompletedModal, setShowCompletedModal] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  // Stopwatch Timer Refs & States
  const startTimeRef = useRef(null);
  const [secondsElapsed, setSecondsElapsed] = useState(0);

  // Puzzle DOM Container Refs
  const puzzleContainerRef = useRef(null);
  const puzzleWordsRef = useRef(null);

  // Prevent duplicate next-puzzle execution
  const nextPuzzleTimerRef = useRef(null);

  // ---------------------------------------------------------
  // LOAD PUZZLE BATCH
  // ---------------------------------------------------------
  function loadPuzzleBatch(wordsList, startIndex) {
    const nextWords = wordsList
      .slice(startIndex, startIndex + 5)
      .map((w) => String(w).toUpperCase());

    if (nextWords.length === 0) {
      setShowCompletedModal(true);
      setIsNextLoading(false);
      return;
    }

    // Clear old puzzle immediately
    if (puzzleContainerRef.current) {
      puzzleContainerRef.current.innerHTML = "";
    }

    if (puzzleWordsRef.current) {
      puzzleWordsRef.current.innerHTML = "";
    }

    // Give DOM a moment to clear before creating next puzzle
    setTimeout(() => {
      if (
        window.wordfindgame &&
        puzzleContainerRef.current &&
        puzzleWordsRef.current
      ) {
        window.wordfindgame.create(
          nextWords,
          puzzleContainerRef.current,
          puzzleWordsRef.current,
          "COURSE_ID",
          "PRACTICE_ID"
        );
      }

      setIsNextLoading(false);
    }, 100);
  }

  // ---------------------------------------------------------
  // NEXT PUZZLE
  // ---------------------------------------------------------
  const handleNextBatch = () => {
    if (isNextLoading) return;

    setIsNextLoading(true);

    const newIndex = currentIndex + 5;

    // All puzzles completed
    if (newIndex >= allWords.length) {
      setIsNextLoading(false);
      setShowCompletedModal(true);
      return;
    }

    // Move to next batch
    setCurrentIndex(newIndex);

    // Reset timer
    setSecondsElapsed(0);

    startTimeRef.current = new Date();
    window.d = startTimeRef.current;

    // Load next puzzle without page reload
    loadPuzzleBatch(allWords, newIndex);
  };

  // ---------------------------------------------------------
  // PUZZLE SAVED EVENT
  // ---------------------------------------------------------
  // wordfindgame.js API save hone ke baad ye event fire karega.
  useEffect(() => {
    const handlePuzzleSaved = (event) => {
      console.log("Puzzle saved successfully:", event?.detail);

      // Show success message on current screen
      setErrorMessage("");
      setSuccessMessage("✅ Your answer saved successfully.");

      // Prevent duplicate timer
      if (nextPuzzleTimerRef.current) {
        clearTimeout(nextPuzzleTimerRef.current);
      }

      // Small delay so user can see success message,
      // then immediately load next puzzle without reload.
      nextPuzzleTimerRef.current = setTimeout(() => {
        setSuccessMessage("");

        handleNextBatch();
      }, 700);
    };

    const handlePuzzleSaveError = (event) => {
      console.error(
        "Puzzle save failed:",
        event?.detail
      );

      setSuccessMessage("");
      setErrorMessage(
        "Puzzle completed, but there was an issue saving your answer. Please try again."
      );

      setTimeout(() => {
        setErrorMessage("");
      }, 4000);
    };

    window.addEventListener(
      "wordfind:puzzle-saved",
      handlePuzzleSaved
    );

    window.addEventListener(
      "wordfind:puzzle-save-error",
      handlePuzzleSaveError
    );

    return () => {
      window.removeEventListener(
        "wordfind:puzzle-saved",
        handlePuzzleSaved
      );

      window.removeEventListener(
        "wordfind:puzzle-save-error",
        handlePuzzleSaveError
      );

      if (nextPuzzleTimerRef.current) {
        clearTimeout(nextPuzzleTimerRef.current);
      }
    };
  }, [allWords, currentIndex, isNextLoading]);

  // ---------------------------------------------------------
  // PREVENT MOBILE PAGE SCROLL WHILE DRAGGING GRID
  // ---------------------------------------------------------
  useEffect(() => {
    const container = puzzleContainerRef.current;

    if (!container) return;

    const preventScroll = (e) => {
      if (e.touches && e.touches.length === 1) {
        e.preventDefault();
      }
    };

    container.addEventListener(
      "touchstart",
      preventScroll,
      { passive: false }
    );

    container.addEventListener(
      "touchmove",
      preventScroll,
      { passive: false }
    );

    return () => {
      container.removeEventListener(
        "touchstart",
        preventScroll
      );

      container.removeEventListener(
        "touchmove",
        preventScroll
      );
    };
  }, [loading]);

  // ---------------------------------------------------------
  // LIVE STOPWATCH
  // ---------------------------------------------------------
  useEffect(() => {
    let timer;

    if (!loading && allWords.length > 0) {
      timer = setInterval(() => {
        setSecondsElapsed((prev) => prev + 1);
      }, 1000);
    }

    return () => {
      if (timer) {
        clearInterval(timer);
      }
    };
  }, [loading, allWords]);

  // ---------------------------------------------------------
  // TIMER FORMATTER
  // ---------------------------------------------------------
  const formatTimer = (totalSeconds) => {
    const mins = Math.floor(totalSeconds / 60)
      .toString()
      .padStart(2, "0");

    const secs = (totalSeconds % 60)
      .toString()
      .padStart(2, "0");

    return `${mins}:${secs}`;
  };

  // ---------------------------------------------------------
  // INITIAL PUZZLE LOAD
  // ---------------------------------------------------------
  useEffect(() => {
    const token = localStorage.getItem("jwt_token");

    if (!token) {
      navigate("/", { replace: true });
      return;
    }

    api
      .get("/competition/puzzle-init")
      .then((res) => {
        const response = res.data;

        if (
          response.success &&
          response.words &&
          response.words.length > 0
        ) {
          startTimeRef.current = new Date();

          // Existing wordfindgame.js compatibility
          window.d = startTimeRef.current;

          setAllWords(response.words);
          setCurrentIndex(0);
          setSecondsElapsed(0);
          setLoading(false);

          loadPuzzleBatch(response.words, 0);
        } else {
          setLoading(false);
          setShowCompletedModal(true);
        }
      })
      .catch((err) => {
        console.error("Puzzle load error:", err);

        setLoading(false);

        setLoadStatus(
          "Failed to load puzzle session."
        );

        setErrorMessage(
          "Failed to load puzzle session. Please refresh."
        );
      });
  }, [navigate]);

  // ---------------------------------------------------------
  // OPEN SKIP MODAL
  // ---------------------------------------------------------
  const handleOpenSkipModal = () => {
    if (isNextLoading) return;

    setShowSkipModal(true);
  };

  // ---------------------------------------------------------
  // CONFIRM SKIP
  // ---------------------------------------------------------
  const handleConfirmSkip = () => {
    setShowSkipModal(false);

    const endTime = new Date();

    const startTime =
      startTimeRef.current || endTime;

    const timeDiffSeconds = Math.floor(
      (endTime - startTime) / 1000
    );

    api
      .post("/competition/skip-puzzle", {
        startTime: startTime.toString(),
        endTime: endTime.toString(),
        timeDiff: timeDiffSeconds.toString(),
        userAnswer: "",
        remark: null,
      })
      .then(() => {
        navigate("/word-dashboard");
      })
      .catch((err) => {
        console.error("Skip error:", err);

        setErrorMessage(
          "Failed to skip puzzle. Try again."
        );
      });
  };

  // ---------------------------------------------------------
  // DASHBOARD
  // ---------------------------------------------------------
  const handleGoToDashboard = () => {
    navigate("/word-dashboard");
  };

  // ---------------------------------------------------------
  // LOADING SCREEN
  // ---------------------------------------------------------
  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100 bg-dark text-white">
        <div
          className="spinner-border text-warning me-2"
          role="status"
        ></div>

        <span className="fs-5 fw-bold">
          {loadStatus}
        </span>
      </div>
    );
  }

  // ---------------------------------------------------------
  // PAGE
  // ---------------------------------------------------------
  return (
    <div
      className="min-vh-100 d-flex align-items-center justify-content-center py-2 py-md-4 px-2 position-relative"
      style={{
        background:
          "radial-gradient(circle, #1a1c29 0%, #0d0e15 100%)",
        color: "#fff",
        overflowX: "hidden",
      }}
    >
      {/* Universal Grid Styling */}
      <style>{`
        #puzzle-container {
          touch-action: none !important;
          -webkit-user-select: none;
          user-select: none;
          display: inline-block;
          margin: 0 auto;
        }

        #puzzle-container .puzzleSquare {
          height: clamp(28px, 6.8vw, 38px) !important;
          width: clamp(28px, 6.8vw, 38px) !important;
          font-size: clamp(14px, 3.8vw, 18px) !important;
          font-weight: bold !important;
          text-align: center !important;
          background-color: rgba(255, 255, 255, 0.08) !important;
          border: 1px solid rgba(255, 255, 255, 0.2) !important;
          margin: 1.5px !important;
          color: #f8fafc !important;
          border-radius: 6px !important;
          transition: background-color 0.15s ease;
          display: inline-flex !important;
          align-items: center !important;
          justify-content: center !important;
          line-height: 1 !important;
          box-sizing: border-box !important;
        }

        #puzzle-container div {
          display: flex !important;
          justify-content: center !important;
          margin: 0 !important;
          padding: 0 !important;
        }

        #puzzle-container .puzzleSquare.selected {
          background-color: #f59e0b !important;
          color: #000 !important;
        }

        #puzzle-container .puzzleSquare.found {
          background-color: #22c55e !important;
          color: #ffffff !important;
        }

        #puzzle-words {
          width: 100%;
        }

        #puzzle-words ul {
          padding-left: 0 !important;
          margin: 0 !important;
          list-style: none !important;
          display: flex !important;
          flex-wrap: wrap !important;
          justify-content: center !important;
          gap: 8px !important;
        }

        #puzzle-words li {
          padding: 5px 12px !important;
          margin: 0 !important;
          background: rgba(56, 189, 248, 0.15) !important;
          border: 1px solid rgba(56, 189, 248, 0.4) !important;
          border-radius: 20px !important;
          color: #38bdf8 !important;
          text-decoration: none !important;
          font-weight: 700 !important;
          font-size: clamp(12px, 3.2vw, 15px) !important;
          letter-spacing: 0.5px !important;
          transition: all 0.2s ease;
        }

        #puzzle-words .wordFound {
          text-decoration: line-through !important;
          color: #94a3b8 !important;
          background: rgba(255, 255, 255, 0.05) !important;
          border-color: rgba(255, 255, 255, 0.1) !important;
          opacity: 0.5;
        }

        .puzzle-save-success {
          animation: puzzleSuccessIn 0.25s ease-out;
        }

        @keyframes puzzleSuccessIn {
          from {
            opacity: 0;
            transform: translateY(-8px);
          }

          to {
            opacity: 1;
            transform: translateY(0);
          }
        }
      `}</style>

      <div
        className="card p-3 p-md-4 shadow-lg text-center mx-auto rounded-4 d-flex flex-column"
        style={{
          maxWidth: "850px",
          width: "100%",
          background: "rgba(255, 255, 255, 0.05)",
          backdropFilter: "blur(12px)",
          border:
            "1px solid rgba(255, 255, 255, 0.15)",
        }}
      >
        {/* Header Bar */}
        <div className="d-flex justify-content-between align-items-center mb-2 flex-wrap gap-2">

          <button
            className="btn btn-outline-light btn-sm fw-bold px-3 rounded-pill d-flex align-items-center gap-1"
            onClick={handleGoToDashboard}
          >
            <i className="bi bi-arrow-left"></i>
            Back
          </button>

          

          <button
            className="btn btn-primary btn-sm fw-bold px-3 rounded-pill d-flex align-items-center gap-1 shadow-sm"
            onClick={handleNextBatch}
            disabled={isNextLoading}
          >
            {isNextLoading
              ? "Loading..."
              : "Next Puzzle ➡️"}
          </button>
        </div>

        <h4 className="fw-bold mb-1 text-warning fs-5 fs-md-4">
          Word Search Puzzle
        </h4>

        <p className="text-secondary small mb-2">
          Drag across letters to find target words
        </p>

        {/* SUCCESS MESSAGE */}
        {successMessage && (
          <div
            className="alert alert-success py-2 px-3 small fw-bold mb-2 border-0 shadow-sm puzzle-save-success"
            role="alert"
          >
            <i className="bi bi-check-circle-fill me-2"></i>
            {successMessage}
          </div>
        )}

        {/* ERROR MESSAGE */}
        {errorMessage && (
          <div
            className="alert alert-danger py-2 px-3 small fw-bold mb-2 border-0 shadow-sm"
            role="alert"
          >
            <i className="bi bi-exclamation-circle-fill me-2"></i>
            {errorMessage}
          </div>
        )}

        {/* WORD CHIPS */}
        <div className="mb-3 px-1">
          <div
            ref={puzzleWordsRef}
            id="puzzle-words"
            className="p-2 rounded-3"
            style={{
              background: "rgba(0, 0, 0, 0.25)",
            }}
          ></div>
        </div>

        {/* PUZZLE GRID */}
        <div className="d-flex justify-content-center align-items-center mb-3">
          <div
            ref={puzzleContainerRef}
            id="puzzle-container"
            className="p-1 p-md-2 rounded-3"
            style={{
              background: "rgba(0, 0, 0, 0.3)",
            }}
          ></div>
        </div>

        {/* SKIP BUTTON */}
        <div className="pt-2 border-top border-secondary mt-1">
          <button
            className="btn btn-outline-danger px-4 py-2 fw-bold shadow rounded-pill"
            onClick={handleOpenSkipModal}
            disabled={isNextLoading}
          >
            <i className="bi bi-skip-forward-fill me-1"></i>
            Skip out all puzzle games
          </button>
        </div>
      </div>

      {/* SKIP CONFIRMATION MODAL */}
      {showSkipModal && (
        <div
          className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center px-3"
          style={{
            backgroundColor: "rgba(0, 0, 0, 0.8)",
            zIndex: 1050,
          }}
        >
          <div
            className="card p-4 rounded-4 shadow-lg text-center bg-dark text-white border-warning"
            style={{
              maxWidth: "420px",
              width: "100%",
            }}
          >
            <div className="mb-2 text-warning fs-1">
              <i className="bi bi-exclamation-triangle-fill"></i>
            </div>

            <h5 className="fw-bold text-warning mb-2">
              Confirm Skip
            </h5>

            <p className="fw-semibold text-light mb-1">
              Are you sure all puzzle games will be skipped?
            </p>

            <p className="text-secondary small mb-4">
              
            </p>

            <div className="d-flex gap-2 justify-content-center">
              <button
                className="btn btn-outline-light rounded-pill px-4 py-2 fw-semibold"
                onClick={() => setShowSkipModal(false)}
              >
                Cancel
              </button>

              <button
                className="btn btn-danger rounded-pill px-4 py-2 fw-bold shadow"
                onClick={handleConfirmSkip}
              >
                Yes, Skip All
              </button>
            </div>
          </div>
        </div>
      )}

      {/* COMPLETION MODAL */}
      {showCompletedModal && (
        <div
          className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center px-3"
          style={{
            backgroundColor: "rgba(0, 0, 0, 0.85)",
            zIndex: 1050,
          }}
        >
          <div
            className="card p-4 rounded-4 shadow-lg text-center bg-dark text-white border-success"
            style={{
              maxWidth: "420px",
              width: "100%",
            }}
          >
            <div className="mb-2 text-success fs-1">
              <i className="bi bi-trophy-fill"></i>
            </div>

            <h4 className="fw-bold text-success mb-2">
              🎉 Congratulations! You Finished All Games.
            </h4>

            <p className="text-light mb-1">
              You have completed all the puzzles!
            </p>

            <button
              className="btn btn-success rounded-pill px-5 py-2 fw-bold shadow"
              onClick={handleGoToDashboard}
            >
              Go to Dashboard
            </button>
          </div>
        </div>
      )}
    </div>
  );
}