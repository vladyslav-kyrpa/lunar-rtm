import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom"
import NotFoundPage from "./NotFoundPage";
import LoadingScreen from "../shared/LoadingScreen";
import { IconButton } from "../shared/Buttons";
import { HiddenTextBox } from "../shared/TextBoxes";

import SendIcon from "../../assets/icons/send.svg";

import type ChatMessage from "../../data-model/ChatMessage";
import type Chat from "../../data-model/Chat";

import api from "../../api/ChatsApi";
import ChatDetailsPage from "./ChatDetailsPage";
import EditChatPage from "./EditChatPage";

export default function ChatPage() {
    const [chat, setChat] = useState<Chat | null>(null);
    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [text, setText] = useState("");

    const [showDetails, setShowDetails] = useState(false);
    const [isEditMode, setIsEditMode] = useState(false);

    const navigate = useNavigate();
    const { id } = useParams();
    const scrollRef = useRef<HTMLDivElement | null>(null);

    // TODO: just for development
    const currentUserId = "2";

    if (!id) {
        return <NotFoundPage />;
    }

    useEffect(() => {
        api.fetchChat(id).then((result) => {
            setChat(result);
            return api.fetchMessages(id);
        }).then((result) => {
            setMessages(result);
        }).catch((error) => {
            console.log(error);
        });
    }, [id]);

    useEffect(() => {
        if (chat) {
            scrollPageDown();
        }
    }, [chat]);

    const scrollPageDown = () => {
        console.log("Scroll down");
        scrollRef?.current?.scrollIntoView({ 'behavior': "smooth" });
    }

    const handleOnSend = () => {
        console.log("Send message...")
    }

    const handleOnAddMember = (username: string) => {
        api.inviteUser(id, username).then((result)=>{
            console.log(result);
        }).catch((error)=>{
            console.error(error);
        });
    }

    const toggleEdit = () => {
        setIsEditMode(!isEditMode);
    }

    const toggleShowDetails = () => {
        setShowDetails(!showDetails);
    }

    const handleOnSave = () => {
        navigate("")
        setIsEditMode(false);
    }

    if (chat === null) {
        return <LoadingScreen />
    }
    if (isEditMode) {
        return <EditChatPage chat={chat} onClose={toggleEdit} onSubmit={handleOnSave} />
    }
    if (showDetails) {
        return <ChatDetailsPage chat={chat}
            onClose={toggleShowDetails}
            onEdit={toggleEdit}
            onAddMember={handleOnAddMember} />
    }
    return <div className="flex flex-col h-screen">
        <div className="sticky top-0 h-16 flex items-center justify-center bg-surface"
            onClick={toggleShowDetails}>
            <p className="font-bold">{chat.title}</p>
        </div>

        <div className="flex flex-col flex-1 p-2 overflow-y-auto">
            {messages.map((item, key) =>
                <MessageItem item={item} key={key} isIncoming={currentUserId === item.sender} />)
            }
            <div id="list-bottom-pointer" ref={scrollRef} />
        </div>

        <div className="flex flex-row m-2 p-2 rounded border bg-surface border-surface-outline">
            <HiddenTextBox className="w-full" placeholder="Type message..." onChange={(value) => setText(value)} value={text} />
            <IconButton inverted={true} iconSrc={SendIcon} onClick={handleOnSend}></IconButton>
        </div>
    </div>
}

interface MessageItemProps {
    item: ChatMessage,
    key: number,
    isIncoming: boolean
}
function MessageItem({ item, key, isIncoming }: MessageItemProps) {
    return <div className={"flex flex-row m" + (isIncoming ? "e-auto" : "s-auto")} key={key}>
        <div className="bg-surface p-4 mb-2 rounded border border-surface-outline">
            <div className="font-bold mb-1"> from
                {isIncoming ? " @" + item.sender + ":" : " You:"}
            </div>
            {item.content}
            <div className="text-minor-text text-end mt-1">at {item.timestamp}</div>
        </div>
    </div>
}