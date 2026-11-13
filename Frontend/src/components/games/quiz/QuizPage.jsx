import { useEffect, useState, useRef, useCallback } from 'react';
import { useNavigate, useLocation, useParams } from 'react-router-dom';
import api from '../../../services/api';

function QuizPage() {
    const navigate = useNavigate();
    const location = useLocation();
    const { id } = useParams();

    const [loading, setLoading] = useState(true);
    const [loadStatus, setLoadStatus] = useState("Please Wait while we are preparing quiz for you");
    const [quizName, setQuizName] = useState("");
    const [questions, setQuestions] = useState([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [examId, setExamId] = useState(0);
    const [attemptedCount, setAttemptedCount] = useState(0);
    const [secondsElapsed, setSecondsElapsed] = useState(0); // Live Stopwatch
    const [isFinished, setIsFinished] = useState(false);
    const [showSummaryModal, setShowSummaryModal] = useState(false);
    
    const timerRef = useRef(null);

    const queryParams = new URLSearchParams(location.search);
    const quizId = id || queryParams.get('QuizId') || queryParams.get('quizId') || 332;
    const topicId = queryParams.get('TopicID') || queryParams.get('topicID') || 6256;
    const levelId = queryParams.get('LevelID') || queryParams.get('levelID') || 1;
    const languageId = queryParams.get('LanguageID') || queryParams.get('langId') || 1;
    const isInstituteQuiz = queryParams.get('IsInstituteQuiz') || "No";
    const isCertificate = queryParams.get('Iscertificate') || "False";

    // 1. Live Stopwatch Effect
    useEffect(() => {
        const stopwatch = setInterval(() => {
            setSecondsElapsed((prev) => prev + 1);
        }, 1000);
        return () => clearInterval(stopwatch);
    }, [currentIndex]);

    // Format Stopwatch MM:SS
    const formatTimer = (totalSeconds) => {
        const mins = Math.floor(totalSeconds / 60).toString().padStart(2, '0');
        const secs = (totalSeconds % 60).toString().padStart(2, '0');
        return `${mins}:${secs}`;
    };

    // 2. Fetch Questions Function
    const loadQuestionsData = useCallback(() => {
        api.get(`/competition/get-questions?id=${quizId}&levelID=${levelId}&topicID=${topicId}&iscertificate=${isCertificate}&langId=${languageId}`)
            .then(res => {
                const response = res.data;
                const isSuccess = response.Success !== undefined ? response.Success : response.success;
                const message = response.Message !== undefined ? response.Message : response.message;
                const data = response.Data !== undefined ? response.Data : response.data;

                if (isSuccess) {
                    setExamId(message);
                    setQuestions(data || []);
                    setLoading(false);
                } else {
                    setLoadStatus(message || "Failed to load questions.");
                }
            })
            .catch(err => {
                console.error("Questions load error:", err);
                setLoadStatus("Failed to fetch questions.");
            });
    }, [quizId, levelId, topicId, isCertificate, languageId]);

    // 3. Fetch Quiz Parameters and Questions with Safe Cleanup (Fixes ESLint Ref Warning)
    useEffect(() => {
        document.body.style.margin = '0';
        document.body.style.padding = '0';

        const token = localStorage.getItem('jwt_token');
        if (!token) {
            navigate('/', { replace: true });
            return;
        }

        api.get(`/competition/start-exam?quizId=${quizId}&topicID=${topicId}&levelID=${levelId}&isInstituteQuiz=${isInstituteQuiz}&iscertificate=${isCertificate}`)
            .then(res => {
                const resData = res.data;
                const responseObj = resData.Data || resData.data;
                const responseMsg = resData.Message || resData.message;

                if (responseObj) {
                    setQuizName(responseObj.TopicName || responseObj.topicName || "HEM MEGA QUIZ");
                    loadQuestionsData();
                } else {
                    setLoadStatus(responseMsg || "Error loading quiz parameters.");
                }
            })
            .catch(err => {
                console.error("Initialization error:", err);
                setLoadStatus("Server connection failed.");
            });

        // Safe Ref Cleanup for ESLint
        const currentTimer = timerRef.current;
        return () => {
            if (currentTimer) {
                clearInterval(currentTimer);
            }
        };
    }, [navigate, quizId, topicId, levelId, isInstituteQuiz, isCertificate, loadQuestionsData]);

    const handleSelectOption = (questionId, optionId) => {
        const updatedQuestions = [...questions];
        const currentQ = updatedQuestions[currentIndex];
        const optionsList = currentQ.QuestionOption || currentQ.questionOption || [];

        optionsList.forEach(opt => {
            const optId = opt.OptionID !== undefined ? opt.OptionID : opt.optionID;
            opt.IsAnswer = (optId === optionId);
            opt.isAnswer = (optId === optionId);
        });

        setQuestions(updatedQuestions);

        api.post('/competition/submit-answer', { 
            QuestionID: questionId, 
            selectedOption: optionId, 
            ExamID: examId 
        })
            .then(res => {
                const response = res.data;
                const isSuccess = response.Success !== undefined ? response.Success : response.success;
                if (isSuccess) {
                    const newAttempted = updatedQuestions.filter(q => {
                        const opts = q.QuestionOption || q.questionOption || [];
                        return opts.some(o => (o.IsAnswer !== undefined ? o.IsAnswer : o.isAnswer));
                    }).length;
                    setAttemptedCount(newAttempted);
                }
            })
            .catch(err => console.error("Submit option error:", err));
    };

    const handleOpenSummary = () => {
        setShowSummaryModal(true);
    };

    const handleFinishExam = () => {
        setShowSummaryModal(false);
        setLoading(true);
        setLoadStatus("Analyzing Result Please Wait...");

        api.get(`/competition/get-exam-result?ExamID=${examId}&IsIQuiz=${isInstituteQuiz}&Iscertificate=${isCertificate}&IsAttempt=false`)
            .then(res => {
                const response = res.data;
                if (response.success || response.Success) {
                    setIsFinished(true);
                    setLoading(false);
                } else {
                    alert(response.Message || response.message);
                    setLoading(false);
                }
            })
            .catch(err => {
                console.error("Exam completion error:", err);
                setLoading(false);
            });
    };

    const currentQuestion = questions[currentIndex];
    const unattemptedCount = questions.length - attemptedCount;

    if (loading) {
        return (
            <div className="d-flex flex-column justify-content-center align-items-center min-vh-100 text-white" style={{ background: "radial-gradient(circle, #1a1c29 0%, #0d0e15 100%)" }}>
                <div className="spinner-border text-warning mb-3" role="status" style={{ width: '3rem', height: '3rem' }}></div>
                <span className="fs-5 fw-semibold">{loadStatus}</span>
            </div>
        );
    }

    return (
        <div
            className="min-vh-100 d-flex align-items-center justify-content-center py-4 px-3"
            style={{
                background: "radial-gradient(circle, #1a1c29 0%, #0d0e15 100%)",
                color: "#fff",
            }}
        >
            {/* Expanded Container Width */}
            <div
                className="card p-3 p-md-4 shadow-lg text-center mx-auto rounded-4"
                style={{
                    maxWidth: "960px",
                    width: "100%",
                    background: "rgba(255, 255, 255, 0.05)",
                    backdropFilter: "blur(12px)",
                    border: "1px solid rgba(255, 255, 255, 0.15)",
                }}
            >
                {/* Header Navigation Bar */}
                <div className="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
                    <button
                        className="btn btn-outline-light btn-sm fw-bold px-3 py-1 rounded-pill d-flex align-items-center gap-1"
                        onClick={() => navigate('/quiz-dashboard')}
                    >
                        <i className="bi bi-arrow-left"></i> Back
                    </button>

                    <div className="d-flex gap-2 align-items-center">
                        <span className="badge bg-primary fs-6 px-3 py-2 rounded-pill fw-bold">
                            Q {currentIndex + 1} / {questions.length}
                        </span>

                       
                    </div>

                    <button
                        className="btn btn-outline-warning btn-sm fw-bold px-3 py-1 rounded-pill d-flex align-items-center gap-1"
                        onClick={() => navigate('/dashboard')}
                    >
                        <i className="bi bi-house-door-fill"></i> Dashboard
                    </button>
                </div>

                {!isFinished ? (
                    <>
                        {/* Title & Stats */}
                        <h3 className="fw-bold text-warning mb-2 text-uppercase">{quizName}</h3>

                        <div className="d-flex justify-content-center gap-2 gap-md-3 mb-3 flex-wrap small">
                            <span className="badge bg-secondary text-white px-3 py-2 rounded-pill border border-secondary">
                                Total: {questions.length}
                            </span>
                            <span className="badge bg-success text-white px-3 py-2 rounded-pill border border-success">
                                Attempted: {attemptedCount}
                            </span>
                            <span className="badge bg-warning text-dark px-3 py-2 rounded-pill border border-warning fw-bold">
                                Unattempted: {unattemptedCount}
                            </span>
                        </div>

                        {/* Question Palette Number Grid */}
                        <div 
                            className="d-flex flex-wrap justify-content-center gap-2 mb-4 p-2 rounded-3"
                            style={{ background: "rgba(0, 0, 0, 0.25)", maxHeight: "120px", overflowY: "auto" }}
                        >
                            {questions.map((q, idx) => {
                                const isAnswered = q.QuestionOption?.some(o => o.IsAnswer || o.isAnswer) || false;
                                const isActive = idx === currentIndex;
                                return (
                                    <button
                                        key={q.QuestionID || q.questionID || idx}
                                        onClick={() => setCurrentIndex(idx)}
                                        className="btn btn-sm rounded-circle p-0 fw-bold d-flex justify-content-center align-items-center"
                                        style={{
                                            width: '38px',
                                            height: '38px',
                                            backgroundColor: isActive ? '#f59e0b' : (isAnswered ? '#10b981' : '#334155'),
                                            color: '#fff',
                                            border: isActive ? '2px solid #fff' : '1px solid rgba(255,255,255,0.2)',
                                            transition: 'all 0.2s',
                                            fontSize: '14px'
                                        }}
                                    >
                                        {idx + 1}
                                    </button>
                                );
                            })}
                        </div>

                        {/* Current Question Block */}
                        {currentQuestion && (
                            <div className="text-start mb-4 px-md-2">
                                <h5 className="fw-bold text-light mb-4 fs-5 leading-snug">
                                    <span className="text-warning me-2 fs-5">{currentIndex + 1}.</span> 
                                    {currentQuestion.QuestionName || currentQuestion.questionName}
                                </h5>

                                {/* Options Grid */}
                                <div className="row g-3">
                                    {((currentQuestion.QuestionOption || currentQuestion.questionOption) || []).map((opt, oIdx) => {
                                        const optionLabels = ['A', 'B', 'C', 'D', 'E'];
                                        const optId = opt.OptionID !== undefined ? opt.OptionID : opt.optionID;
                                        const optText = opt.Options !== undefined ? opt.Options : opt.options;
                                        const isAns = opt.IsAnswer !== undefined ? opt.IsAnswer : opt.isAnswer;

                                        return (
                                            optText && (
                                                <div className="col-12 col-md-6" key={optId}>
                                                    <div
                                                        onClick={() => handleSelectOption(currentQuestion.QuestionID || currentQuestion.questionID, optId)}
                                                        className="p-3 rounded-3 cursor-pointer d-flex align-items-center transition-all h-100"
                                                        style={{
                                                            background: isAns ? "rgba(16, 185, 129, 0.25)" : "rgba(255, 255, 255, 0.08)",
                                                            border: isAns ? "2px solid #10b981" : "1px solid rgba(255, 255, 255, 0.15)",
                                                            cursor: "pointer",
                                                            color: "#fff",
                                                            transition: "all 0.2s ease"
                                                        }}
                                                    >
                                                        <span 
                                                            className={`badge me-3 rounded-circle d-flex align-items-center justify-content-center flex-shrink-0 ${isAns ? 'bg-success' : 'bg-primary'}`}
                                                            style={{ width: '32px', height: '32px', fontSize: '14px', fontWeight: 'bold' }}
                                                        >
                                                            {optionLabels[oIdx]}
                                                        </span>
                                                        <span className="fw-medium text-start fs-6">{optText}</span>
                                                    </div>
                                                </div>
                                            )
                                        );
                                    })}
                                </div>
                            </div>
                        )}

                        {/* Navigation Footer */}
                        <div className="d-flex justify-content-between align-items-center border-top border-secondary pt-3 mt-3">
                            <button
                                className="btn btn-outline-light text-white px-4 py-2 rounded-pill fw-semibold d-flex align-items-center gap-2"
                                onClick={() => setCurrentIndex(prev => Math.max(prev - 1, 0))}
                                disabled={currentIndex === 0}
                            >
                                <i className="bi bi-arrow-left"></i> Previous
                            </button>

                            {currentIndex === questions.length - 1 ? (
                                <button
                                    className="btn btn-success fw-bold px-4 py-2 rounded-pill shadow-lg d-flex align-items-center gap-2"
                                    onClick={handleOpenSummary}
                                >
                                    Submit Exam <i className="bi bi-send-fill"></i>
                                </button>
                            ) : (
                                <button
                                    className="btn btn-primary px-4 py-2 rounded-pill fw-semibold d-flex align-items-center gap-2"
                                    onClick={() => setCurrentIndex(prev => Math.min(prev + 1, questions.length - 1))}
                                >
                                    Next <i className="bi bi-arrow-right"></i>
                                </button>
                            )}
                        </div>
                    </>
                ) : (
				/* Completion View */
                    <div className="py-4 text-center">
                        <h2 className="text-success fw-bold mb-3">🎉 Exam Completed Successfully!</h2>
                        <p className="text-light-50 mb-4">Your responses have been recorded for evaluation.</p>
                       {/* <button 
                            className="btn btn-warning rounded-pill px-4 fw-bold py-2 shadow" 
                            onClick={() => navigate('/quiz-dashboard')}
                        >
                            Return to Quiz Dashboard
                        </button>*/}
				</div>
                )}
            </div>

            {/* Exam Summary Modal Popup */}
            {showSummaryModal && (
                <div 
                    className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center px-3"
                    style={{ backgroundColor: 'rgba(0, 0, 0, 0.8)', zIndex: 1050 }}
                >
                    <div 
                        className="card p-4 rounded-4 shadow-lg text-center bg-dark text-white border-secondary"
                        style={{ maxWidth: '440px', width: '100%' }}
                    >
                        <h4 className="fw-bold text-warning mb-3">Exam Summary</h4>
                        
                        <div className="p-3 mb-3 rounded-3 text-start bg-secondary bg-opacity-25 border border-secondary">
                            <div className="d-flex justify-content-between mb-2">
                                <span>Total Questions:</span>
                                <strong className="text-info fs-6">{questions.length}</strong>
                            </div>
                            <div className="d-flex justify-content-between mb-2">
                                <span>Attempted:</span>
                                <strong className="text-success fs-6">{attemptedCount}</strong>
                            </div>
                            <div className="d-flex justify-content-between">
                                <span>Unattempted:</span>
                                <strong className="text-warning fs-6">{unattemptedCount}</strong>
                            </div>
                        </div>

                        <p className="small text-danger fw-semibold mb-4">
                            ⚠️ Please ensure you have reviewed your answers before final submission.
                        </p>

                        <div className="d-flex gap-2 justify-content-end">
                            <button
                                className="btn btn-outline-light rounded-pill px-3 py-2 fw-semibold"
                                onClick={() => setShowSummaryModal(false)}
                            >
                                Cancel
                            </button>
                            <button
                                className="btn btn-success rounded-pill px-4 py-2 fw-bold"
                                onClick={handleFinishExam}
                            >
                                Confirm Submit
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default QuizPage;