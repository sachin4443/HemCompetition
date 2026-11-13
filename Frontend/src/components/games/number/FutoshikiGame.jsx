import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../../services/api";

// Helper: Generates full standard date string: "Fri Jul 31 2026 14:26:59 GMT+0530 (India Standard Time)"
const getFullDateString = () => new Date().toString();

export default function FutoshikiGame() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [questions, setQuestions] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [selectedCell, setSelectedCell] = useState(null);
  const [grid, setGrid] = useState([
    [0, 0, 0, 0],
    [0, 0, 0, 0],
    [0, 0, 0, 0],
    [0, 0, 0, 0]
  ]);

  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [submitting, setSubmitting] = useState(false);

  // Full Standard Date String State Initialization
  const [startTime, setStartTime] = useState(getFullDateString());
  const [secondsElapsed, setSecondsElapsed] = useState(0);

  const userId = localStorage.getItem("userId");

  // Timer Effect
  useEffect(() => {
    const timer = setInterval(() => {
      setSecondsElapsed((prev) => prev + 1);
    }, 1000);

    return () => clearInterval(timer);
  }, [currentIndex]);

  const formatTimer = (totalSeconds) => {
    const mins = Math.floor(totalSeconds / 60).toString().padStart(2, '0');
    const secs = (totalSeconds % 60).toString().padStart(2, '0');
    return `${mins}:${secs}`;
  };

  const loadQuestionData = (qData) => {
    if (!qData) return;
    setGrid(
      qData.initialGrid || [
        [0, 0, 0, 0],
        [0, 0, 0, 0],
        [0, 0, 0, 0],
        [0, 0, 0, 0]
      ]
    );
    setSelectedCell(null);
    setErrorMessage("");
    setSuccessMessage("");
    setStartTime(getFullDateString()); // Dynamic full date per question
    setSecondsElapsed(0);
  };

  // Fetch Questions
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
        const res = await api.get(`/FutoshikiApi/GetQuestions/${userId}`);
        const data = res.data;

        if (!isMounted) return;

        if (data && data.success && data.data && data.data.length > 0) {
          setQuestions(data.data);
          loadQuestionData(data.data[0]);
        } else {
          setErrorMessage(data?.message || "Aapke liye koi pending game nahi hai.");
        }
      } catch (err) {
        console.error("Futoshiki fetch error:", err);
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

  const resetBoard = () => {
    if (currentQ) loadQuestionData(currentQ);
  };

  const handleNumberInput = (num) => {
    if (!selectedCell) return;
    const { r, c } = selectedCell;
    if (currentQ?.initialGrid?.[r]?.[c] !== 0) return;

    setErrorMessage("");
    const newGrid = grid.map((row, rIdx) =>
      row.map((val, cIdx) => (rIdx === r && cIdx === c ? num : val))
    );
    setGrid(newGrid);
  };

  const handleErase = () => {
    if (!selectedCell) return;
    const { r, c } = selectedCell;
    if (currentQ?.initialGrid?.[r]?.[c] !== 0) return;

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

  // Submit Handler
  const handleSubmit = async () => {
    if (submitting || !userId) return;

    for (let r = 0; r < 4; r++) {
      for (let c = 0; c < 4; c++) {
        if (grid[r][c] === 0) {
          setErrorMessage("⚠️ Fill all empty cells first!");
          return;
        }
      }
    }

    setSubmitting(true);
    setErrorMessage("");
    setSuccessMessage("");

    const fcid = currentQ?.fcid || currentQ?.FCID;
    const endTime = getFullDateString();

    try {
      const res = await api.post("/FutoshikiApi/SaveAnswer", {
        UserId: Number(userId),
        FCID: Number(fcid),
        UserAnswer: grid.flat().join(""),
        StartTime: startTime,
        EndTime: endTime,
        IsLastQuestion: currentIndex === questions.length - 1
      });

      const resData = res.data;

      if (resData && resData.success) {
        setSuccessMessage("Record Saved successfully!");
        setTimeout(() => {
          setSubmitting(false);
          processNextTurn();
        }, 1000);
      } else {
        setErrorMessage(resData?.message || "Incorrect Solution.");
        setSubmitting(false);
      }
    } catch (err) {
      console.error("Futoshiki submit error:", err);
      setErrorMessage("Error submitting answer.");
      setSubmitting(false);
    }
  };

  // Skip Handler (Passes StartTime and EndTime)
  const handleSkip = async () => {
    if (submitting || !userId) return;

    const fcid = currentQ?.fcid || currentQ?.FCID || 0;

    if (!fcid) {
      setErrorMessage("Invalid question.");
      return;
    }

    setSubmitting(true);
    setErrorMessage("");
    setSuccessMessage("");

    const endTime = getFullDateString();

    try {
      const res = await api.post("/FutoshikiApi/SkipQuestion", {
        UserId: Number(userId),
        FCID: Number(fcid),
        StartTime: startTime,
        EndTime: endTime
      });

      const resData = res.data;

      if (resData && resData.success) {
        setSuccessMessage("Question skipped!");
        setTimeout(() => {
          setSubmitting(false);
          processNextTurn();
        }, 500);
      } else {
        setErrorMessage(resData?.message || "Skip failed.");
        setSubmitting(false);
      }
    } catch (err) {
      console.error("Futoshiki skip error:", err);
      setErrorMessage("Network error.");
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100 bg-dark text-white">
        <div className="spinner-border text-primary me-2"></div>
        <span className="fs-5 fw-bold">Loading Futoshiki Game...</span>
      </div>
    );
  }

  if (!currentQ) {
    return (
      <div className="text-center p-5 fs-5 text-white bg-dark vh-100 d-flex flex-column justify-content-center align-items-center">
        <h3 className="mb-3">{errorMessage || "No Game Questions Available"}</h3>
        <button className="btn btn-warning rounded-pill px-4 fw-bold" onClick={() => navigate("/dashboard")}>
          Return to Dashboard
        </button>
      </div>
    );
  }

  return (
    <div
      className="min-vh-100 d-flex flex-column align-items-center justify-content-center p-2"
      style={{
        background: "radial-gradient(circle, #1a1c29 0%, #0d0e15 100%)",
        color: "#fff"
      }}
    >
      <div
        className="card p-3 text-center rounded-4 shadow-lg"
        style={{
          maxWidth: "440px",
          width: "100%",
          background: "rgba(255, 255, 255, 0.05)",
          backdropFilter: "blur(12px)",
          border: "1px solid rgba(255, 255, 255, 0.15)"
        }}
      >
        <div className="d-flex justify-content-between align-items-center mb-3">
          <button
            className="btn btn-outline-light btn-sm fw-bold px-2 "
            onClick={() => navigate("/competition/1")}
          >
            ⬅️ Back
          </button>
{/*
          <span className="badge bg-primary fs-6 px-3 py-2 rounded-pill">
            Q {currentIndex + 1} / {questions.length}
          </span>
       
          <span className="badge bg-danger fs-6 px-3 py-2 rounded-pill shadow-sm">
            ⏱️ {formatTimer(secondsElapsed)}
          </span>
*/}
          <button
            className="btn btn-outline-warning btn-sm fw-bold px-2 	"
            onClick={handleSkip}
            disabled={submitting}
          >
            Skip ⏭️
          </button>
        </div>

        <h3 className="fw-bold text-warning mb-3">Futoshiki</h3>

        {/* Game Grid */}
        <div className="d-flex flex-column align-items-center mb-3">
          {[0, 1, 2, 3].map((r) => (
            <React.Fragment key={`row-${r}`}>
              <div className="d-flex align-items-center justify-content-center">
                {[0, 1, 2, 3].map((c) => {
                  const isFixed = currentQ.initialGrid?.[r]?.[c] !== 0;
                  const isSelected = selectedCell?.r === r && selectedCell?.c === c;

                  return (
                    <React.Fragment key={`cell-${r}-${c}`}>
                      <button
                        className="btn fw-bold rounded-2 d-flex justify-content-center align-items-center"
                        style={{
                          width: "56px",
                          height: "48px",
                          fontSize: "1.3rem",
                          border: isSelected ? "2px solid #0d6efd" : "1px solid rgba(255, 255, 255, 0.2)",
                          backgroundColor: isSelected ? "#3d5af1" : isFixed ? "#343a40" : "#212529",
                          color: isFixed ? "#ffc107" : "#ffffff"
                        }}
                        onClick={() => !isFixed && setSelectedCell({ r, c })}
                      >
                        {grid[r][c] !== 0 ? grid[r][c] : ""}
                      </button>

                      {c < 3 && (
                        <div className="fw-bold fs-4 text-warning d-flex justify-content-center align-items-center px-1" style={{ width: "24px", height: "48px" }}>
                          {currentQ.hSigns?.[r]?.[c] || ""}
                        </div>
                      )}
                    </React.Fragment>
                  );
                })}
              </div>

              {r < 3 && (
                <div className="d-flex justify-content-center align-items-center w-100 my-1">
                  {[0, 1, 2, 3].map((c) => (
                    <React.Fragment key={`vsign-${r}-${c}`}>
                      <div className="fw-bold fs-5 text-warning d-flex justify-content-center align-items-center" style={{ width: "56px", height: "20px" }}>
                        {currentQ.vSigns?.[r]?.[c] || ""}
                      </div>
                      {c < 3 && <div style={{ width: "24px" }}></div>}
                    </React.Fragment>
                  ))}
                </div>
              )}
            </React.Fragment>
          ))}
        </div>

        {/* Notifications */}
        {successMessage && (
          <div className="alert alert-success py-2 small fw-bold mb-3">
            {successMessage}
          </div>
        )}

        {errorMessage && (
          <div className="alert alert-danger py-2 small fw-bold mb-3">
            {errorMessage}
          </div>
        )}

        {/* Keypad */}
        <div className="d-flex justify-content-center gap-2 mb-3">
          {[1, 2, 3, 4].map((num) => (
            <button
              key={num}
              className="btn btn-primary btn-lg fw-bold rounded-3 px-3 shadow-sm"
              onClick={() => handleNumberInput(num)}
            >
              {num}
            </button>
          ))}
        </div>

        {/* Footer Actions */}
        <div className="d-flex justify-content-center gap-2">
          <button className="btn btn-warning text-dark fw-bold btn-md px-3 rounded-3" onClick={resetBoard}>
           <i className="fa fa-refresh" aria-hidden="true"></i>
          </button>
          <button className="btn btn-danger fw-bold btn-md px-3 rounded-3" onClick={handleErase}>
          <i className="fa fa-eraser" aria-hidden="true"></i>
          </button>
          <button className="btn btn-success fw-bold btn-md px-4 rounded-3 shadow" onClick={handleSubmit} disabled={submitting}>
            {submitting ? "Checking..." : "✔ Submit"}
          </button>
        </div>
      </div>
    </div>
  );
}