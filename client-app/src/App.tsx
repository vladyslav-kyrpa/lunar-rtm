import { Route, Routes } from "react-router-dom"
import NotFoundPage from "./components/pages/NotFoundPage"
import LoginPage from "./components/pages/LoginPage"
import RegisterPage from "./components/pages/RegisterPage"
import ChatsListPage from "./components/pages/ChatsListPage"
import MainLayout from "./components/shared/MainLayout"
import ChatPage from "./components/pages/ChatPage"
import ProfilePage from "./components/pages/ProfilePage"

function App() {

  // TODO: Proper routing
  // if not Authenticated -> to lending page
  // if authenticated -> to home page (with list of chats)

  return <Routes>
    <Route path="/log-in" element={<LoginPage />} />
    <Route path="/register" element={<RegisterPage />} />
    <Route path="/chat/:id" element={
      <MainLayout>
        <ChatPage />
      </MainLayout>
    } />
    <Route path="/profile/:username" element={
      <MainLayout>
        <ProfilePage />
      </MainLayout>
    } />
    <Route path="/" element={
      <MainLayout>
        <ChatsListPage />
      </MainLayout>
    } />
    <Route path="/chats" element={
      <MainLayout>
        <ChatsListPage />
      </MainLayout>
    } />
    <Route path="*" element={<NotFoundPage />} />
  </Routes>
}

export default App