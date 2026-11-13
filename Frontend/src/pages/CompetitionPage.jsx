import { useSearchParams, useNavigate } from 'react-router-dom';
import FutoshikiGame from '../components/games/number/FutoshikiGame';
import Game24 from '../components/games/number/Game24';
import Kenken from '../components/games/number/kenken';
import QuizGameComponent from '../components/games/quiz/QuizGameComponent';

export default function CompetitionPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const rawSubGameId = searchParams.get('subGame');
  const subGameId = rawSubGameId ? rawSubGameId.toLowerCase() : '';

  const renderGameComponent = () => {
    switch (subGameId) {
      case 'futoshiki':
        return <FutoshikiGame />;
      case 'game24':
        return <Game24 />;
      case 'kenken':
        return <Kenken />;
      case 'quiz1':
      case 'quiz2':
      case 'quiz3':
      case 'quiz4':
        return <QuizGameComponent quizId={subGameId} />;
      default:
        return (
          <div className="text-center py-5">
            <h4 className="fw-bold text-dark">Game Component Under Construction</h4>
            <p className="text-muted small">SubGame ID: {rawSubGameId}</p>
            <button className="btn btn-primary rounded-pill px-4 mt-2" onClick={() => navigate(-1)}>
              Go Back
            </button>
          </div>
        );
    }
  };

  return (
    <div className="container-fluid py-2 py-md-4 min-vh-100 d-flex flex-column justify-content-center px-2 px-md-3">
      {renderGameComponent()}
    </div>
  );
}