import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { verifyAuthToken } from '../services/authService';
import 'bootstrap-icons/font/bootstrap-icons.css';
import DashboardHeader from "../components/dashboard/DashboardHeader";
import PdfViewerModal from "../components/dashboard/PdfViewerModal";

export default function DashboardPage() {
    const [dashboardData, setDashboardData] = useState(null);
    const [gameStatuses, setGameStatuses] = useState(null);
    const [loading, setLoading] = useState(true);
    const [currentDate, setCurrentDate] = useState('');
    const navigate = useNavigate();
    const [showGuide, setShowGuide] = useState(false);

    useEffect(() => {
        const checkSecurityAndLoad = async () => {
            const isValid = await verifyAuthToken();
            if (!isValid) {
                localStorage.clear();
                sessionStorage.clear();
                navigate('/', { replace: true });
                return;
            }

            const options = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
            setCurrentDate(new Date().toLocaleDateString('en-US', options));

            try {
                // 1. Fetch User Profile
                const profileRes = await api.get('/dashboard/user-profile');
                setDashboardData(profileRes.data);
// Add this line so all sub-pages get the exact ID instantly:
const validUserId = profileRes.data.userId || profileRes.data.studentId || profileRes.data.id;
if (validUserId) {
  localStorage.setItem("userId", validUserId);
}
                // 2. Fetch Dynamic Game Status using Student/User ID
                const studentId = profileRes.data.studentId || profileRes.data.id || profileRes.data.userId;
                if (studentId) {
                    const statusRes = await api.get(`/Competition/status/${studentId}`);
                    setGameStatuses(statusRes.data);
                }
            } catch (err) {
                console.error('Unauthorized session or API error', err);
                localStorage.clear();
                navigate('/', { replace: true });
            } finally {
                setLoading(false);
            }
        };

        checkSecurityAndLoad();

        const handlePopState = async () => {
            const isValid = await verifyAuthToken();
            if (!isValid) {
                navigate('/', { replace: true });
            }
        };
        window.addEventListener('popstate', handlePopState);

        return () => {
            window.removeEventListener('popstate', handlePopState);
        };
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

    // Helper function to calculate button state based on 'S', 'P', 'C'
    const getGameStatusDetails = (gameId) => {
        // Mapping gameId to API response keys
        // Game 1 = Number Game, Game 2 = Quiz Game, Game 3 = Word Game
        let statusCode = 'S'; // Default Start

        if (gameStatuses) {
            if (gameId === 1) statusCode = gameStatuses.number_Game_Status || gameStatuses.numberGameStatus || 'S';
            else if (gameId === 2) statusCode = gameStatuses.quiz_Game_Status || gameStatuses.quizGameStatus || 'S';
            else if (gameId === 3) statusCode = gameStatuses.word_Game_Status || gameStatuses.wordGameStatus || 'S';
        }

        const normalizedStatus = String(statusCode).toUpperCase();

        if (normalizedStatus === 'C') {
            return {
                label: 'Completed',
                disabled: true,
                btnClass: 'btn-success',
                icon: 'bi-check-circle-fill'
            };
        } else if (normalizedStatus === 'P') {
            return {
                label: 'Continue Challenge',
                disabled: false,
                btnClass: 'btn-warning text-dark',
                icon: 'bi-play-circle-fill'
            };
        } else {
            // Default 'S' or Start
            return {
                label: 'Start Challenge',
                disabled: false,
                btnClass: 'btn-primary',
                icon: 'bi-play-fill'
            };
        }
    };

    if (loading) {
        return (
            <div className="container d-flex justify-content-center align-items-center min-vh-100">
                <div className="spinner-border text-success" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            </div>
        );
    }

    const initials = dashboardData ? `${dashboardData.firstName?.[0] || ''}${dashboardData.lastName?.[0] || ''}` : 'U';

    return (
        <div className="container-fluid py-4">
            <DashboardHeader
                dashboardData={dashboardData}
                currentDate={currentDate}
                initials={initials}
                handleLogout={handleLogout}
                onOpenGuide={() => setShowGuide(true)}
            />

            <PdfViewerModal show={showGuide} onClose={() => setShowGuide(false)} />

            {/* --- Section Title & Active Badge --- */}
            <div className="d-flex justify-content-between align-items-center mb-4 px-1">
                <h4 className="fw-bold text-dark m-0">Dashboard</h4>
                <span className="badge bg-success-subtle text-success rounded-pill px-3 py-2 small fw-semibold">
                    <i className="bi bi-circle-fill text-success me-1" style={{ fontSize: '0.5rem' }}></i> Active Session
                </span>
            </div>

            {/* --- Cards Grid --- */}
            <div className="row g-4">
                {dashboardData?.games?.map((game) => {
                    const image =
                        game.gameId === 1
                            ? "/Numbergame.png"
                            : game.gameId === 2
                                ? "/QuizGame.png"
                                : "/WordGame.png";

                    const statusInfo = getGameStatusDetails(game.gameId);

                    return (
                        <div className="col-12 col-md-6 col-xl-4" key={game.gameId}>
                            <div
                                className="card border-0 shadow rounded-4 h-100 text-center overflow-hidden d-flex flex-column"
                                style={{ transition: ".3s" }}
                                onMouseEnter={(e) => {
                                    e.currentTarget.style.transform = "translateY(-8px)";
                                }}
                                onMouseLeave={(e) => {
                                    e.currentTarget.style.transform = "translateY(0)";
                                }}
                            >
                                <div className="p-4 pb-2">
                                    <img
                                        src={image}
                                        alt={game.title}
                                        className="img-fluid"
                                        style={{ height: "200px", objectFit: "contain" }}
                                    />
                                </div>

                                <div className="card-body d-flex flex-column justify-content-end flex-grow-1 p-4 pt-0">
                                    <div>
                                        <button
                                            className={`btn ${statusInfo.btnClass} btn-lg rounded-pill px-4 fw-semibold w-100`}
                                            disabled={statusInfo.disabled}
                                           onClick={() => {
    if (!statusInfo.disabled) {
        if (game.gameId === 2) {
            navigate('/quiz-dashboard'); 
        }else if (game.gameId === 3) {
            navigate('/word-dashboard'); 
        }
 else {
            navigate(`/competition/${game.gameId}`);
        }
    }
}}
                                        >
                                            <i className={`bi ${statusInfo.icon} me-2`}></i>
                                            {statusInfo.label}
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
}