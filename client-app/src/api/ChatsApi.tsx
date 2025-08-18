import type Chat from "../data-model/Chat";
import type ChatHeader from "../data-model/ChatHeader";
import type ChatMessage from "../data-model/ChatMessage";

async function fetchChats(): Promise<ChatHeader[]> {
    const result = await fetch("/api/chats");
    if(result.status === 200){
        // todo: check structure
        return (await result.json()) as ChatHeader[]; 
    } else {
        throw Error("Failed to fetch chats " + result.statusText);
    }
}

async function fetchChat(id: string): Promise<Chat> {
    console.log(id);
    const result = await fetch("/api/chats/" + id);
    if(result.status === 200){
        return (await result.json()) as Chat;
    } else {
        throw Error("Failed to fetch chat: " + result.statusText)
    }
}

async function fetchMessages(id:string): Promise<ChatMessage[]>{
    const result = await fetch("/api/chats/" + id + "/messages");
    if(result.ok){
        return (await result.json()) as ChatMessage[];
    } else {
        throw Error("Failed to get messages: " + result.statusText);
    }
}

async function createChat(title: string, description: string) {
    const result = await fetch("/api/chats/create", {
        method:"POST",
        headers: {
            "Content-Type":"application/json",
        },
        body: JSON.stringify({title, description, type:3}) // TODO: add type selection
    });
    if(!result.ok){
        throw Error("Failed to create chat: " + result.statusText);
    }
}

async function updateChat(id:string, title?: string, description?: string) {
    const result = await fetch("/api/chats/" + id + "/update", {
        method:"POST",
        headers: {
            "Content-Type":"application/json",
        },
        body: JSON.stringify({title, description})
    });
    if(!result.ok){
        throw Error("Failed to update chat: " + result.statusText);
    }
}

async function inviteUser(id: string, username: string) {
    const result = await fetch("/api/chats/" + id + "/add-member", {
        method:"POST",
        headers: {
            "Content-Type":"application/json"
        },
        body: JSON.stringify({username})
    });
    if(!result.ok){
        throw Error("Failed to add chat member: " + result.statusText);
    }
}

async function updateImage(id:string, image:File) {
    const formData = new FormData();
    formData.append("image", image);
    const result = await fetch("/api/chats/" + id + "/update-image", {
        method:"POST",
        body: formData 
    });
    if(!result.ok){
        throw Error("Failed to update chat image: " + result.statusText);
    }

}

export default { fetchChats, fetchChat, fetchMessages, createChat, updateChat, updateImage, inviteUser }