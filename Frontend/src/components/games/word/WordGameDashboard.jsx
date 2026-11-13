import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import RulesModal from '../../../components/dashboard/RulesModal';
import api from '../../../services/api';
import DashboardHeader from "../../../components/dashboard/DashboardHeader";
import PdfViewerModal from "../../../components/dashboard/PdfViewerModal";

export default function WordGameDashboard() {
  const navigate = useNavigate();

  const [selectedSubGame, setSelectedSubGame] = useState(null);
  const [showRulesModal, setShowRulesModal] = useState(false);
  const [showGuide, setShowGuide] = useState(false);

  // Dashboard Header & API state
  const [dashboardData, setDashboardData] = useState(null);
  const [currentDate, setCurrentDate] = useState('');
  const [statuses, setStatuses] = useState(null);
  const [loading, setLoading] = useState(true);

  // Dedicated word games configuration data
  const wordGameData = {
    mainTitle: 'Word Games',
    subGames: [
      {
        subGameId: 'scrumble',
        title: 'Scramble',
        image: '/Scrumble.jpeg',
        rules: [
          'Unscramble the given letters to form valid words.',
          'Each correct word earns points.',
          'Click "Start Game" to begin.'
        ]
      },
      {
        subGameId: 'puzzle',
        title: 'Puzzle',
        image: '/puzzle.png',
        rules: [
          'Search for hidden words horizontally, vertically, or diagonally in the letter grid.',
          'Complete all words or use skip if needed.',
          'Click "Start Game" to begin.'
        ]
      },
      {
        subGameId: 'wordgame',
        title: 'Word Challenge',
        image: '/Word.jpeg',
        rules: [
          'Complete all sequential word levels within the given time limit.',
          'Click "Start Game" when ready.'
        ]
      }
    ]
  };

  // Fetch Live Statuses and User Profile from API
  useEffect(() => {
    const fetchUserDataAndStatuses = async () => {
      try {
        setLoading(true);

        // Format current date for the header
        const options = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
        setCurrentDate(new Date().toLocaleDateString('en-US', options));

        // Fetch User Profile
        const profileRes = await api.get('/dashboard/user-profile');
        setDashboardData(profileRes.data);

        const userId = profileRes.data.studentId || profileRes.data.id || profileRes.data.userId;
        if (userId) {
          localStorage.setItem("userId", userId);
          const endpoint = `/Competition/wordgame/status/${userId}?_t=${Date.now()}`;
          const res = await api.get(endpoint);
          setStatuses(res.data);
        }
      } catch (err) {
        console.error("Error fetching user profile or word game statuses:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchUserDataAndStatuses();
  }, []);

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

  // Dynamic Button & Badge UI Config Generator
  const getButtonConfig = (subGameId) => {
    let rawStatus = 'S'; // Default 'S' (Start)

    if (statuses) {
      const subId = subGameId.toLowerCase();
      if (subId.includes('scramble') || subId.includes('scrumble')) {
        rawStatus = statuses.scramble_Game_Status || statuses.Scramble_Game_Status || 'S';
      } else if (subId.includes('puzzle')) {
        rawStatus = statuses.puzzle_Game_Status || statuses.Puzzle_Game_Status || 'S';
      } else if (subId.includes('wordgame')) {
        rawStatus = statuses.word_Game_Status || statuses.Word_Game_Status || 'S';
      }
    }

    const st = String(rawStatus).toUpperCase();

    if (st === 'C') {
      return { 
        label: 'Completed', 
        disabled: true, 
        className: 'btn-success opacity-75',
        badgeText: 'Completed',
        badgeBg: 'bg-success',
        borderColor: '#6c757d',
        icon: 'bi-check-circle-fill'
      };
    } else if (st === 'P') {
      return { 
        label: 'Continue', 
        disabled: false, 
        className: 'btn-warning text-dark fw-bold',
        badgeText: 'In Progress',
        badgeBg: 'bg-warning-subtle text-warning-emphasis',
        borderColor: '#f59e0b',
        icon: 'bi-play-circle-fill'
      };
    } else if (st === 'LOCKED') {
      return { 
        label: 'Locked', 
        disabled: true, 
        className: 'btn-secondary',
        badgeText: 'Locked',
        badgeBg: 'bg-danger-subtle text-danger',
        borderColor: '#dc3545',
        icon: 'bi-lock-fill'
      };
    } else {
      return { 
        label: 'Start Challenge', 
        disabled: false, 
        className: 'btn-primary',
        badgeText: 'Active',
        badgeBg: 'bg-primary-subtle text-primary',
        borderColor: '#0d6efd',
        icon: 'bi-play-fill'
      };
    }
  };

  const handleOpenRules = (subGame) => {
    setSelectedSubGame(subGame);
    setShowRulesModal(true);
  };

  const handleStartGame = async () => {
    setShowRulesModal(false);

    // Fullscreen trigger on user interaction for ExamSecurityProvider
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
      console.warn("Fullscreen permission error:", err);
    }

    if (selectedSubGame) {
      if (selectedSubGame.subGameId === 'puzzle') {
        navigate('/puzzle');
      } else if (selectedSubGame.subGameId === 'wordgame') {
        navigate('/word');
      } else if (selectedSubGame.subGameId === 'scrumble') {
        navigate('/scrumble');
      } else {
        navigate(`/competition/play/3?subGame=${selectedSubGame.subGameId}`);
      }
    }
  };

  const initials = dashboardData ? `${dashboardData.firstName?.[0] || ''}${dashboardData.lastName?.[0] || ''}` : 'U';

  const subGamesCount = wordGameData.subGames.length;
  const colClass = subGamesCount >= 4 
    ? 'col-12 col-sm-6 col-md-4 col-lg-3' 
    : 'col-12 col-md-6 col-lg-4';

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

      {/* Mobile-Responsive Header Container */}
      <div className="d-flex flex-row justify-content-between align-items-center mb-4 px-2 px-md-4 gap-2">
        <div>
          <h3 className="fw-bold text-dark m-0 fs-4 fs-md-2">{wordGameData.mainTitle}</h3>
          <p className="text-muted small m-0 d-none d-sm-block">Select a word challenge to test your vocabulary</p>
        </div>
        <button 
          className="btn btn-outline-secondary rounded-pill px-3 py-1 py-md-2 btn-sm text-nowrap fw-semibold shadow-sm" 
          onClick={() => navigate('/dashboard')}
        >
          <i className="bi bi-arrow-left me-1"></i> Dashboard
        </button>
      </div>

      {loading ? (
        <div className="text-center py-5">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      ) : (
        <div className="container-lg px-2 px-md-3">
          <div className="row g-3 g-md-4 justify-content-center">
            {wordGameData.subGames.map((sub) => {
              const btn = getButtonConfig(sub.subGameId);

              return (
                <div className={colClass} key={sub.subGameId}>
                  <div 
                    className="card border-0 shadow-sm rounded-4 h-100 text-center p-3 d-flex flex-column justify-content-between bg-white overflow-hidden position-relative"
                    style={{ 
                      transition: "transform 0.3s ease, box-shadow 0.3s ease",
                      borderTop: `5px solid ${btn.borderColor}`
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.transform = "translateY(-6px)";
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.transform = "translateY(0)";
                    }}
                  >
                    {/* Game Image Container */}
                    <div className="my-2 d-flex align-items-center justify-content-center" style={{ minHeight: '130px' }}>
                      <img
                        src={sub.image}
                        alt={sub.title}
                        className="img-fluid"
                        style={{ maxHeight: '120px', objectFit: 'contain' }}
                      />
                    </div>

                    {/* Title & Action Button */}
                    <div>
                      <h6 className="fw-bold text-dark my-3 text-truncate" title={sub.title}>
                        {sub.title}
                      </h6>
                      <button
                        className={`btn ${btn.className} px-3 rounded-pill w-100 fw-semibold py-2 shadow-sm`}
                        disabled={btn.disabled}
                        onClick={() => handleOpenRules(sub)}
                      >
                        <i className={`bi ${btn.icon} me-1`}></i> {btn.label}
                      </button>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* Rules Modal */}
      <RulesModal
        show={showRulesModal}
        onClose={() => setShowRulesModal(false)}
        gameDetails={selectedSubGame}
        onStartGame={handleStartGame}
      />
    </div>
  );
}