import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

function QuizDashboard() {
    const navigate = useNavigate();

    useEffect(() => {
        document.body.style.margin = '0';
        document.body.style.backgroundColor = '#f8fafc';
        const userId = localStorage.getItem('userId');
        if (!userId) { 
            navigate('/', { replace: true }); 
            return; 
        }
    }, [navigate]);

    const handleStartQuiz = (topicId) => {
        const quizId = 332;
        const levelId = 1;
        const languageId = 1;
        const isInstituteQuiz = 'No';
        const isCertificate = 'False';

        // Redirects directly to your Competition/Quiz Play page with required query parameters
        navigate(`/competition/play/${quizId}?TopicID=${topicId}&LevelID=${levelId}&LanguageID=${languageId}&IsInstituteQuiz=${isInstituteQuiz}&Iscertificate=${isCertificate}`);
    };

    return (
        <div style={{ minHeight: '100vh', width: '100vw', background: '#f8fafc', fontFamily: 'Segoe UI, sans-serif', boxSizing: 'border-box' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', background: '#1e293b', padding: '18px 40px', color: '#fff' }}>
                <h2 style={{ margin: 0, fontSize: '20px', color: '#38bdf8' }}>Quiz Dashboard</h2>
                <button 
                    onClick={() => navigate('/dashboard', { replace: true })} 
                    style={{ padding: '8px 16px', background: '#64748b', color: '#fff', border: 'none', borderRadius: '6px', cursor: 'pointer' }}
                >
                    Back to Dashboard
                </button>
            </div>

            <div style={{ padding: '40px 5%', maxWidth: '1400px', margin: '0 auto', display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '25px' }}>
                {[6256, 6257, 6258, 6259].map((topicId, index) => (
                    <div key={topicId} style={{ background: '#fff', padding: '25px', borderRadius: '12px', boxShadow: '0 4px 6px rgba(0,0,0,0.05)', textAlign: 'center', borderTop: '5px solid #f59e0b' }}>
                        <h3>HS Quiz {index + 1}</h3>
                        <p style={{ color: '#64748b', fontSize: '14px' }}>Multiple-choice assessment track {index + 1}.</p>
                        <button 
                            onClick={() => handleStartQuiz(topicId)} 
                            style={{ width: '100%', padding: '12px', background: '#f59e0b', color: '#fff', border: 'none', borderRadius: '8px', fontWeight: 'bold', cursor: 'pointer' }}
                        >
                            Start Quiz
                        </button>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default QuizDashboard;