import { useCallback, useEffect, useState } from "react"
import api from "../../api/ChatsApi";
import type ChatMessage from "../../data-model/ChatMessage";
import { useSignalR } from "./SignalRContext";

type UseChatMessagesReturn = [
    ChatMessage[], // messages
    (text:string, chatId:string) => Promise<void>, // send message
]
export default function useChatMessages(chatId:string):UseChatMessagesReturn {
    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const { connection } = useSignalR();

    useEffect(()=>{
        api.fetchMessages(chatId).then((result)=>{
            setMessages(result);
        }).catch((error)=>{
            console.error(error);
        });
    }, []);

    useEffect(()=>{
        if(!connection){
            console.error("no connection established");
            return;
        }

        // add handlers
        const handleNewMessage = (message:ChatMessage) => {
            if(message.chatId != chatId) return; // skip if for other chat
            console.log("new message received", message);
            setMessages(prevMessages => [...prevMessages, message]);
        }

        connection.on("receive-message", handleNewMessage);

        // remove handler on unmount
        return () => {
            connection.off("receive-message", handleNewMessage);
        }
    }, [connection]);

    const sendMessage = useCallback( async (content:string, chatId:string) => {
            if(!connection){
                console.error("no connection established");
                return;
            }
            connection.send("send-message", { content, chatId });
    }, [connection]);

    return [ messages, sendMessage ]; 
}