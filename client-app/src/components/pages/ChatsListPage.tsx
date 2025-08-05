import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import AvatarImage from "../shared/AvatarImage";
import { Block } from "../shared/Blocks";
import api from "../../api/ChatsApi";
import type ChatHeader from "../../data-model/ChatHeader";

export default function ChatsListPage() {
    const [items, setItems] = useState<ChatHeader[]>([]);
    const navigate = useNavigate();

    const handleChatSelected = (id: string) => {
        navigate("/chat/" + id)
    }

    useEffect(() => {
        api.fetchChats().then((result) => {
            console.log(result);
            setItems(result);
        }).catch((error) => {
            console.log(error);
        })
    }, []);

    return <>
        <div className="sticky top-0 h-16 flex items-center justify-center bg-surface">
            <p className="font-bold">Chats list</p>
        </div>
        <ChatsList onSelected={handleChatSelected} items={items} />
    </>
}

interface ChatsListProps {
    items: ChatHeader[]
    onSelected: (id: string) => void
}
function ChatsList({ items, onSelected }: ChatsListProps) {
    return items.map((item, key) => <ChatListItem key={key}
        title={item.title}
        id={item.id}
        iconUrl={item.imageUrl}
        newMessages={item.newMessages}
        onSelected={onSelected} />
    )
}

interface ChatListItemProps {
    title: string,
    newMessages: number,
    iconUrl: string,
    id: string,
    onSelected: (id: string) => void
}
function ChatListItem({ title, newMessages, iconUrl, id, onSelected }: ChatListItemProps) {
    return <div onClick={() => onSelected(id)}>
        <Block className="mx-3 mt-3 flex items-center space-x-3 transition 
            ease-in-out duration-150 hover:bg-on-surface">
            <AvatarImage size="medium" imgUrl={iconUrl} />
            <p>{title}</p>
            {newMessages > 0 &&
                <p className="bg-white p-1 rounded text-black">{newMessages}</p>
            }
        </Block>
    </div>
}