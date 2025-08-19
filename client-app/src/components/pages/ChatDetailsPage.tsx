import { useState } from "react";
import type Chat from "../../data-model/Chat";
import { Block, OnSurfaceBlock } from "../shared/Blocks";
import { IconButton, MinorButton } from "../shared/Buttons";
import { FormTextBox } from "../shared/TextBoxes";
import type UserHeader from "../../data-model/UserHeader";
import { useNavigate } from "react-router-dom";

import AvatarImage from "../shared/AvatarImage";
import EditIcon from "../../assets/icons/edit.svg";
import CloseIcon from "../../assets/icons/close.svg";

interface ChatDetailsPageProps {
    chat: Chat;
    onClose: () => void;
    onEdit: () => void;
    onAddMember: (username: string) => void;
}

export default function ChatDetailsPage({ chat, onClose, onEdit, onAddMember }: ChatDetailsPageProps) {
    const [usernameText, setUsernameText] = useState("");

    return <div className="h-screen flex flex-col p-3">
        <Block>
            <div className="flex flex-row items-start mb-5">
                <AvatarImage imgUrl={"/api/images/chat-avatar/" + chat.imageId} size="large"></AvatarImage>
                <div className="ms-4 me-auto">
                    <div className="flex mb-1">
                        <p className="me-2 text-2xl">{chat.title}</p>
                        <IconButton inverted={true} onClick={onEdit} iconSrc={EditIcon} />
                    </div>
                    <div className="">Members {chat.members.length}</div>
                </div>
                <IconButton inverted={true} onClick={onClose} iconSrc={CloseIcon} />
            </div>
            <p>{chat.description}</p>
        </Block>
        <p className="text-center font-bold my-4">Members</p>
        <Block className="flex flex-col overflow-y-auto">
            <OnSurfaceBlock className="flex mb-1">
                <FormTextBox className="flex-1" placeholder="Enter username" value={usernameText} onChange={setUsernameText} />
                <MinorButton onClick={() => onAddMember(usernameText)}>
                    Invite
                </MinorButton>
            </OnSurfaceBlock>
            {chat.members.map((user, key) => <UserListItem user={user} key={key}></UserListItem>)}
        </Block>
    </div>
}

interface UserListItemProps {
    user: UserHeader,
}

function UserListItem({ user }: UserListItemProps) {
    const navigate = useNavigate();
    const handleOnClick = () => {
        navigate("/profile/" + user.username);
    }
    return <div onClick={handleOnClick} className="mb-1">
        <OnSurfaceBlock className="mt-1 flex items-center hover:bg-on-surface-outline">
            <AvatarImage size="small" imgUrl={"/api/images/profile-avatar/" + user.imageId}></AvatarImage>
            <p className="ms-2">{user.displayName} (@{user.username})</p>
        </OnSurfaceBlock>
    </div>
}

