import { useEffect } from 'react';
import {
    BrowserRouter as Router,
    Routes,
    Route
} from 'react-router-dom';

import AuthenticatedSecurityLayout
    from "./components/security/AuthenticatedSecurityLayout";

import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import DuplicateTabPage from './pages/DuplicateTabPage';

import GameSecurityWrapper
    from "./components/security/GameSecurityWrapper";

import ProtectedRoute
    from './components/guards/ProtectedRoute';

import CompetitionInstructions
    from "./pages/CompetitionInstructions";

import CompetitionPage
    from "./pages/CompetitionPage";

import ResultPage
    from "./pages/ResultPage";

import QuizDashboard
    from './pages/QuizDashboard';

import api from './services/api';

import QuizPage
    from './components/games/quiz/QuizPage';

import PuzzlePage
    from './components/games/word/PuzzlePage';

import WordDashboard
    from './components/games/word/WordGameDashboard';

import WordGamePage
    from './components/games/word/WordGamePage';

import ScramblePage
    from './components/games/word/ScramblePage';

import CompetitionSecurityPage
    from "./pages/CompetitionSecurityPage";

import AdminLoginPage
    from "./pages/admin/AdminLoginPage";

import AdminDashboard
    from "./pages/admin/AdminDashboard";
import AdminSessionsPage
  from "./pages/admin/AdminSessionsPage";

// =====================================================
// NORMAL USER SESSION GUARD
// =====================================================

function SessionGuard() {

    useEffect(() => {

        const checkActiveSession = async () => {

            const token =
                localStorage.getItem("jwt_token");

            // Admin login ko touch nahi karna
            if (!token) {
                return;
            }

            try {

                await api.post("/auth/heartbeat");

            } catch (error) {

                console.warn(
                    "User session expired."
                );

                localStorage.removeItem("jwt_token");
                localStorage.removeItem("user_profile");

                sessionStorage.clear();

                window.location.replace("/");

            }

        };


        checkActiveSession();

        const interval =
            setInterval(
                checkActiveSession,
                5000
            );


        return () => {
            clearInterval(interval);
        };

    }, []);


    return null;
}


// =====================================================
// APP
// =====================================================

export default function App() {

    return (

        <Router>

            <SessionGuard />

            <Routes>


                {/* =================================================
                    NORMAL USER LOGIN
                ================================================= */}

                <Route
                    path="/"
                    element={<LoginPage />}
                />


                {/* =================================================
                    ADMIN LOGIN
                    COMPLETELY SEPARATE
                ================================================= */}

                <Route
                    path="/admin"
                    element={<AdminLoginPage />}
                />


                {/* =================================================
                    ADMIN DASHBOARD
                    COMPLETELY SEPARATE FROM USER SECURITY
                ================================================= */}

                <Route
                    path="/admin/dashboard"
                    element={<AdminDashboard />}
                />
                <Route
  path="/admin/sessions"
  element={<AdminSessionsPage />}
/>

                {/* =================================================
                    DUPLICATE TAB
                ================================================= */}

                <Route
                    path="/duplicate-tab"
                    element={<DuplicateTabPage />}
                />


                {/* =================================================
                    IMPORTANT:
                    NORMAL USER LOGIN KE BAAD
                    SABSE PEHLE YAHAN AAYEGA

                    /competition-security

                    Is route ko AuthenticatedSecurityLayout
                    ke BAHAR rakha gaya hai.
                ================================================= */}

                <Route
                    path="/competition-security"
                    element={
                        <ProtectedRoute>
                            <CompetitionSecurityPage />
                        </ProtectedRoute>
                    }
                />


                {/* =================================================
                    CAMERA SECURITY KE BAAD NORMAL USER AREA
                ================================================= */}

                <Route
                    element={
                        <AuthenticatedSecurityLayout />
                    }
                >

                    {/* =================================================
                        USER DASHBOARD
                    ================================================= */}

                    <Route
                        path="/dashboard"
                        element={
                            <ProtectedRoute>
                                <DashboardPage />
                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        COMPETITION INSTRUCTIONS
                    ================================================= */}

                    <Route
                        path="/competition/:id"
                        element={
                            <ProtectedRoute>
                                <CompetitionInstructions />
                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        RESULT
                    ================================================= */}

                    <Route
                        path="/result/:id"
                        element={
                            <ProtectedRoute>
                                <ResultPage />
                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        COMPETITION PLAY
                    ================================================= */}

                    <Route
                        path="/competition/play/:id"
                        element={
                            <ProtectedRoute>

                                <GameSecurityWrapper>

                                    <CompetitionPage />

                                </GameSecurityWrapper>

                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        QUIZ
                    ================================================= */}

                    <Route
                        path="/Quiz/:id"
                        element={
                            <ProtectedRoute>

                                <GameSecurityWrapper>

                                    <QuizPage />

                                </GameSecurityWrapper>

                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        QUIZ DASHBOARD
                    ================================================= */}

                    <Route
                        path="/quiz-dashboard"
                        element={
                            <ProtectedRoute>
                                <QuizDashboard />
                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        WORD DASHBOARD
                    ================================================= */}

                    <Route
                        path="/word-dashboard"
                        element={
                            <ProtectedRoute>
                                <WordDashboard />
                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        PUZZLE
                    ================================================= */}

                    <Route
                        path="/puzzle"
                        element={
                            <ProtectedRoute>

                                <GameSecurityWrapper>

                                    <PuzzlePage />

                                </GameSecurityWrapper>

                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        WORD GAME
                    ================================================= */}

                    <Route
                        path="/word"
                        element={
                            <ProtectedRoute>

                                <GameSecurityWrapper>

                                    <WordGamePage />

                                </GameSecurityWrapper>

                            </ProtectedRoute>
                        }
                    />


                    {/* =================================================
                        SCRAMBLE
                    ================================================= */}

                    <Route
                        path="/scrumble"
                        element={
                            <ProtectedRoute>

                                <GameSecurityWrapper>

                                    <ScramblePage />

                                </GameSecurityWrapper>

                            </ProtectedRoute>
                        }
                    />

                </Route>

            </Routes>

        </Router>
    );
}