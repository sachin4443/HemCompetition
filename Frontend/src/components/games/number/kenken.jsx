import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../../services/api";

const getFullDateString = () => new Date().toString();

// Soft colors compatible with dark theme background
const CAGE_COLORS = {
  A: "rgba(187, 222, 251, 0.9)", // Soft Light Blue
  B: "rgba(220, 237, 200, 0.9)", // Soft Light Green
  C: "rgba(255, 224, 178, 0.9)", // Soft Light Orange
  D: "rgba(248, 187, 208, 0.9)", // Soft Light Pink
  E: "rgba(225, 190, 231, 0.9)"  // Soft Light Purple
};

export default function Kenken() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [questions, setQuestions] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [selectedCell, setSelectedCell] = useState(null);
  const [grid, setGrid] = useState([
    [0, 0, 0],
    [0, 0, 0],
    [0, 0, 0]
  ]);

  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const [startTime, setStartTime] = useState(getFullDateString());
  const [secondsElapsed, setSecondsElapsed] = useState(0);

  const userId = localStorage.getItem("userId");

  // Live Timer Interval
  useEffect(() => {
    const timer = setInterval(() => {
      setSecondsElapsed((prev) => prev + 1);
    }, 1000);
    return () => clearInterval(timer);
  }, [currentIndex]);

  const formatTimer = (totalSeconds) => {
    const mins = Math.floor(totalSeconds / 60).toString().padStart(2, "0");
    const secs = (totalSeconds % 60).toString().padStart(2, "0");
    return `${mins}:${secs}`;
  };

  const loadQuestionData = (qData) => {
    if (!qData) return;
    setGrid([
      [0, 0, 0],
      [0, 0, 0],
      [0, 0, 0]
    ]);
    setSelectedCell(null);
    setErrorMessage("");
    setSuccessMessage("");
    setStartTime(getFullDateString());
    setSecondsElapsed(0);
  };

  useEffect(() => {
    let isMounted = true;
    const fetchQuestions = async () => {
      if (!userId) {
        if (isMounted) {
          setErrorMessage("User session not found. Please log in again.");
          setLoading(false);
        }
        return;
      }

      setLoading(true);
      try {
        const res = await api.get(`/KenkenApi/GetQuestions/${userId}`);
        const data = res.data;

        if (!isMounted) return;

        if (data && data.success && data.data && data.data.length > 0) {
          setQuestions(data.data);
          loadQuestionData(data.data[0]);
        } else {
          setErrorMessage("No pending KenKen game available.");
        }
      } catch {
        if (isMounted) setErrorMessage("Server Connection Failed.");
      } finally {
        if (isMounted) setLoading(false);
      }
    };

    fetchQuestions();
    return () => {
      isMounted = false;
    };
  }, [userId]);

  const currentQ = questions[currentIndex];

  const handleNumberInput = (num) => {
    if (!selectedCell) return;
    const { r, c } = selectedCell;
    setErrorMessage("");

    const newGrid = grid.map((row, rIdx) =>
      row.map((val, cIdx) => (rIdx === r && cIdx === c ? num : val))
    );
    setGrid(newGrid);
  };

  const handleErase = () => {
    if (!selectedCell) return;
    const { r, c } = selectedCell;
    setErrorMessage("");
    const newGrid = grid.map((row, rIdx) =>
      row.map((val, cIdx) => (rIdx === r && cIdx === c ? 0 : val))
    );
    setGrid(newGrid);
  };

  const processNextTurn = () => {
    if (currentIndex + 1 < questions.length) {
      const nextIdx = currentIndex + 1;
      setCurrentIndex(nextIdx);
      loadQuestionData(questions[nextIdx]);
    } else {
      navigate("/competition/1");
    }
  };

  const handleSubmit = async () => {
    if (submitting || !userId) return;

    for (let r = 0; r < 3; r++) {
      for (let c = 0; c < 3; c++) {
        if (grid[r][c] === 0) {
          setErrorMessage("⚠️ Fill all empty cells first!");
          return;
        }
      }
    }

    setSubmitting(true);
    setErrorMessage("");
    setSuccessMessage("");

    const kcmid = currentQ?.kcmid || currentQ?.KCMID;
    const endTime = getFullDateString();

    try {
      const res = await api.post("/KenkenApi/SaveAnswer", {
        UserId: Number(userId),
        KCMID: Number(kcmid),
        UserAnswer: grid.flat().join(""),
        StartTime: startTime,
        EndTime: endTime,
        IsLastQuestion: currentIndex === questions.length - 1
      });

      if (res.data && res.data.success) {
        setSuccessMessage("Record Saved successfully!");
        setTimeout(() => {
          setSubmitting(false);
          processNextTurn();
        }, 1000);
      } else {
        setErrorMessage(res.data?.message || "Incorrect Solution.");
        setSubmitting(false);
      }
    } catch {
      setErrorMessage("Error submitting answer.");
      setSubmitting(false);
    }
  };

  const handleSkip = async () => {
    if (submitting || !userId) return;

    const kcmid = currentQ?.kcmid || currentQ?.KCMID;
    setSubmitting(true);
    const endTime = getFullDateString();

    try {
      const res = await api.post("/KenkenApi/SkipQuestion", {
        UserId: Number(userId),
        KCMID: Number(kcmid),
        StartTime: startTime,
        EndTime: endTime
      });

      if (res.data && res.data.success) {
        setSuccessMessage("Question skipped!");
        setTimeout(() => {
          setSubmitting(false);
          processNextTurn();
        }, 500);
      } else {
        setErrorMessage("Skip failed.");
        setSubmitting(false);
      }
    } catch {
      setErrorMessage("Network error.");
      setSubmitting(false);
    }
  };

  const handleBackToCompetition = () => {
    navigate("/competition/1");
  };

  const handleGoToDashboard = () => {
    navigate("/dashboard");
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100 bg-dark text-white">
        <div className="spinner-border text-warning me-2" role="status"></div>
        <span className="fs-5 fw-bold">Loading KenKen Game...</span>
      </div>
    );
  }

  if (!currentQ) {
    return (
      <div className="d-flex flex-column justify-content-center align-items-center vh-100 bg-dark text-white">
        <div className="fs-5 mb-3">{errorMessage || "No Game Questions Available"}</div>
        <button className="btn btn-warning rounded-pill px-4 fw-bold" onClick={handleGoToDashboard}>
          Return to Dashboard
        </button>
      </div>
    );
  }

  return (
    <div
      className="min-vh-100 d-flex align-items-center justify-content-center py-4"
      style={{
        background: "radial-gradient(circle, #1a1c29 0%, #0d0e15 100%)",
        color: "#fff"
      }}
    >
      <div
        className="card p-4 shadow-lg text-center mx-auto rounded-4"
        style={{
          maxWidth: "480px",
          width: "100%",
          background: "rgba(255, 255, 255, 0.05)",
          backdropFilter: "blur(12px)",
          border: "1px solid rgba(255, 255, 255, 0.15)"
        }}
      >
        {/* Navigation Header Bar */}
        <div className="d-flex justify-content-between align-items-center mb-3">
          <button
            className="btn btn-outline-light btn-sm fw-bold px-2"
            onClick={handleBackToCompetition}
          >
            ⬅️ Back
          </button>
 {/*
          <span className="badge bg-primary fs-6 px-3 py-2 rounded-pill">
            Q {currentIndex + 1} / {questions.length}
          </span>

          <span className="badge bg-danger fs-6 px-3 py-2 rounded-pill shadow-sm">
            ⏱️ {formatTimer(secondsElapsed)}
          </span>*/}

          <button
            className="btn btn-outline-warning btn-sm fw-bold px-2"
           onClick={handleSkip}
              disabled={submitting}
          >
             Skip ⏭️
          </button>
        </div>

        {/* Title */}
        <h3 className="fw-bold mb-1 text-warning">KenKen</h3>
        <p className="text-secondary small mb-3">Fill numbers 1-3 based on cages and targets</p>

        {/* 3x3 KenKen Grid */}
        <div className="d-inline-block border border-2 border-dark mb-3 mx-auto rounded-3 overflow-hidden" style={{ width: "288px", height: "288px" }}>
          {[0, 1, 2].map((r) => (
            <div key={`row-${r}`} className="d-flex">
              {[0, 1, 2].map((c) => {
                const cageKey = currentQ.cageGrid?.[r]?.[c] || "";
                const targetText = currentQ.targetGrid?.[r]?.[c] || "";
                const isSelected = selectedCell?.r === r && selectedCell?.c === c;
                const cellBgColor = CAGE_COLORS[cageKey] || "#ffffff";

                return (
                  <div
                    key={`cell-${r}-${c}`}
                    onClick={() => setSelectedCell({ r, c })}
                    className="position-relative d-flex justify-content-center align-items-center border border-dark"
                    style={{
                      width: "96px",
                      height: "96px",
                      backgroundColor: isSelected ? "#fff59d" : cellBgColor, // Highlight selected cell in soft yellow
                      cursor: "pointer",
                      boxShadow: isSelected ? "inset 0 0 0 3px #f57f17" : "none"
                    }}
                  >
                    {/* Target Text (Top-Left) */}
                    {targetText && (
                      <span className="position-absolute top-0 start-0 p-1 fw-bold fs-6 text-dark" style={{ lineHeight: "1" }}>
                        {targetText}
                      </span>
                    )}

                    {/* Cell Entered Number */}
                    <span className="fw-bold fs-2 text-dark">
                      {grid[r][c] !== 0 ? grid[r][c] : ""}
                    </span>
                  </div>
                );
              })}
            </div>
          ))}
        </div>

        {/* Success Alert */}
        {successMessage && (
          <div className="alert alert-success py-2 small fw-bold mb-3">
            {successMessage}
          </div>
        )}

        {/* Error Alert */}
        {errorMessage && (
          <div className="alert alert-danger py-2 small fw-bold mb-3">
            {errorMessage}
          </div>
        )}

        {/* Main Action Buttons (Submit & Skip) */}
        <div className="row g-2 mb-3">
          
         {/*  <div className="col-4">
            <button
              className="btn btn-outline-danger btn-lg w-100 fw-bold"
              onClick={handleSkip}
              disabled={submitting}
            >
              Skip ⏭️
            </button>
          </div> */}
        </div>

        {/* Input Keypad (1, 2, 3) */}
        <div className="row g-2 mb-2">
          {[1, 2, 3].map((num) => (
            <div key={num} className="col-4">
              <button
                className="btn btn-primary btn-lg w-100 fw-bold shadow-sm"
                onClick={() => handleNumberInput(num)}
              >
                {num}
              </button>
            </div>
          ))}
        </div>

        {/* Reset & Erase Controls */}
        <div className="row g-2">
          <div className="col-3">
            <button
              className="btn btn-warning text-dark fw-bold btn-lg w-100"
              onClick={() => loadQuestionData(currentQ)}
            >
             <i className="fa fa-refresh" aria-hidden="true"></i>
            </button>
          </div>
          <div className="col-3">
            <button
              className="btn btn-danger fw-bold btn-lg w-100"
              onClick={handleErase}
            >
             <i className="fa fa-eraser" aria-hidden="true"></i>
            </button>
          </div>
		  <div className="col-6">
            <button
              className="btn btn-success btn-lg w-100 fw-bold shadow"
              onClick={handleSubmit}
              disabled={submitting}
            >
              {submitting ? "Saving" : "✔ Submit"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}