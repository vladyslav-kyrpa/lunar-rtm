import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import './styles/index.css'
import App from './App.tsx'
import { SignalRProvider } from './components/hooks/SignalRContext.tsx';
import { AuthProvider } from './components/hooks/AuthContext.tsx';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <SignalRProvider>
          <App/>
        </SignalRProvider>
      </AuthProvider>
    </BrowserRouter>
  </React.StrictMode>,
)
