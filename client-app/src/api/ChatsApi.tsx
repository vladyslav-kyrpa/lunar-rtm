import type Chat from "../data-model/Chat";
import type ChatHeader from "../data-model/ChatHeader";
import Placeholder from "../assets/img-placeholder.jpg"
import type ChatMessage from "../data-model/ChatMessage";

async function fetchChats(): Promise<ChatHeader[]> {
        const chats: ChatHeader[] = [
        {
            title: "some long text here",
            id: "1",
            newMessages: 12,
            imageUrl: Placeholder
        },
        {
            title: "1",
            id: "2",
            newMessages: 0,
            imageUrl: Placeholder
        },
        {
            title: "some long text here",
            id: "1",
            newMessages: 12,
            imageUrl: Placeholder
        },
        {
            title: "1",
            id: "2",
            newMessages: 0,
            imageUrl: Placeholder
        },
        {
            title: "some long text here",
            id: "1",
            newMessages: 12,
            imageUrl: Placeholder
        },
        {
            title: "1",
            id: "2",
            newMessages: 0,
            imageUrl: Placeholder
        },
        {
            title: "some long text here",
            id: "1",
            newMessages: 12,
            imageUrl: Placeholder
        },
        {
            title: "1",
            id: "2",
            newMessages: 0,
            imageUrl: Placeholder
        },
    ];
    return chats;
}

async function fetchChat(id: string): Promise<Chat> {
    console.log(id);
    const items: Chat = {
        id:id,
        title: "Chat titile",
        imageUrl: "",
        description: "some text here",
        members: [
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
        ]
    };
    return items;
}

async function fetchMessages(id:string): Promise<ChatMessage[]>{
    const messages: ChatMessage[] = [
        { sender: "1", date: "2025-08-21 12:20", content: "text message" },
        { sender: "2", date: "2025-08-21 12:20", content: "text message" },
        { sender: "2", date: "2025-08-21 12:20", content: "text messageghjkgkhjg hj gjkg hjk g" },
        { sender: "1", date: "2025-08-21 12:20", content: "text message" },
        { sender: "2", date: "2025-08-21 12:20", content: "text message gkh gk j ghjk jhg j" },
        { sender: "1", date: "2025-08-21 12:20", content: "text message" },
        { sender: "2", date: "2025-08-21 12:20", content: "text message" },
        { sender: "1", date: "2025-08-21 12:20", content: "text message" },
        { sender: "2", date: "2025-08-21 12:20", content: "text message" },
    ];
    return messages;
}

async function createChat(title: string, description: string, image?: File) {
    console.log("create chat");
}

async function updateChat(id:string, title?: string, description?: string, image?: File|null) {
    console.log("Update conversation");
}

async function inviteUser(id: string, username: string) {
    console.log("invite user");
}

export default { fetchChats, fetchChat, fetchMessages, createChat, updateChat, inviteUser }