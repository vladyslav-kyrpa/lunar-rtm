import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import TextBox from "../shared/TextBox";
import ActiveButton from "../shared/ActiveButton";
import AvatarImage from "../shared/AvatarImage";
import SearchIcon from "../../assets/icons/search.svg";
import Placeholder from "../../assets/img-placeholder.jpg"

export default function ChatsListPage() {
    const [items, setItems] = useState<ChatHeader[]>([]);
    const [searchValue, setSearchValue] = useState("");
    const navigate = useNavigate();

    const handleChatSelected = (id: string) => {
        navigate("/chat/" + id)
    }

    const handleSearch = (value: string) => {
        setSearchValue(value);
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
        <div className="p-3 rounded bg-surface sticky top-16">
            <SearchBar onSearch={handleSearch} />
        </div>
        <ChatsList
            onSelected={handleChatSelected}
            items={items.filter(e => e.title.includes(searchValue))} />
    </>
}

async function fetchChats(): Promise<ChatHeader[]> {
    const items = getList();

    const delay = new Promise<void>(resolve => {
        setTimeout(() => {
            resolve();
        }, 2000);
    });
    await delay;

    return items;
}

interface SearchBarProps {
    onSearch: (value: string) => void
}
function SearchBar({ onSearch }: SearchBarProps) {
    const [searchValue, setSearchValue] = useState('');

    return <div className="flex flex-row 
        justify-center items-center" onClick={() => { }}>
        <TextBox value={searchValue} style="me-2"
            onChange={(e) => setSearchValue(e)}
            placeholder="Search by title..." />
        <ActiveButton onClick={() => onSearch(searchValue)}>
            <img src={SearchIcon} alt="search" width={24} height={24} />
        </ActiveButton>
    </div>
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
    return <div className="flex items-center space-x-3 mt-3 mx-3 p-3 
            transition duration-200 ease-in-out hover:bg-on-surface rounded"
        onClick={() => onSelected(id)}>
        <AvatarImage size="medium" iconUrl={iconUrl} />
        <p>{title}</p>
        {newMessages > 0 &&
            <p className="bg-white p-1 rounded text-black">{newMessages}</p>
        }
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