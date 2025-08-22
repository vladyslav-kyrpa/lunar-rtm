import { useEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom"
import NotFoundPage from "../NotFoundPage";
import LoadingScreen from "../../shared/LoadingScreen";
import { IconButton } from "../../shared/Buttons";
import { HiddenTextBox } from "../../shared/TextBoxes";
import SendIcon from "../../../assets/icons/send.svg";
import type ChatMessage from "../../../data-model/ChatMessage";
import ChatDetailsPage from "./ChatDetailsPage";
import EditChatPage from "./EditChatPage";
import { ChatProvider, useChatContext } from "./ChatContext";
import { useAuth } from "../../hooks/AuthContext";

export default function ChatPage() {
    const { id } = useParams();
    if (!id) return <NotFoundPage />

    return <ChatProvider chatId={id}>
        <ChatPageContent />
    </ChatProvider>
}

function ChatPageContent() {
    const { chat, messages, sendMessage } = useChatContext();
    const { username: currentUsername } = useAuth();
    const [text, setText] = useState("");
    const [showDetails, setShowDetails] = useState(false);
    const [isEditMode, setIsEditMode] = useState(false);

    const scrollRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        if (chat) scrollPageDown();
    }, [chat]);

    const scrollPageDown = () => {
        console.log("Scroll down");
        scrollRef?.current?.scrollIntoView({ 'behavior': "smooth" });
    }

    const handleOnSend = () => {
        sendMessage(text).then(() => setText(""));
    }

    const toggleEdit = () => setIsEditMode(!isEditMode);
    const toggleShowDetails = () => setShowDetails(!showDetails);

    if (chat === null) return <LoadingScreen />
    if (isEditMode) return <EditChatPage onClose={toggleEdit} onSubmit={toggleEdit} />
    if (showDetails) return <ChatDetailsPage onClose={toggleShowDetails} onEdit={toggleEdit} />

    return <div className="flex flex-col h-screen">
        <div className="sticky top-0 h-16 flex items-center justify-center bg-surface"
            onClick={toggleShowDetails}>
            <p className="font-bold">{chat.title}</p>
        </div>

        <div className="flex flex-col flex-1 p-2 overflow-y-auto">
            {messages.map((item, key) => <MessageItem item={item} key={key}
                isIncoming={currentUsername === item.sender} />)}
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
    isIncoming: boolean
}
function MessageItem({ item, isIncoming }: MessageItemProps) {
    return <div className={"flex flex-row m" + (!isIncoming ? "e-auto" : "s-auto")}>
        <div className="bg-surface p-4 mb-2 rounded border border-surface-outline">
            <div className="font-bold mb-1"> from
                {!isIncoming ? " @" + item.sender + ":" : " You:"}
            </div>
            {item.content}
            <div className="text-minor-text text-end mt-1">at {item.timestamp}</div>
        </div>
    </div>
}