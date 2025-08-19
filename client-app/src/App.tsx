import { Outlet, Route, Routes } from "react-router-dom"
import NotFoundPage from "./components/pages/NotFoundPage"
import LoginPage from "./components/pages/LoginPage"
import RegisterPage from "./components/pages/RegisterPage"
import ChatsListPage from "./components/pages/ChatsListPage"
import MainLayout from "./components/shared/MainLayout"
import ChatPage from "./components/pages/ChatPage"
import ProfilePage from "./components/pages/ProfilePage"
import NewChatPage from "./components/pages/NewChatPage"
import ProtectedRoute from "./components/shared/ProtectedRoute"

const MainLayoutWrapper = () => (
  <MainLayout>
    <Outlet />
  </MainLayout>
);

const ProtectedRouteWrapper = () => (
  <ProtectedRoute>
    <Outlet />
  </ProtectedRoute>
);

export default function App() {
  return <Routes>
    <Route path="/log-in" element={<LoginPage />} />
    <Route path="/register" element={<RegisterPage />} />
    <Route element={<ProtectedRouteWrapper/>}>
      <Route element={<MainLayoutWrapper/>}>
        <Route path="/chat/:id" element={<ChatPage />}/>
        <Route path="/profile/:username" element={<ProfilePage />} />
        <Route path="/" element={<ChatsListPage />} />
        <Route path="/create-chat" element={<NewChatPage/>} />
        <Route path="/chats" element={<ChatsListPage />} />
      </Route>
    </Route>
    <Route path="*" element={<NotFoundPage />} />
  </Routes>
}