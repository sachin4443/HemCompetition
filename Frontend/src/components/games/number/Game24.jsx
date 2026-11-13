import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../../services/api"; 

export default function Game24() {
  const navigate = useNavigate();

  const [questions, setQuestions] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [loading, setLoading] = useState(true);
  const [userInput, setUserInput] = useState("");
  const [history, setHistory] = useState([]);
  const [usedIndexes, setUsedIndexes] = useState([]);
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [submitting, setSubmitting] = useState(false);

  // Dynamic UserId from LocalStorage (set by Dashboard)
  const userId = localStorage.getItem("userId");

  // Question Start Time & Live Stopwatch Timer Tracking
  const [startTime, setStartTime] = useState(new Date().toISOString());
  const [secondsElapsed, setSecondsElapsed] = useState(0);

  // 1. Live Timer Interval Effect
  useEffect(() => {
    const timer = setInterval(() => {
      setSecondsElapsed((prev) => prev + 1);
    }, 1000);

    return () => clearInterval(timer);
  }, [currentIndex]);

  // Format Timer into MM:SS
  const formatTimer = (totalSeconds) => {
    const mins = Math.floor(totalSeconds / 60).toString().padStart(2, '0');
    const secs = (totalSeconds % 60).toString().padStart(2, '0');
    return `${mins}:${secs}`;
  };

  // 2. Fetch Questions via Axios API using Dynamic UserId
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

      try {
        setLoading(true);
        // Using axios instance with JWT token auto-attached
        const res = await api.get(`/Game24Api/GetQuestions/${userId}`);
        
        if (isMounted && res.data && res.data.success && res.data.data?.length > 0) {
          setQuestions(res.data.data);
          setStartTime(new Date().toISOString());
          setSecondsElapsed(0);
        } else if (isMounted) {
          setErrorMessage(res.data?.message || "No questions available.");
        }
      } catch (err) {
        console.error("Fetch questions error:", err);
        if (isMounted) setErrorMessage("Failed to load questions. Please refresh.");
      } finally {
        if (isMounted) setLoading(false);
      }
    };

    fetchQuestions();

    return () => {
      isMounted = false;
    };
  }, [userId]);

  const currentQuestion = questions[currentIndex];

  const resetBoard = () => {
    setUserInput("");
    setHistory([]);
    setUsedIndexes([]);
    setErrorMessage("");
  };

  const handleNumberClick = (val, index) => {
    if (usedIndexes.includes(index)) return;
    setErrorMessage("");
    setUserInput((prev) => prev + val);
    setUsedIndexes((prev) => [...prev, index]);
    setHistory((prev) => [...prev, { type: "number", value: val, index }]);
  };

  const handleOpClick = (symbol) => {
    setErrorMessage("");
    setUserInput((prev) => prev + symbol);
    setHistory((prev) => [...prev, { type: "operator", value: symbol }]);
  };

  const handleBackspace = () => {
    if (history.length === 0) return;
    setErrorMessage("");
    const last = history[history.length - 1];
    const newHistory = history.slice(0, -1);

    if (last.type === "number") {
      setUsedIndexes((prev) => prev.filter((i) => i !== last.index));
    }

    setHistory(newHistory);
    setUserInput(newHistory.map((item) => item.value).join(""));
  };

  // Safe Math Evaluator
  const evaluateExpression = (expr) => {
    try {
      if (/[^0-9+\-*/()\s]/.test(expr)) return null;
      const fn = new Function(`return (${expr})`);
      const val = fn();
      return isFinite(val) ? val : null;
    } catch {
      return null;
    }
  };

  // Validation Check
  const validateSolution = () => {
    if (usedIndexes.length < 4) {
      setErrorMessage("⚠️ All 4 numbers must be used!");
      return false;
    }

    const calcResult = evaluateExpression(userInput);

    if (calcResult === null) {
      setErrorMessage("❌ Invalid mathematical expression!");
      return false;
    }

    if (Math.abs(calcResult - 24) > 0.0001) {
      setErrorMessage("❌ Wrong Answer! Expression does not equal 24.");
      return false;
    }

    return true;
  };

  const processNextTurn = () => {
    if (currentIndex + 1 < questions.length) {
      setCurrentIndex((prev) => prev + 1);
      resetBoard();
      setStartTime(new Date().toISOString());
      setSecondsElapsed(0);
    } else {
      navigate("/competition/1");
    }
  };

  // Submit Action using `api` Axios Instance
  const handleSubmit = async () => {
    if (!userInput.trim() || submitting || !userId) return;

    if (!validateSolution()) return;

    setSubmitting(true);
    setErrorMessage("");
    setSuccessMessage("");

    const gcid = currentQuestion?.gcid ?? currentQuestion?.GCID;
    const endTime = new Date().toISOString();
    const isLastQuestion = currentIndex === questions.length - 1;

    try {
      const response = await api.post("/Game24Api/SaveAnswer", {
        UserId: Number(userId),
        GCID: Number(gcid),
        UserAnswer: userInput,
        StartTime: startTime,
        EndTime: endTime,
        IsLastQuestion: isLastQuestion
      });

      if (response.data && response.data.success) {
        setSuccessMessage(response.data.message || "Record saved successfully!");

        setTimeout(() => {
          setSuccessMessage("");
          processNextTurn();
        }, 1200);
      } else {
        setErrorMessage(response.data?.message || "Failed to save answer.");
      }
    } catch (err) {
      console.error("Save answer error:", err);
      setErrorMessage("Error saving answer. Try again.");
    } finally {
      setSubmitting(false);
    }
  };

  // Skip Action using `api` Axios Instance
  const handleSkip = async () => {
    if (submitting || !userId) return;

    setSubmitting(true);
    setErrorMessage("");
    setSuccessMessage("");

    const gcid = currentQuestion?.gcid ?? currentQuestion?.GCID;

    try {
      const res =await api.post("/Game24Api/SkipQuestion", {
        UserId: Number(userId),
        GCID: Number(gcid)
      });
	  if (res.data && res.data.success) {
        setSuccessMessage("Question skipped!");
        
      } else {
        setErrorMessage("Skip failed.");
        
      }
    } catch (err) {
      console.error("Skip error:", err);
    } finally {
      setSubmitting(false);
      processNextTurn();
    }
  };

  // Navigation Handlers
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
        <span className="fs-5 fw-bold">Loading Questions...</span>
      </div>
    );
  }

  if (!currentQuestion) {
    return (
      <div className="d-flex flex-column justify-content-center align-items-center vh-100 bg-dark text-white">
        <div className="fs-5 mb-3">{errorMessage || "No Game Questions Available"}</div>
        <button className="btn btn-warning rounded-pill px-4 fw-bold" onClick={handleGoToDashboard}>
          Return to Dashboard
        </button>
      </div>
    );
  }

  const numbers = [
    currentQuestion.number1 ?? currentQuestion.Number1,
    currentQuestion.number2 ?? currentQuestion.Number2,
    currentQuestion.number3 ?? currentQuestion.Number3,
    currentQuestion.number4 ?? currentQuestion.Number4,
  ];

  return (
    <div
      className="min-vh-100 d-flex align-items-center justify-content-center py-4"
      style={{
        background: "radial-gradient(circle, #1a1c29 0%, #0d0e15 100%)",
        color: "#fff",
      }}
    >
      <div
        className="card p-4 shadow-lg text-center mx-auto rounded-4"
        style={{
          maxWidth: "480px",
          width: "100%",
          background: "rgba(255, 255, 255, 0.05)",
          backdropFilter: "blur(12px)",
          border: "1px solid rgba(255, 255, 255, 0.15)",
        }}
      >
        {/* Header Bar */}
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
          </span>
 */}
          <button
            className="btn btn-outline-warning btn-sm fw-bold px-2 "
            onClick={handleSkip}
              disabled={submitting}
          >
            Skip⏭️
          </button>
        </div>

        <h3 className="fw-bold mb-1 text-warning">Game 24</h3>
        <p className="text-secondary small mb-3">Target: Make 24 using all numbers once</p>

        {/* Numbers Display */}
        <div className="row g-2 mb-3">
          {numbers.map((num, idx) => (
            <div key={idx} className="col-3">
              <div
                className={`p-3 fw-bold fs-3 rounded-3 ${
                  usedIndexes.includes(idx)
                    ? "bg-secondary text-white opacity-25 border border-dark"
                    : "bg-warning text-dark shadow-sm border border-warning"
                }`}
              >
                {num}
              </div>
            </div>
          ))}
        </div>

        {/* Input Field */}
        <input
          type="text"
          className="form-control form-control-lg text-center fs-3 fw-bold bg-dark text-warning border-secondary rounded-3 mb-2"
          value={userInput}
          placeholder="Enter Expression..."
          readOnly
        />

        {/* Success Alert */}
        {successMessage && (
          <div className="alert alert-success py-2 small fw-bold mb-3 animate__animated animate__fadeIn">
            {successMessage}
          </div>
        )}

        {/* Error Alert */}
        {errorMessage && (
          <div className="alert alert-danger py-2 small fw-bold mb-3">
            {errorMessage}
          </div>
        )}

        {/* Action Buttons */}
        <div className="row g-2 mb-3">
          <div className="col-12">
            <button
              className="btn btn-success btn-lg w-100 fw-bold shadow"
              onClick={handleSubmit}
              disabled={submitting}
            >
              {submitting ? "Saving Answer..." : "✔ Submit"}
            </button>
          </div>
          {/* <div className="col-4">
            <button
              className="btn btn-outline-danger btn-lg w-100 fw-bold"
              onClick={handleSkip}
              disabled={submitting}
            >
              Skip
            </button>
          </div>*/}
        </div>

        {/* Numbers Keypad */}
        <div className="row g-2 mb-2">
          {numbers.map((num, idx) => (
            <div key={idx} className="col-3">
              <button
                className={`btn btn-lg w-100 fw-bold ${
                  usedIndexes.includes(idx) ? "btn-secondary opacity-50" : "btn-primary shadow-sm"
                }`}
                disabled={usedIndexes.includes(idx)}
                onClick={() => handleNumberClick(num, idx)}
              >
                {num}
              </button>
            </div>
          ))}
        </div>

        {/* Operators Keypad */}
        <div className="row g-2 mb-2">
          {["+", "-", "*", "/"].map((op) => (
            <div key={op} className="col-3">
              <button
                className="btn btn-info text-white fw-bold btn-lg w-100 shadow-sm"
                onClick={() => handleOpClick(op)}
              >
                {op}
              </button>
            </div>
          ))}
        </div>

        {/* Controls Keypad */}
        <div className="row g-2">
          <div className="col-3">
            <button className="btn btn-secondary fw-bold btn-lg w-100" onClick={() => handleOpClick("(")}>(</button>
          </div>
          <div className="col-3">
            <button className="btn btn-secondary fw-bold btn-lg w-100" onClick={() => handleOpClick(")")}>)</button>
          </div>
          <div className="col-3">
            <button className="btn btn-warning text-dark fw-bold btn-lg w-100" onClick={resetBoard}> <i className="fa fa-refresh" aria-hidden="true"></i></button>
          </div>
          <div className="col-3">
            <button className="btn btn-danger fw-bold btn-lg w-100" onClick={handleBackspace}><i className="fa fa-eraser" aria-hidden="true"></i></button>
          </div>
        </div>
      </div>
    </div>
  );
}