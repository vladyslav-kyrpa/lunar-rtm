import * as signalR from "@microsoft/signalr";
import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { useAuth } from "./AuthContext";

type SignalRContextType = {
    connection: signalR.HubConnection | null;
}

const SignalRContext = createContext<SignalRContextType>({connection : null});

export const useSignalR = () => {
    const context = useContext(SignalRContext);
    if(!context) {
        throw Error("must be initialised invide SignalRProvider")
    }
    return context;
}

interface SignalRProviderProps {
    children: ReactNode
}
export const SignalRProvider = ({children}:SignalRProviderProps) => {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null); 
    const { username } = useAuth();

    useEffect(()=>{
        if(!username) {
            // cleanu up on logout
            connection?.stop();
            setConnection(null);
            return;
        }

        // new connection
        const builder = new signalR.HubConnectionBuilder()
            .withUrl("/api/chat-hub", {
                withCredentials: true
            })
            .withAutomaticReconnect()
            .build();
        setConnection(builder);

        builder.start().then(()=>{
            console.log("Connection established");
        }).catch((e)=>{
            console.error("Failed to establish connection: ", e);
        });

        return () => {
            builder.stop();
        }
        
    },[username]);

    return <SignalRContext.Provider value={{connection}}>
        {children}
    </SignalRContext.Provider>
}
