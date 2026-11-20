import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import './index.css'
import App from './App.tsx'
import AuthPage from './AuthPage.tsx'
import Dashboard from './Dashboard.tsx'
import CreatePage from './CreatePage.tsx'
import WantedPage from './WantedPage.tsx'
import CommunityPage from './CommunityPage.tsx'
import MyPostsPage from './MyPostsPage.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<App />} />
        <Route path="/login" element={<AuthPage mode="login" />} />
        <Route path="/register" element={<AuthPage mode="register" />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/create" element={<CreatePage />} />
        <Route path="/wanted-items" element={<WantedPage />} />
        <Route path="/community" element={<CommunityPage />} />
        <Route path="/my-posts" element={<MyPostsPage />} />
      </Routes>
    </BrowserRouter>
  </StrictMode>,
)
