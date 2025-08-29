import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AvatarSize, ChatImage } from "../shared/Avatars";
import { Block } from "../shared/Blocks";
import api from "../../api/ChatsApi";
import type ChatHeader from "../../data-model/ChatHeader";
import DefaultPageLayout from "../shared/DefaultPageLayout";

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

    return <DefaultPageLayout title="Chat list">
        <ChatsList onSelected={handleChatSelected} items={items} />
    </DefaultPageLayout>
}

interface ChatsListProps {
    items: ChatHeader[]
    onSelected: (id: string) => void
}
function ChatsList({ items, onSelected }: ChatsListProps) {
    return items.map((item, key) => <ChatListItem key={key}
        title={item.title}
        id={item.id}
        imageId={item.imageId}
        newMessages={item.newMessagesCount}
        onSelected={onSelected} />
    )
}

interface ChatListItemProps {
    title: string,
    newMessages: number,
    imageId: string,
    id: string,
    onSelected: (id: string) => void
}
function ChatListItem({ title, newMessages, imageId, id, onSelected }: ChatListItemProps) {
    return <div onClick={() => onSelected(id)}>
        <Block className="mx-3 mt-3 flex items-center space-x-3 transition 
            ease-in-out duration-150 hover:bg-on-surface">
            <ChatImage size={AvatarSize.Small} imageId={imageId} />
            <p>{title}</p>
            {newMessages > 0 &&
                <p className="bg-white p-1 rounded text-black">{newMessages}</p>
            }
        </Block>
    </div>
}