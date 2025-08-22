import { useState } from "react";
import { Block, OnSurfaceBlock } from "../../shared/Blocks";
import { IconButton, MinorButton } from "../../shared/Buttons";
import { FormTextBox } from "../../shared/TextBoxes";
import type UserHeader from "../../../data-model/UserHeader";
import { useNavigate } from "react-router-dom";

import CloseIcon from "../../../assets/icons/close.svg";
import { ChatType } from "../../../data-model/Chat";
import { AvatarSize, ProfileImage } from "../../shared/Avatars";
import { useChatContext } from "./ChatContext";
import { useAuth } from "../../hooks/AuthContext";
import LoadingScreen from "../../shared/LoadingScreen";
import { UpdatableAvatar } from "../../shared/UpdatableAvatar";


interface ChatDetailsPageProps {
    onClose: () => void;
    onEdit: () => void;
}

export default function ChatDetailsPage({ onClose, onEdit }: ChatDetailsPageProps) {
    const [usernameText, setUsernameText] = useState("");
    const { chat, delete: deleteChat, addMember, removeMember, leave, updateImage } = useChatContext();
    const { username } = useAuth();

    const navigate = useNavigate();
    const isOwner = () => chat?.owner.username == username;

    const onDelete = () => {
        deleteChat().then((_) => navigate("/"));
    }

    const onAddMember = () => {
        addMember(usernameText).then((_) => setUsernameText(""));
    }

    const onRemoveMember = (member: string) => {
        removeMember(member).then((_) => navigate(""));
    }

    const onLeave = () => {
        leave().then((_) => navigate("/"));
    }

    const onImageUpdate = async (image: File) => {
        return updateImage(image);
    }

    const typeToString = (type: ChatType): string => {
        console.log("Chat type: ", type)
        if (type == ChatType.MONOLOGUE) return "Monologue";
        if (type == ChatType.DIALOGUE) return "Dialogue";
        if (type == ChatType.POLYLOGUE) return "Polylogue";
        else return "Unknown";
    }

    if (!chat) return <LoadingScreen />

    return <div className="h-screen flex-col p-3 pt-15 w-[500px] mx-auto">
        <Block className="flex flex-col items-center">
            <div className="flex items-end w-full">
                <IconButton inverted={true}
                    iconSrc={CloseIcon}
                    onClick={onClose}
                    className="ms-auto" />
            </div>

            <UpdatableAvatar imageId={chat.imageId}
                type={"chat-avatar"}
                canEdit={isOwner()}
                onSave={onImageUpdate} />
            <p className="me-2 mt-5 text-2xl">{chat.title}</p>
            <p className="text-minor-text mb-5">Members: {chat.members.length}</p>
            <p className="text-minor-text mb-5">type: {typeToString(chat.type)}</p>
            <p className="mb-5">{chat.description}</p>

            {isOwner() ? <div>
                <MinorButton onClick={onEdit} >Edit chat</MinorButton>
                <MinorButton onClick={onDelete} >Delete chat</MinorButton>
            </div> : <div>
                <MinorButton onClick={onLeave} >Leave</MinorButton>
            </div>}
        </Block>

        <p className="text-center font-bold my-4">Members</p>
        <Block className="flex flex-col overflow-y-auto">
            {isOwner() && <OnSurfaceBlock className="flex mb-1">
                <FormTextBox className="flex-1" placeholder="Enter username" value={usernameText} onChange={setUsernameText} />
                <MinorButton onClick={onAddMember}>
                    Invite
                </MinorButton>
            </OnSurfaceBlock>}
            {chat.members.map((user, key) => <UserListItem user={user} key={key}
                canRemove={isOwner() && user.username != username}
                onRemove={() => onRemoveMember(user.username)} />)
            }
        </Block>
    </div>
}

interface UserListItemProps {
    user: UserHeader,
    canRemove: boolean,
    onRemove: () => void
}
function UserListItem({ user, onRemove, canRemove }: UserListItemProps) {
    const navigate = useNavigate();
    const handleOnClick = () => {
        navigate("/profile/" + user.username);
    }
    return <div onClick={handleOnClick} className="mb-1">
        <OnSurfaceBlock className="mt-1 flex items-center hover:bg-on-surface-outline">
            <ProfileImage size={AvatarSize.Small} imageId={user.imageId} />
            <p className="ms-2 me-auto">{user.displayName} (@{user.username})</p>
            {canRemove && <IconButton iconSrc={CloseIcon} onClick={onRemove} inverted={true} />}
        </OnSurfaceBlock>
    </div>
}
