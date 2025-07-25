import { useState } from "react"
import TextBox from "../shared/TextBox"
import ActiveButton from "../shared/ActiveButton";
import { useNavigate } from "react-router-dom";
import AvatarImage from "../shared/AvatarImage";

export default function ChatsListPage() {
    const navigate = useNavigate();
    const handleChatSelected = (id: string) => {
        navigate("/chat/" + id)
    }
    const items = getList();
    const [selectedItems, setSelectedItems] = useState(items);
    const handleSearch = (value: string) => {
        setSelectedItems(items.filter(e => e.title.includes(value)))
    }

    return <div className="">
        <div className="sticky top-0">
            <SearchBar onSearch={handleSearch} />
        </div>
        <ChatsList onSelected={handleChatSelected} items={selectedItems} />
    </div>
}

interface SearchBarProps {
    onSearch: (value: string) => void
}
function SearchBar({ onSearch }: SearchBarProps) {
    const [searchValue, setSearchValue] = useState('');
    return <div className="flex flex-row 
        justify-center items-center p-2 bg-surface" onClick={() => { }}>
        <TextBox value={searchValue} style="me-2"
            onChange={(e) => setSearchValue(e)}
            placeholder="Search by title..." />
        <ActiveButton onClick={() => onSearch(searchValue)}>Search</ActiveButton>
    </div>
}

interface ChatsListProps {
    items: ChatHeader[]
    onSelected: (id: string) => void
}
function ChatsList({ items, onSelected }: ChatsListProps) {
    return items.map((item, _) => {
        return <ChatListItem title={item.title}
            id={item.id} iconUrl={item.imageUrl}
            newMessages={item.newMessages}
            onSelected={onSelected} />
    })
}

interface ChatListItemProps {
    title: string,
    newMessages: number,
    iconUrl: string,
    id: string,
    onSelected: (id: string) => void
}
function ChatListItem({ title, newMessages, iconUrl, id, onSelected }: ChatListItemProps) {
    return <div className="flex items-center space-x-3 mt-2 mx-2 p-3
        bg-surface rounded-2xl" onClick={() => onSelected(id)}>
        <AvatarImage size="medium" iconUrl={iconUrl} />
        <p>{title}</p><p className="bg-active p-1 rounded text-black">{newMessages}</p>
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
            imageUrl: "/"
        },
        {
            title: "1",
            id: "2",
            newMessages: 12,
            imageUrl: "/"
        },
        {
            title: "1",
            id: "3",
            newMessages: 12,
            imageUrl: "/"
        },
        {
            title: "1",
            id: "4",
            newMessages: 12,
            imageUrl: "/"
        },
        {
            title: "12",
            id: "5",
            newMessages: 12,
            imageUrl: "/"
        },
    ];
    return chats
}