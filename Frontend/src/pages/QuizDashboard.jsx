import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from "../services/api";
import DashboardHeader from "../components/dashboard/DashboardHeader";
import PdfViewerModal from "../components/dashboard/PdfViewerModal";

function QuizDashboard() {
    const navigate = useNavigate();
    const [quizStatuses, setQuizStatuses] = useState({
        quiz1: 0,
        quiz2: 0,
        quiz3: 0,
        quiz4: 0
    });
    const [loadingStatus, setLoadingStatus] = useState(true);

    // Dashboard Header & Profile states
    const [dashboardData, setDashboardData] = useState(null);
    const [currentDate, setCurrentDate] = useState('');
    const [showGuide, setShowGuide] = useState(false);

    useEffect(() => {
        document.body.style.margin = '0';
        document.body.style.backgroundColor = '#f8fafc';

        // Format current date for header
        const options = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
        setCurrentDate(new Date().toLocaleDateString('en-US', options));
    }, []);

    useEffect(() => {
        const token = localStorage.getItem('jwt_token');
        if (!token) { 
            navigate('/', { replace: true }); 
            return;
        }

        let userId = localStorage.getItem('userId');

        if (!userId && token) {
            try {
                const base64Url = token.split('.')[1];
                const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
                const decoded = JSON.parse(window.atob(base64));
                userId = decoded.UserId || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
            } catch (e) {
                console.error("Error decoding token for userId:", e);
            }
        }

        // Fetch User Profile for DashboardHeader
        api.get('/dashboard/user-profile')
            .then(res => {
                setDashboardData(res.data);
                const validUserId = res.data.userId || res.data.studentId || res.data.id;
                if (validUserId) {
                    localStorage.setItem("userId", validUserId);
                }
            })
            .catch(err => {
                console.error("Failed to load user profile:", err);
            });

        if (userId) {
            api.get(`/competition/quiz/status/${userId}`)
                .then(res => {
                    const data = res.data;
                    setQuizStatuses({
                        quiz1: data.IsQuiz1Locked ?? data.isQuiz1Locked ?? 0,
                        quiz2: data.IsQuiz2Locked ?? data.isQuiz2Locked ?? 0,
                        quiz3: data.IsQuiz3Locked ?? data.isQuiz3Locked ?? 0,
                        quiz4: data.IsQuiz4Locked ?? data.isQuiz4Locked ?? 0
                    });
                })
                .catch(err => {
                    console.error("Failed to load quiz statuses:", err);
                })
                .finally(() => {
                    setLoadingStatus(false);
                });
        } else {
            Promise.resolve().then(() => setLoadingStatus(false));
        }
    }, [navigate]);

    const handleLogout = async () => {
        try {
            await api.post('/auth/logout');
        } catch (err) {
            console.error('Logout error', err);
        } finally {
            localStorage.clear();
            sessionStorage.clear();
            window.location.replace('/');
        }
    };

    const handleStartQuiz = async (topicId) => {
        // Trigger Fullscreen on User Action for Security Provider compatibility
        try {
            if (!document.fullscreenElement) {
                const docEl = document.documentElement;
                if (docEl.requestFullscreen) {
                    await docEl.requestFullscreen();
                } else if (docEl.webkitRequestFullscreen) {
                    await docEl.webkitRequestFullscreen();
                } else if (docEl.msRequestFullscreen) {
                    await docEl.msRequestFullscreen();
                }
            }
        } catch (err) {
            console.warn("Fullscreen error:", err);
        }

        const quizId = 332;
        const levelId = 1;
        const languageId = 1;
        const isInstituteQuiz = 'No';
        const isCertificate = 'False';

        navigate(`/Quiz/${quizId}?QuizId=${quizId}&TopicID=${topicId}&LevelID=${levelId}&LanguageID=${languageId}&IsInstituteQuiz=${isInstituteQuiz}&Iscertificate=${isCertificate}`);
    };

    const topics = [
        { id: 6256, statusKey: 'quiz1', name: 'HEM MEGA Quiz 1', path: '/HS_Quiz_1.png' },
        { id: 6257, statusKey: 'quiz2', name: 'HEM MEGA Quiz 2', path: '/HS_Quiz_2.png' },
        { id: 6258, statusKey: 'quiz3', name: 'HEM MEGA Quiz 3', path: '/HS_Quiz_3.png' },
        { id: 6259, statusKey: 'quiz4', name: 'HEM MEGA Quiz 4', path: '/HS_Quiz_4.png' }
    ];

    const initials = dashboardData ? `${dashboardData.firstName?.[0] || ''}${dashboardData.lastName?.[0] || ''}` : 'U';

    return (
        <div className="container-fluid py-3 py-md-4 min-vh-100 bg-light">
            {/* Integrated Dashboard Header */}
            <DashboardHeader
                dashboardData={dashboardData}
                currentDate={currentDate}
                initials={initials}
                handleLogout={handleLogout}
                onOpenGuide={() => setShowGuide(true)}
            />

            <PdfViewerModal show={showGuide} onClose={() => setShowGuide(false)} />

            {/* Header Area */}
            <div className="d-flex flex-row justify-content-between align-items-center mb-4 px-2 px-md-4 gap-2">
                <div>
                    <h3 className="fw-bold text-dark m-0 fs-4 fs-md-2">Hem Mega Quiz</h3>
                    <p className="text-muted small m-0 d-none d-sm-block">Select a round to start your competition</p>
                </div>
                <button 
                    className="btn btn-outline-secondary rounded-pill px-3 py-1 py-md-2 btn-sm text-nowrap fw-semibold shadow-sm" 
                    onClick={() => navigate('/dashboard')}
                >
                    <i className="bi bi-arrow-left me-1"></i> Dashboard
                </button>
            </div>

            {/* Quiz Grid */}
            <div className="container-lg px-2 px-md-3">
                <div className="row g-3 g-md-4 justify-content-center">
                    {topics.map((topic) => {
                        const statusValue = quizStatuses[topic.statusKey];
                        const isCompleted = statusValue === 1;

                        return (
                            <div className="col-12 col-sm-6 col-lg-3" key={topic.id}>
                                <div 
                                    className="card border-0 shadow-sm rounded-4 h-100 text-center p-3 d-flex flex-column justify-content-between overflow-hidden position-relative bg-white"
                                    style={{ 
                                        transition: "transform 0.3s ease, shadow 0.3s ease",
                                        borderTop: `5px solid ${isCompleted ? '#6c757d' : '#0d6efd'}` 
                                    }}
                                    onMouseEnter={(e) => {
                                        e.currentTarget.style.transform = "translateY(-6px)";
                                    }}
                                    onMouseLeave={(e) => {
                                        e.currentTarget.style.transform = "translateY(0)";
                                    }}
                                >
                                    {/* Quiz Image */}
                                    <div className="my-2 d-flex align-items-center justify-content-center" style={{ minHeight: '130px' }}>
                                        <img
                                            src={topic.path}
                                            alt={topic.name}
                                            className="img-fluid"
                                            style={{ maxHeight: '120px', objectFit: 'contain' }}
                                        />
                                    </div>

                                    {/* Title & Action Button */}
                                    <div>
                                        <h6 className="fw-bold text-dark my-3 text-truncate" title={topic.name}>
                                            {topic.name}
                                        </h6>

                                        <button 
                                            onClick={() => !isCompleted && handleStartQuiz(topic.id)} 
                                            disabled={isCompleted || loadingStatus}
                                            className={`btn w-100 rounded-pill py-2 fw-semibold shadow-sm ${
                                                isCompleted 
                                                    ? 'btn-success opacity-75' 
                                                    : 'btn-primary'
                                            }`}
                                        >
                                            {loadingStatus ? (
                                                <span>
                                                    <span className="spinner-border spinner-border-sm me-2" role="status"></span>
                                                    Loading...
                                                </span>
                                            ) : isCompleted ? (
                                                <span>
                                                    <i className="bi bi-check-circle-fill me-1"></i> Completed
                                                </span>
                                            ) : (
                                                <span>
                                                    <i className="bi bi-play-fill me-1"></i> Start Quiz
                                                </span>
                                            )}
                                        </button>
                                    </div>
                                </div>
                            </div>
                        );
                    })}
                </div>
            </div>
        </div>
    );
}

export default QuizDashboard;