import { createContext, useContext, useState, type ReactNode } from "react"
import api from "../../api/AuthApi";

type AuthContextType = {
    username: string | null;
    logIn: (username: string, password: string) => Promise<void>;
    logOut: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context)
        throw Error("Context should be initialized invide AuthProvider")
    return context;
}

interface AuthProviderProps {
    children: ReactNode;
}
export function AuthProvider({ children }: AuthProviderProps) {
    const [username, setUsername] = useState(getCurrentUsername());

    const onLogIn = async (login: string, password: string): Promise<void> => {
        await api.login(login, password);
        setCurrentUsername(login);
        setUsername(login);
    }

    const onLogOut = async (): Promise<void> => {
        await api.logout();
        setCurrentUsername("");
        setUsername(null);
    }

    return (
        <AuthContext.Provider value={{ username, logIn: onLogIn, logOut: onLogOut }}>
            {children}
        </AuthContext.Provider>
    );
}

function getCurrentUsername() {
    return localStorage.getItem("current-username");
}

function setCurrentUsername(value:string) {
    return localStorage.setItem("current-username", value);
}