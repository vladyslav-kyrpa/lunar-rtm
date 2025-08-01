import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import AvatarImage from "../shared/AvatarImage";
import Placeholder from "../../assets/img-placeholder.jpg"
import { Block } from "../shared/Blocks";

export default function ChatsListPage() {
    const [items, setItems] = useState<ChatHeader[]>([]);
    const navigate = useNavigate();

    const handleChatSelected = (id: string) => {
        navigate("/chat/" + id)
    }

    useEffect(() => {
        fetchChats().then((result) => {
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

async function fetchChats(): Promise<ChatHeader[]> {
    const items = getList();
    return items;
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

interface ChatHeader {
    id: string,
    title: string,
    newMessages: number,
    imageUrl: string
}

function getList() {
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