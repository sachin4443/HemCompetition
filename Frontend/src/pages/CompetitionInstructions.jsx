import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { GAME_CATEGORIES_DATA } from '../data/gameRulesData';
import RulesModal from '../components/dashboard/RulesModal';
import DashboardHeader from "../components/dashboard/DashboardHeader";
import PdfViewerModal from "../components/dashboard/PdfViewerModal";
import api from '../services/api';

export default function CompetitionInstructions() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [selectedSubGame, setSelectedSubGame] = useState(null);
  const [showRulesModal, setShowRulesModal] = useState(false);
  const [showGuide, setShowGuide] = useState(false);

  // Dashboard Header & Profile states
  const [dashboardData, setDashboardData] = useState(null);
  const [currentDate, setCurrentDate] = useState('');
  const [statuses, setStatuses] = useState(null);
  const [loading, setLoading] = useState(true);

  const categoryData = GAME_CATEGORIES_DATA[id];

  useEffect(() => {
    // Format current date for header
    const options = { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' };
    setCurrentDate(new Date().toLocaleDateString('en-US', options));
  }, []);

  useEffect(() => {
    const fetchStatusesAndProfile = async () => {
      try {
        setLoading(true);

        // Fetch User Profile for DashboardHeader
        const profileRes = await api.get('/dashboard/user-profile');
        setDashboardData(profileRes.data);
        
        const validUserId = profileRes.data.studentId || profileRes.data.id || profileRes.data.userId;
        if (validUserId) {
          localStorage.setItem("userId", validUserId);
        }

        const userId = validUserId || 297680;

        if (userId && id) {
          let endpoint = '';
          if (String(id) === '1') endpoint = `/Competition/numbergame/status/${userId}`;
          else if (String(id) === '2') endpoint = `/Competition/quiz/status/${userId}`;
          else if (String(id) === '3') endpoint = `/Competition/wordgame/status/${userId}`;

          if (endpoint) {
            const res = await api.get(endpoint);
            setStatuses(res.data);
          }
        }
      } catch (err) {
        console.error("Error fetching sub-game statuses or user profile:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchStatusesAndProfile();
  }, [id]);

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

  const getButtonConfig = (subGameId) => {
    let rawStatus = 'S';

    if (statuses) {
      const subId = subGameId.toLowerCase();

      if (String(id) === '1') {
        if (subId.includes('kenken')) rawStatus = statuses.kenken_Game_Status || statuses.kenkenGameStatus || 'S';
        if (subId.includes('futoshiki')) rawStatus = statuses.futoshiki_Game_Status || statuses.Futoshiki_Game_Status || 'S';
        if (subId.includes('game24')) rawStatus = statuses.game24_Game_Status || statuses.Game24_Game_Status || 'S';
      } else if (String(id) === '2') {
        if (subId === 'quiz1') rawStatus = (statuses.isQuiz1Locked === 1 || statuses.IsQuiz1Locked === 1) ? 'LOCKED' : 'S';
        if (subId === 'quiz2') rawStatus = (statuses.isQuiz2Locked === 1 || statuses.IsQuiz2Locked === 1) ? 'LOCKED' : 'S';
        if (subId === 'quiz3') rawStatus = (statuses.isQuiz3Locked === 1 || statuses.IsQuiz3Locked === 1) ? 'LOCKED' : 'S';
        if (subId === 'quiz4') rawStatus = (statuses.isQuiz4Locked === 1 || statuses.IsQuiz4Locked === 1) ? 'LOCKED' : 'S';
      } else if (String(id) === '3') {
        if (subId.includes('scramble')) rawStatus = statuses.scramble_Game_Status || statuses.Scramble_Game_Status || 'S';
        if (subId.includes('puzzle')) rawStatus = statuses.puzzle_Game_Status || statuses.Puzzle_Game_Status || 'S';
        if (subId.includes('wordgame')) rawStatus = statuses.word_Game_Status || statuses.Word_Game_Status || 'S';
      }
    }

    const st = String(rawStatus).toUpperCase();

    if (st === 'C') {
      return { label: 'Completed', disabled: true, className: 'btn-success' };
    } else if (st === 'P') {
      return { label: 'Continue', disabled: false, className: 'btn-warning text-dark' };
    } else if (st === 'LOCKED') {
      return { label: 'Locked', disabled: true, className: 'btn-secondary' };
    } else {
      return { label: 'Start', disabled: false, className: 'btn-primary' };
    }
  };

  if (!categoryData) {
    return (
      <div className="container py-5 text-center">
        <h4>Game Category Not Found!</h4>
        <button className="btn btn-primary mt-3 rounded-pill" onClick={() => navigate('/dashboard')}>
          Go to Dashboard
        </button>
      </div>
    );
  }

  const handleOpenRules = (subGame) => {
    setSelectedSubGame(subGame);
    setShowRulesModal(true);
  };

  const handleStartGame = async () => {
    setShowRulesModal(false);

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
      navigate(`/competition/play/${id}?subGame=${selectedSubGame.subGameId}`);
    }
  };

  const initials = dashboardData ? `${dashboardData.firstName?.[0] || ''}${dashboardData.lastName?.[0] || ''}` : 'U';

  const subGamesCount = categoryData.subGames.length;
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
      <div className="d-flex flex-row justify-content-between align-items-center mb-4 px-2 px-md-3 gap-2">
        <h4 className="fw-bold text-dark m-0 fs-5 fs-md-3 text-truncate">
          {categoryData.mainTitle}
        </h4>
        <button 
          className="btn btn-outline-secondary rounded-pill px-3 py-1 py-md-2 btn-sm text-nowrap" 
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
        <div className="row g-3 justify-content-center px-1 px-md-2">
          {categoryData.subGames.map((sub) => {
            const btn = getButtonConfig(sub.subGameId);

            return (
              <div className={colClass} key={sub.subGameId}>
                <div className="card border-0 shadow-sm rounded-4 h-100 text-center p-3 d-flex flex-column justify-content-between bg-white">
                  <div className="mb-2 d-flex align-items-center justify-content-center" style={{ minHeight: '120px' }}>
                    <img
                      src={sub.image}
                      alt={sub.title}
                      className="img-fluid"
                      style={{ maxHeight: '110px', objectFit: 'contain' }}
                    />
                  </div>

                  <div>
                    <h6 className="fw-bold mb-3 text-truncate" title={sub.title}>{sub.title}</h6>
                    <button
                      className={`btn ${btn.className} px-3 rounded-pill w-100 fw-semibold py-2`}
                      disabled={btn.disabled}
                      onClick={() => handleOpenRules(sub)}
                    >
                      {btn.label}
                    </button>
                  </div>
                </div>
              </div>
            );
          })}
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