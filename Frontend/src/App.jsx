import { lazy, Suspense } from 'react'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { StudentAuthProvider } from './auth/StudentAuth'
import { DuplicateTabPage, MonitoredArea, RequireStudent, StudentLayout } from './student/StudentShell'
import LoginPage from './student/LoginPage'
import AppealPage from './student/AppealPage'
import PrecheckPage from './student/PrecheckPage'
import DashboardPage, { SectionPage } from './student/DashboardPage'
import GamePlayPage from './games/GamePlayPage'
import { QuizPlayPage, QuizResultPage } from './student/QuizPages'
import { Spinner } from './ui/kit'

// The admin panel is a separate bundle: students never download it.
const AdminApp = lazy(() => import('./admin/AdminApp'))

function StudentApp() {
  return (
    <StudentAuthProvider>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/appeal" element={<AppealPage />} />
        <Route path="/duplicate-tab" element={<DuplicateTabPage />} />
        <Route element={<RequireStudent />}>
          <Route path="/precheck" element={<PrecheckPage />} />
          <Route element={<MonitoredArea />}>
            <Route element={<StudentLayout />}>
              <Route path="/dashboard" element={<DashboardPage />} />
              <Route path="/section/:key" element={<SectionPage />} />
              <Route path="/quiz/result/:attemptId" element={<QuizResultPage />} />
            </Route>
            <Route path="/play/:gameKey" element={<GamePlayPage />} />
            <Route path="/quiz/:quizId" element={<QuizPlayPage />} />
          </Route>
        </Route>
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </StudentAuthProvider>
  )
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/admin/*" element={<Suspense fallback={<Spinner full />}><AdminApp /></Suspense>} />
        <Route path="/*" element={<StudentApp />} />
      </Routes>
    </BrowserRouter>
  )
}
