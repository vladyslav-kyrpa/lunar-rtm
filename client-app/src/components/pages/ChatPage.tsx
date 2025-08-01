import { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom"
import NotFoundPage from "./NotFoundPage";
import SendIcon from "../../assets/icons/send.svg";
import AvatarImage from "../shared/AvatarImage";
import EditIcon from "../../assets/icons/edit.svg";
import CloseIcon from "../../assets/icons/close.svg";
import LoadingScreen from "../shared/LoadingScreen";
import { IconButton, MinorButton } from "../shared/Buttons";
import { FormTextBox, HiddenTextBox } from "../shared/TextBoxes";
import { Block, OnSurfaceBlock } from "../shared/Blocks";

export default function ChatPage() {
    const [chat, setChat] = useState<ChatMessages | null>(null);
    const [text, setText] = useState("");
    const [showDetails, setShowDetails] = useState(false);

    const [isEditMode, setIsEditMode] = useState(false);
    const [newTitle, setNewTitle] = useState("");
    const [newDescription, setNewDescription] = useState("");

    const { id } = useParams();
    const currentUserId = "2";
    const scrollRef = useRef<HTMLDivElement | null>(null);

    if (!id) {
        return <NotFoundPage />;
    }

    useEffect(() => {
        fetchData(id).then((result) => {
            setChat(result);
            setNewTitle(result.title);
            setNewDescription(result.description);
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

    const handleOnEdit = () => {
        setIsEditMode(true);
    }

    const handleOnShowDetails = () => {
        setShowDetails(!showDetails);
    }
    
    const handleOnSave = () => {
        // api call
        console.log("save changes");
        if(chat === null) return;
        // update ui
        const newChat = {...chat, title:newTitle, desctiption:newDescription};
        setChat(newChat);
    }

    const handleOnCancel = () => {
        if(chat === null) return;
        setNewTitle(chat.title);
        setNewDescription(chat.description);
        setIsEditMode(false);
    }

    const handleInput = (value:string, param:string) => {
        if(param === "title")
            setNewTitle(value);
        if(param === "description")
            setNewDescription(value);
    }

    if (chat === null) {
        return <LoadingScreen />
    }
    if(isEditMode){
        return <div className="flex items-center justify-center h-screen">
            <Block className="w-[400px] mt-15 flex flex-col">
                <p className="font-bold text-center mb-5">Edit conversation</p>
                <label className="mb-2">Title</label> 
                <FormTextBox value={newTitle} onChange={(e)=>handleInput(e, "title")}></FormTextBox>
                <label className="mt-5 mb-2">Description</label>
                <FormTextBox value={newDescription} onChange={(e)=>handleInput(e, "description")}></FormTextBox>
                <MinorButton className="mt-5 mb-2 w-full" onClick={handleOnSave}>Save</MinorButton>
                <MinorButton className="w-full" onClick={handleOnCancel}>Cancel</MinorButton>
            </Block>
        </div>
    }
    if (showDetails) {
        return <div className="h-screen w-screen flex flex-col p-3">
            <Block className="mt-15 flex flex-row items-start">
                <AvatarImage imgUrl={chat.imageUrl} size="large"></AvatarImage>
                <div className="ms-4 me-auto">
                    <div className="flex mb-1">
                        <p className="me-2 text-2xl">Title</p>
                        <IconButton inverted={true} onClick={handleOnEdit} iconSrc={EditIcon} />
                    </div>
                    <div className="">Members {chat.members.length}</div>
                </div>
                <IconButton inverted={true} onClick={handleOnShowDetails}
                    iconSrc={CloseIcon} />
            </Block>
            <p className="text-center font-bold my-4">Members</p>
            <Block className="flex flex-col overflow-y-auto">
                {chat.members.map((user, key) => <UserListItem user={user} key={key}></UserListItem>)}
            </Block>
        </div>
    }
    return <div className="flex flex-col h-screen">
        <div className="sticky top-0 h-16 flex items-center justify-center bg-surface"
            onClick={handleOnShowDetails}>
            <p className="font-bold">{chat.title}</p>
        </div>

        <div className="flex flex-col flex-1 p-2 overflow-y-auto">
            {chat.messages.map((item, key) =>
                <MessageItem item={item} key={key} isIncoming={currentUserId === item.sender} />)
            }
            <div ref={scrollRef} /> {/* <- This is key! */}
        </div>
        
        <div className="flex flex-row m-2 p-2 rounded border bg-surface border-surface-outline">
            <HiddenTextBox className="w-full" placeholder="Type message..." onChange={(value) => setText(value)} value={text} />
            <IconButton inverted={true} iconSrc={SendIcon} onClick={handleOnSend}></IconButton> 
        </div>
    </div>
}

interface UserListItemProps {
    user: UserHeader,
    key: number
}
function UserListItem({ user, key }: UserListItemProps) {
    const navigate = useNavigate();
    const handleOnClick = () => {
        navigate("/profile/"+ user.username);
    }
    return <div onClick={handleOnClick} key={key} className="mb-1">
        <OnSurfaceBlock className="mt-1 flex items-center hover:bg-on-surface-outline">
            <AvatarImage size="small" imgUrl={user.imageUrl}></AvatarImage>
            <p className="ms-2">{user.displayName} (@{user.username})</p>
        </OnSurfaceBlock>
    </div>
}

interface MessageItemProps {
    item: Message,
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
            <div className="text-minor-text text-end mt-1">at {item.date}</div>
        </div>
    </div>
}

async function fetchData(id: string): Promise<ChatMessages> {
    console.log(id);
    const items: ChatMessages = {
        title: "Chat titile",
        imageUrl: "",
        description:"some text here",
        messages: [
            { sender: "1", date: "2025-08-21 12:20", content: "text message" },
            { sender: "2", date: "2025-08-21 12:20", content: "text message" },
            { sender: "2", date: "2025-08-21 12:20", content: "text messageghjkgkhjg hj gjkg hjk g" },
            { sender: "1", date: "2025-08-21 12:20", content: "text message" },
            { sender: "2", date: "2025-08-21 12:20", content: "text message gkh gk j ghjk jhg j" },
            { sender: "1", date: "2025-08-21 12:20", content: "text message" },
            { sender: "2", date: "2025-08-21 12:20", content: "text message" },
            { sender: "1", date: "2025-08-21 12:20", content: "text message" },
            { sender: "2", date: "2025-08-21 12:20", content: "text message" },
        ],
        members: [
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
            { username: "1", displayName: "user1", imageUrl: "" },
            { username: "2", displayName: "user2", imageUrl: "" },
        ]
    };
    return items;
}

interface UserHeader {
    username: string,
    displayName: string,
    imageUrl: string
}

interface Message {
    sender: string,
    date: string,
    content: string,
}

interface ChatMessages {
    title: string,
    description: string,
    imageUrl: string
    messages: Message[]
    members: UserHeader[]
}