import { Route, Routes } from 'react-router-dom'
import LgHomePage from './pages/LgHomePage'
import LoginPage from './pages/LoginPage'
import NotFoundPage from './pages/NotFoundPage'
import useWindowWigth from './hooks/useWindowWigth'
import ChatPage from './pages/ChatPage'

function App() {
  const width = useWindowWigth();
  
  return <Routes>
    {width <= 700 
      ? <Route path="/" element={<ChatPage/>}/> 
      : <Route path="/" element={<LgHomePage/>}/>
    }
    <Route path="/login" element={<LoginPage/>}/>
    <Route path="*" element={<NotFoundPage/>}/>
  </Routes>
}

export default App
