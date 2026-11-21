import { Navigate, Route, Routes } from 'react-router-dom'
import AuthPage from './pages/AuthPage'
import CommunityPage from './pages/CommunityPage'
import CreatePage from './pages/CreatePage'
import DashboardPage from './pages/DashboardPage'
import ExplorePage from './pages/ExplorePage'
import ListingDetailPage from './pages/ListingDetailPage'
import MyPostsPage from './pages/MyPostsPage'
import UserProfilePage from './pages/UserProfilePage'
import WantedPage from './pages/WantedPage'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<ExplorePage />} />
      <Route path="/login" element={<AuthPage mode="login" />} />
      <Route path="/register" element={<AuthPage mode="register" />} />
      <Route path="/listings/:id" element={<ListingDetailPage />} />
      <Route path="/users/:id" element={<UserProfilePage />} />
      <Route path="/dashboard" element={<DashboardPage />} />
      <Route path="/create" element={<CreatePage />} />
      <Route path="/my-posts" element={<MyPostsPage />} />
      <Route path="/wanted-items" element={<WantedPage />} />
      <Route path="/community" element={<CommunityPage />} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
