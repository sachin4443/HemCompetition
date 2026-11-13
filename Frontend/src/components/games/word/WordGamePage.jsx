import { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../../services/api";

export default function WordGamePage() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [loadStatus, setLoadStatus] = useState(
    "Preparing word game for you..."
  );

  // Game Data States
  const [wordList, setWordList] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(0);
  const [currentWordItem, setCurrentWordItem] = useState(null);
  const [userInput, setUserInput] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  // In-page Notification Messages
  const [errorMessage, setErrorMessage] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  // Stopwatch Timer
  const startTimeRef = useRef(null);
  const [secondsElapsed, setSecondsElapsed] = useState(0);

  // Prevent old timeout from affecting next puzzle
  const messageTimeoutRef = useRef(null);

  // -------------------------------------------------------
  // Cleanup timeout on unmount
  // -------------------------------------------------------
  useEffect(() => {
    return () => {
      if (messageTimeoutRef.current) {
        clearTimeout(messageTimeoutRef.current);
      }
    };
  }, []);

  // -------------------------------------------------------
  // Live Stopwatch Timer
  // -------------------------------------------------------
  useEffect(() => {
    let timer;

    if (!loading && currentWordItem) {
      timer = setInterval(() => {
        setSecondsElapsed((prev) => prev + 1);
      }, 1000);
    }

    return () => clearInterval(timer);
  }, [loading, currentWordItem]);

  // -------------------------------------------------------
  // Timer Formatter
  // -------------------------------------------------------
  const formatTimer = (totalSeconds) => {
    const mins = Math.floor(totalSeconds / 60)
      .toString()
      .padStart(2, "0");

    const secs = (totalSeconds % 60)
      .toString()
      .padStart(2, "0");

    return `${mins}:${secs}`;
  };

  // -------------------------------------------------------
  // Fetch Initial Game Data
  // -------------------------------------------------------
  useEffect(() => {
    const token = localStorage.getItem("jwt_token");

    if (!token) {
      navigate("/", { replace: true });
      return;
    }

    api
      .get("/competition/wordgame-init")
      .then((res) => {
        const response = res.data;

        if (
          response.success &&
          response.words &&
          response.words.length > 0
        ) {
          startTimeRef.current = new Date();

          setWordList(response.words);
          setCurrentWordItem(response.words[0]);
          setCurrentIndex(0);
          setSecondsElapsed(0);
          setLoading(false);
        } else {
          setLoading(false);

          setErrorMessage(
            response.message ||
              "You have already completed the word games!"
          );

          messageTimeoutRef.current = setTimeout(() => {
            navigate("/word-dashboard");
          }, 1800);
        }
      })
      .catch((err) => {
        console.error("Word game load error:", err);

        setLoading(false);
        setLoadStatus("Failed to load word game session.");

        setErrorMessage(
          "Failed to load word game session. Please refresh."
        );
      });
  }, [navigate]);

  // -------------------------------------------------------
  // Handle Character Button Click
  // -------------------------------------------------------
  const handleCharClick = (char) => {
    if (isSubmitting) return;

    setErrorMessage("");
    setSuccessMessage("");

    setUserInput((prev) => prev + char);
  };

  // -------------------------------------------------------
  // Clear Input
  // -------------------------------------------------------
  const handleClear = () => {
    if (isSubmitting) return;

    setUserInput("");
    setErrorMessage("");
    setSuccessMessage("");
  };

  // -------------------------------------------------------
  // Check Answer
  // -------------------------------------------------------
  const handleCheck = () => {
    if (isSubmitting) return;

    if (!userInput) {
      setErrorMessage("⚠️ Please enter or select letters first.");
      return;
    }

    if (
      userInput.toUpperCase() ===
      currentWordItem?.wG_Answer?.toUpperCase()
    ) {
      saveWordGameData(userInput, null);
    } else {
      setErrorMessage("❌ Incorrect Answer! Try again.");
      setUserInput("");
    }
  };

  // -------------------------------------------------------
  // Save Word Game Data
  // -------------------------------------------------------
  const saveWordGameData = (answer, remarkValue) => {
    if (isSubmitting || !currentWordItem) return;

    setIsSubmitting(true);
    setErrorMessage("");
    setSuccessMessage("");

    const endTime = new Date();

    const startTime =
      startTimeRef.current || endTime;

    const timeDiffSeconds = Math.floor(
      (endTime - startTime) / 1000
    );

    const payload = {
      Game24ID: currentWordItem.wgid,
      GameTopicID: currentWordItem.wgTopicID,
      UserAnswer: answer,
      StartTime: startTime.toString(),
      EndTime: endTime.toString(),
      TimeDiff: timeDiffSeconds.toString(),
      Remark: remarkValue,
    };

    api
      .post("/competition/wordgame-save", payload)
      .then((res) => {
        setIsSubmitting(false);

        const responseData = res.data;

        console.log(
          "Save API Response:",
          responseData
        );

        // ------------------------------------------------
        // Series Completed
        // ------------------------------------------------
        if (
          responseData[0] === "2" ||
          responseData.isCompleted === true ||
          responseData.status === "completed"
        ) {
          setSuccessMessage(
            remarkValue === "Skipped"
              ? "skipped Question! 🏆 Game Series Completed."
              : "Record  saved successfully! 🏆 Game Series Completed."
          );

          messageTimeoutRef.current = setTimeout(() => {
            navigate("/word-dashboard");
          }, 1500);

          return;
        }

        // ------------------------------------------------
        // Normal Successful Save
        // ------------------------------------------------
        const msgText =
          remarkValue === "Skipped"
            ? "Skipped Question!"
            : "Record saved successfully!";

        setSuccessMessage(msgText);
        setErrorMessage("");

        // Automatically load next word
        messageTimeoutRef.current = setTimeout(() => {
          loadNextItem();
        }, 1200);
      })
      .catch((err) => {
        setIsSubmitting(false);

        console.error("Save error:", err);

        setErrorMessage(
          "Error saving response. Please try again."
        );
      });
  };

  // -------------------------------------------------------
  // Load Next Word
  // -------------------------------------------------------
  const loadNextItem = () => {
    setErrorMessage("");
    setSuccessMessage("");

    const nextIdx = currentIndex + 1;

    // ------------------------------------------------
    // All words completed
    // ------------------------------------------------
    if (nextIdx >= wordList.length) {
      setSuccessMessage(
        "🎉 Congratulations! You Finished All Games."
      );

      messageTimeoutRef.current = setTimeout(() => {
        navigate("/word-dashboard");
      }, 1800);

      return;
    }

    // ------------------------------------------------
    // Load next word WITHOUT page reload
    // ------------------------------------------------
    setCurrentIndex(nextIdx);
    setCurrentWordItem(wordList[nextIdx]);

    setUserInput("");
    setErrorMessage("");
    setSuccessMessage("");

    setSecondsElapsed(0);

    // Start new timer
    startTimeRef.current = new Date();
  };

  // -------------------------------------------------------
  // DIRECT SKIP
  // No confirmation modal
  // -------------------------------------------------------
  const handleSkip = () => {
    if (isSubmitting || !currentWordItem) return;

    setErrorMessage("");
    setSuccessMessage("");

    // Directly save as skipped
    saveWordGameData("0", "Skipped");
  };

  // -------------------------------------------------------
  // Navigate Dashboard
  // -------------------------------------------------------
  const handleGoToDashboard = () => {
    navigate("/word-dashboard");
  };

  // -------------------------------------------------------
  // Loading Screen
  // -------------------------------------------------------
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

  // -------------------------------------------------------
  // Main UI
  // -------------------------------------------------------
  return (
    <div
      className="min-vh-100 d-flex align-items-center justify-content-center py-4 px-2 position-relative"
      style={{
        background:
          "radial-gradient(circle, #1a1c29 0%, #0d0e15 100%)",
        color: "#fff",
      }}
    >
      <div
        className="card p-4 shadow-lg text-center mx-auto rounded-4"
        style={{
          maxWidth: "720px",
          width: "100%",
          background: "rgba(255, 255, 255, 0.05)",
          backdropFilter: "blur(12px)",
          border:
            "1px solid rgba(255, 255, 255, 0.15)",
        }}
      >
        {/* Header Bar */}
        <div className="d-flex justify-content-between align-items-center mb-3">
          <button
            className="btn btn-outline-light btn-sm fw-bold px-3"
            onClick={handleGoToDashboard}
            disabled={isSubmitting}
          >
            ⬅️ Back
          </button>

          <span className="badge bg-danger fs-6 px-3 py-2 rounded-pill shadow-sm">
            
          </span>

          {/* Direct Skip */}
          <button
            className="btn btn-outline-warning btn-sm fw-bold px-3"
            onClick={handleSkip}
            disabled={isSubmitting}
          >
            {isSubmitting ? "Saving..." : "⏭️ Skip"}
          </button>
        </div>

        <h3 className="fw-bold mb-1 text-warning">
          Word Challenge
        </h3>

        <p className="text-secondary small mb-3">
          Word {currentIndex + 1} of {wordList.length}
        </p>

        {/* Success Alert */}
        {successMessage && (
          <div
            className="alert alert-success py-2 px-3 fw-bold mb-3 border-0 shadow-sm"
            role="alert"
          >
            {successMessage}
          </div>
        )}

        {/* Error Alert */}
        {errorMessage && (
          <div
            className="alert alert-danger py-2 px-3 fw-bold mb-3 border-0 shadow-sm"
            role="alert"
          >
            {errorMessage}
          </div>
        )}

        {/* Question / Clue */}
        {currentWordItem && (
          <div
            className="p-3 mb-3 rounded-3 text-start"
            style={{
              background:
                "rgba(255, 255, 255, 0.08)",
              border:
                "1px solid rgba(255, 255, 255, 0.1)",
            }}
          >
            <span className="badge bg-info text-dark mb-1">
              Clue / Hint
            </span>

            <h5 className="mb-0 text-white fw-bold">
              {currentWordItem.wG_Question}
            </h5>
          </div>
        )}

        {/* User Input */}
        <div className="mb-3">
          <div className="input-group mb-3">
            <input
              type="text"
              className="form-control form-control-lg text-center fs-4 fw-bold bg-dark text-warning border-secondary rounded-start"
              placeholder="YOUR ANSWER"
              value={userInput}
              readOnly
              style={{
                letterSpacing: "3px",
              }}
            />

            <button
              className="btn btn-danger px-3 fw-bold"
              type="button"
              onClick={handleClear}
              disabled={
                isSubmitting || !userInput
              }
            >
              Clear ✖
            </button>
          </div>

          {/* Dynamic Letter Keyboard */}
          <div
            className="d-flex flex-nowrap align-items-center gap-2 px-3 py-3 rounded-3 mb-3 shadow-sm"
            style={{
              background: "#ffffff",
              overflowX: "auto",
              overflowY: "hidden",
              border:
                "1px solid #cbd5e1",
            }}
          >
            {currentWordItem?.wG_TopicName &&
              currentWordItem.wG_TopicName
                .split("")
                .map((char, idx) => (
                  <button
                    key={idx}
                    type="button"
                    className="btn fw-bold shadow-sm p-0 d-flex justify-content-center align-items-center flex-shrink-0"
                    style={{
                      width: "38px",
                      height: "38px",
                      fontSize: "16px",
                      borderRadius: "6px",
                      background: "#f1f3f5",
                      color: "#212529",
                      border:
                        "2px solid #adb5bd",
                    }}
                    onClick={() =>
                      handleCharClick(
                        char.toUpperCase()
                      )
                    }
                    disabled={isSubmitting}
                  >
                    {char.toUpperCase()}
                  </button>
                ))}
          </div>
        </div>

        {/* Submit */}
        <div className="row g-2 pt-2 border-top border-secondary">
          <div className="col-12">
            <button
              className="btn btn-success btn-lg w-100 fw-bold shadow"
              onClick={handleCheck}
              disabled={isSubmitting}
            >
              {isSubmitting
                ? "Saving..."
                : "✔ Submit"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}