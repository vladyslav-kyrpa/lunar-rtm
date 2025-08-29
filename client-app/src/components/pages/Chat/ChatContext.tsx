import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type Chat from "../../../data-model/Chat";
import api from "../../../api/ChatsApi";
import type ChatMessage from "../../../data-model/ChatMessage";
import useChatMessages from "../../hooks/UseChatMessages";

type UseChatReturn = {
    chat: Chat | null,
    messages: ChatMessage[],
    update: (title: string, description: string) => Promise<void>,
    updateImage: (image: File) => Promise<void>
    delete: () => Promise<void>,
    leave: () => Promise<void>,
    addMember: (username: string) => Promise<void>,
    removeMember: (username: string) => Promise<void>,
    sendMessage: (content: string) => Promise<void>,
    promoteMember: (username: string, role: string) => Promise<void>
};

const ChatContext = createContext<UseChatReturn | null>(null);

interface ChatProviderProps {
    chatId: string,
    children: ReactNode
}
export function ChatProvider({ chatId, children }: ChatProviderProps) {
    const chatState = useChat(chatId);
    return <ChatContext.Provider value={chatState}>
        {children}
    </ChatContext.Provider>
}

export function useChatContext() {
    const context = useContext(ChatContext);
    if (!context) throw new Error("useChatContext should be inside ChatProvider");
    return context;
}

export function useChat(id: string): UseChatReturn {
    const [chat, setChat] = useState<Chat | null>(null);
    const [messages, sendMessage] = useChatMessages(id);

    useEffect(() => {
        fetchChat();
    }, []);

    const fetchChat = () => {
        api.fetchChat(id)
            .then((chat) => setChat(chat))
            .catch((error) => console.log(error));
    } 

    const onUpdate = async (title: string, description: string) => {
        api.updateChat(id, title, description).then((result) => {
            console.log(result);
            return fetchChat();
        }).then((result)=>{
            console.log(result);
        }).catch((error) => {
            console.error(error);
        });
    }

    const onAddMember = async (username: string) => {
        api.inviteUser(id, username).then((result) => {
            console.log(result);
            return fetchChat();
        }).then((result)=>{
            console.log(result);
        }).catch((error) => {
            console.error(error);
        });
    }

    const onRemoveMember = async (username: string) => {
        api.removeUser(id, username).then((result) => {
            console.log(result);
            return fetchChat();
        }).then((result)=>{
            console.log(result);
        }).catch((error) => {
            console.error(error);
        });
    }

    const onLeave = async () => {
        api.leave(id).then((result) => {
            console.log(result);
        }).catch((error) => {
            console.error(error);
        });
    }

    const onDelete = async () => {
        api.deleteChat(id)
            .then((_) => setChat(null))
            .catch((error) => console.error(error));
    }

    const onUpdateImage = async (image: File) => {
        api.updateImage(id, image)
            .then((result) => console.log(result))
            .catch((error) => console.error(error));
    }

    const onSendMessage = async (content: string) => {
        sendMessage(content, id).then((result) => {
            console.log(result);
        }).catch((error) => {
            console.error(error);
        });
    }

    const onPromoteMember = async (username: string, role: string) => {
        api.promoteUser(username, role, id).then((result)=>{
            console.log(result);
            return fetchChat();
        }).then((result)=>{
            console.log(result);
        }).catch((error)=>{
            console.error(error);
        });
    }

    return {
        chat, messages,
        sendMessage: onSendMessage,
        update: onUpdate,
        addMember: onAddMember,
        removeMember: onRemoveMember,
        delete: onDelete,
        leave: onLeave,
        updateImage: onUpdateImage,
        promoteMember: onPromoteMember
    };
}