import { useState, useEffect, useRef, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../../services/api";

export default function ScramblePage() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [loadStatus, setLoadStatus] = useState("Preparing scramble puzzle...");
  const [questionData, setQuestionData] = useState(null);
  const [scrambleResults, setScrambleResults] = useState([]);
  const [finalAnswer, setFinalAnswer] = useState("");
  const [submitting, setSubmitting] = useState(false);

  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const startTimeRef = useRef(null);
  const [secondsElapsed, setSecondsElapsed] = useState(0);

  // 1. Live Stopwatch Timer Interval Effect
  useEffect(() => {
    let timer;
    if (!loading && questionData) {
      timer = setInterval(() => {
        setSecondsElapsed((prev) => prev + 1);
      }, 1000);
    }
    return () => clearInterval(timer);
  }, [loading, questionData]);

  // Format Timer into MM:SS
  const formatTimer = (totalSeconds) => {
    const mins = Math.floor(totalSeconds / 60).toString().padStart(2, "0");
    const secs = (totalSeconds % 60).toString().padStart(2, "0");
    return `${mins}:${secs}`;
  };

  // Reusable fetch function to load next question data without page reload
  const fetchScrambleData = useCallback(() => {
    setLoading(true);
    api.get("/competition/scramble_init")
      .then((res) => {
        const response = res.data;
        if (response.success) {
          startTimeRef.current = new Date();
          setQuestionData(response);
          setScrambleResults(response.results || []);
          setFinalAnswer(""); // Reset final input box
          setSecondsElapsed(0);
          setLoading(false);
        } else {
          setLoading(false);
          setErrorMessage(response.message || "You have completed all the Games!");
          setTimeout(() => {
            navigate("/word-dashboard");
          }, 2000);
        }
      })
      .catch((err) => {
        console.error("Scramble load error:", err);
        setLoading(false);
        setLoadStatus("Failed to load scramble session.");
        setErrorMessage("Failed to load scramble session. Please refresh.");
      });
  }, [navigate]);

  // 2. Initial Fetch on Mount
  useEffect(() => {
    const token = localStorage.getItem("jwt_token");
    if (!token) {
      navigate("/", { replace: true });
      return;
    }
    fetchScrambleData();
  }, [navigate, fetchScrambleData]);

  // Handle Box Input Change with Auto-Focus Next
  const handleInputChange = (rowIndex, charIndex, val, wordLen) => {
    setErrorMessage("");
    const cleanVal = val.replace(/[^a-zA-Z0-9]/, "").toUpperCase().slice(0, 1);

    const inputEl = document.getElementById(`answer-${rowIndex}-${charIndex}`);
    if (inputEl) inputEl.value = cleanVal;

    // Auto-focus next box
    if (cleanVal && charIndex < wordLen) {
      const nextEl = document.getElementById(`answer-${rowIndex}-${charIndex + 1}`);
      if (nextEl) nextEl.focus();
    }
  };

  // Handle Backspace Key Navigation
  const handleKeyDown = (rowIndex, charIndex, e) => {
    if (e.key === "Backspace") {
      const currentEl = document.getElementById(`answer-${rowIndex}-${charIndex}`);
      if (currentEl && !currentEl.value && charIndex > 1) {
        const prevEl = document.getElementById(`answer-${rowIndex}-${charIndex - 1}`);
        if (prevEl) prevEl.focus();
      }
    }
  };

  // Submit Logic with Local Validations
  // Submit Logic with Local Validations
  const handleSubmit = () => {
    // 🛑 Lock immediately on first click to prevent double firing
    if (submitting || !questionData) return;
    setSubmitting(true); 

    setErrorMessage("");
    setSuccessMessage("");

    // Validate all row inputs
    for (let j = 0; j < scrambleResults.length; j++) {
      const item = scrambleResults[j];
      const wordLen = item.UnscrambleWord.length;
      let rowInput = "";

      for (let i = 1; i <= wordLen; i++) {
        const el = document.getElementById(`answer-${j + 1}-${i}`);
        rowInput += el ? el.value : "";
      }

      if (rowInput.length !== wordLen) {
        setErrorMessage(`⚠️ Please complete the answer for word row ${j + 1}`);
        setSubmitting(false); // 🔓 Unlock if validation fails
        return;
      }
      if (rowInput.toUpperCase() !== item.UnscrambleWord.toUpperCase()) {
        setErrorMessage(`❌ Incorrect answer for word row ${j + 1}`);
        setSubmitting(false); // 🔓 Unlock if validation fails
        return;
      }
    }

    if (!finalAnswer.trim()) {
      setErrorMessage("⚠️ Final answer is required.");
      setSubmitting(false); // 🔓 Unlock if validation fails
      return;
    }

    if (finalAnswer.trim().toUpperCase() !== questionData.answer.toUpperCase()) {
      setErrorMessage("❌ The final answer is incorrect.");
      setSubmitting(false); // 🔓 Unlock if validation fails
      return;
    }

    sendSaveRequest(finalAnswer.trim().toUpperCase(), null);
  };

  // API Call to Save Record (Without Reloading Window)
  // API Call to Save Record (Without Reloading Window)
  // API Call to Save Record (Without Reloading Window)
  const sendSaveRequest = (answerText, remarkVal) => {
    if (submitting) return;
    setSubmitting(true);
    setErrorMessage("");
    setSuccessMessage("");

    const endTime = new Date();
    const startTime = startTimeRef.current || endTime;
    const timeDiffSeconds = Math.floor((endTime - startTime) / 1000);

    const payload = {
      Answer: answerText,
      QuestionId: questionData.questionId,
      TopicId: questionData.topicId,
      StartTime: startTime.toString(),
      EndTime: endTime.toString(),
      TimeDiff: timeDiffSeconds.toString(),
      Remark: remarkVal,
    };

    api.post("/competition/scramble_save", payload)
      .then((res) => {
        const resp = res.data;

        if (resp[0] === "1") {
          setSuccessMessage("Record Saved Successfully!");
          
          // 🔓 Unlock buttons immediately or right before fetching next data
          setSubmitting(false); 

          setTimeout(() => {
            setSuccessMessage("");
            fetchScrambleData(); 
          }, 1500);

        } else if (resp[0] === "2") {
          setSuccessMessage(resp[1] || "Con Completed!");
          setTimeout(() => {
            navigate("/word-dashboard");
          }, 1500);
        } else {
          setSubmitting(false);
          fetchScrambleData();
        }
      })
      .catch((err) => {
        setSubmitting(false);
        console.error("Save error:", err);
        setErrorMessage("Error saving answer. Try again.");
      });
  };

  // Skip Logic
  const handleSkip = () => {
    if (submitting || !questionData) return;
    sendSaveRequest("0", "Skipped");
  };

  // Navigation Handlers
  const handleGoToDashboard = () => {
    navigate("/word-dashboard");
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center vh-100 bg-dark text-white">
        <div className="spinner-border text-warning me-2" role="status"></div>
        <span className="fs-5 fw-bold">{loadStatus}</span>
      </div>
    );
  }

  return (
    <div
      className="min-vh-100 d-flex align-items-center justify-content-center py-2 py-md-4 px-2"
      style={{
        background: "radial-gradient(circle, #1a1c29 0%, #0d0e15 100%)",
        color: "#fff",
      }}
    >
      <div
        className="card p-3 p-md-4 shadow-lg text-center mx-auto rounded-4 d-flex flex-column"
        style={{
          maxWidth: "600px",
          width: "100%",
          maxHeight: "92vh",
          background: "rgba(255, 255, 255, 0.05)",
          backdropFilter: "blur(12px)",
          border: "1px solid rgba(255, 255, 255, 0.15)",
        }}
      >
        {/* Header Bar */}
        <div className="d-flex justify-content-between align-items-center mb-2 flex-shrink-0">
          <button
            className="btn btn-outline-light btn-sm fw-bold px-3 d-flex align-items-center gap-1"
            onClick={handleGoToDashboard}
          >
            ⬅️ Back
          </button>
         
          <button
            className="btn btn-outline-warning btn-sm fw-bold px-3 d-flex align-items-center gap-1"
            onClick={handleSkip}
            disabled={submitting}
          >
            Skip ⏭️
          </button>
        </div>

        <h4 className="fw-bold mb-0 text-warning">Scramble</h4>
        <p className="text-secondary small mb-2">Unscramble words to find the final answer</p>

        {/* Scrollable Main Content Area */}
        <div className="flex-grow-1 overflow-auto px-1 custom-scrollbar" style={{ maxHeight: "calc(92vh - 160px)" }}>
          {/* Question / Hint */}
          {questionData && (
            <div
              className="p-2 p-md-3 mb-2 rounded-3 text-start"
              style={{ background: "rgba(255, 255, 255, 0.08)", border: "1px solid rgba(255, 255, 255, 0.1)" }}
            >
              <h6 className="mb-0 text-white fw-bold leading-snug">{questionData.question}</h6>
            </div>
          )}

          {/* Success Alert */}
          {successMessage && (
            <div className="alert alert-success py-2 small fw-bold mb-2 animate__animated animate__fadeIn">
              {successMessage}
            </div>
          )}

          {/* Error Alert */}
          {errorMessage && (
            <div className="alert alert-danger py-2 small fw-bold mb-2">
              {errorMessage}
            </div>
          )}

          {/* Scramble Word Rows */}
          <div className="mb-2">
            {scrambleResults.map((item, jIndex) => {
              const rowId = jIndex + 1;
              const wordLen = item.UnscrambleWord.length;

              return (
                <div key={rowId} className="mb-2 p-2 rounded-3" style={{ background: "rgba(0, 0, 0, 0.2)" }}>
                  <div className="fw-bold fs-6 text-warning mb-1">
                    {item.ScrambleWord}
                  </div>
                  <div className="d-flex justify-content-center gap-1 gap-sm-2 flex-wrap">
                    {Array.from({ length: wordLen }, (_, iIndex) => {
                      const boxNum = iIndex + 1;
                      const isHighlighted =
                        item.FirstIndex === boxNum ||
                        item.SecondIndex === boxNum ||
                        item.ThirdIndex === boxNum ||
                        item.FourthIndex === boxNum;

                      return (
                        <input
                          key={boxNum}
                          type="text"
                          id={`answer-${rowId}-${boxNum}`}
                          maxLength="1"
                          className="text-uppercase text-center fw-bold shadow-sm"
                          style={{
                            width: "38px",
                            height: "40px",
                            fontSize: "18px",
                            backgroundColor: isHighlighted ? "#fef08a" : "#ffffff",
                            color: "#000000",
                            border: isHighlighted ? "2px solid #eab308" : "1px solid #94a3b8",
                            borderRadius: "6px",
                            outline: "none",
                          }}
                          onChange={(e) => handleInputChange(rowId, boxNum, e.target.value, wordLen)}
                          onKeyDown={(e) => handleKeyDown(rowId, boxNum, e)}
                        />
                      );
                    })}
                  </div>
                </div>
              );
            })}
          </div>

          {/* Final Answer Input */}
          <div className="mt-2 pt-2 border-top border-secondary">
            <input
              type="text"
              className="form-control text-center fs-5 fw-bold bg-light text-warning border-secondary rounded-3 mb-2"
              placeholder="ENTER FINAL ANSWER"
              value={finalAnswer}
              onChange={(e) => setFinalAnswer(e.target.value.toUpperCase())}
              style={{ letterSpacing: "2px" }}
            />

            {/* Action Buttons */}
            <div className="row g-2">
              <div className="col-12">
                <button
                  className="btn btn-success w-100 fw-bold shadow py-2"
                  onClick={handleSubmit}
                  disabled={submitting}
                >
                  {submitting ? "Saving Answer..." : "✔ Submit"}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}