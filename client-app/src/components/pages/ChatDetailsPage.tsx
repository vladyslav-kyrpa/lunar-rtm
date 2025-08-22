import { useState } from "react";
import type Chat from "../../data-model/Chat";
import { Block, OnSurfaceBlock } from "../shared/Blocks";
import { ActiveButton, IconButton, MinorButton } from "../shared/Buttons";
import { FormTextBox } from "../shared/TextBoxes";
import type UserHeader from "../../data-model/UserHeader";
import { useNavigate } from "react-router-dom";
import useNotification from "../hooks/UseNotification";
import ImageUpload from "../shared/ImageUpload";
import BlockNotification from "../shared/BlockNotification";

import api from "../../api/ChatsApi";

import AvatarImage from "../shared/AvatarImage";
import EditIcon from "../../assets/icons/edit.svg";
import CloseIcon from "../../assets/icons/close.svg";
import { getCurrentUsername } from "../../utils/CurrentUser";
import { ChatType } from "../../data-model/Chat";


interface ChatDetailsPageProps {
    chat: Chat;
    onClose: () => void;
    onEdit: () => void;
    onAddMember: (username: string) => void;
}

export default function ChatDetailsPage({ chat, onClose, onEdit, onAddMember }: ChatDetailsPageProps) {
    const [usernameText, setUsernameText] = useState("");
    const isOwner = chat.owner.username == getCurrentUsername();

    const typeToString = (type:ChatType): string => {
        console.log("Chat type: ", type)
        if(type == ChatType.MONOLOGUE) return "Monologue";
        if(type == ChatType.DIALOGUE) return "Dialogue";
        if(type == ChatType.POLYLOGUE) return "Polylogue";
        else return "Unknown";
    }

    return <div className="h-screen flex-col p-3 pt-15 w-[500px] mx-auto">
        <Block className="flex flex-col items-center">
            <div className="flex items-end w-full">
                <IconButton inverted={true} 
                    iconSrc={CloseIcon} 
                    onClick={onClose} 
                    className="ms-auto"/>
            </div>

            <ChatPicture chat={chat} canEdit={isOwner}/>
            <p className="me-2 mt-5 text-2xl">{chat.title}</p>
            <p className="text-minor-text mb-5">Members: {chat.members.length}</p>
            <p className="text-minor-text mb-5">type: {typeToString(chat.type)}</p>
            <p className="mb-5">{chat.description}</p>

            {isOwner && <MinorButton onClick={onEdit} >Edit chat</MinorButton>}
        </Block>

        <p className="text-center font-bold my-4">Members</p>
        <Block className="flex flex-col overflow-y-auto">
            {isOwner && <OnSurfaceBlock className="flex mb-1">
                <FormTextBox className="flex-1" placeholder="Enter username" value={usernameText} onChange={setUsernameText} />
                <MinorButton onClick={() => onAddMember(usernameText)}>
                    Invite
                </MinorButton>
            </OnSurfaceBlock>}
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

interface ChatPictureProps {
    chat: Chat,
    canEdit: boolean
}
function ChatPicture({ chat, canEdit }: ChatPictureProps) {
    const [isUpdate, setIsUpdate] = useState(false);
    const [image, setImage] = useState<File | undefined>();
    const [errorText, isError, showError, hideError] = useNotification();

    const handleOnSave = () => {
        if (!image) {
            showError("Select image first");
            return;
        }

        api.updateImage(chat.id, image).then((_) => {
            handleOnChange();
        }).catch((e) => {
            console.error(e);
            showError("Failed to update an image");
        });
    }
    const handleImageUpload = (image: File) => {
        setImage(image);
        hideError();
    }
    const handleOnChange = () => {
        setIsUpdate(!isUpdate);
        setImage(undefined);
        hideError();
    }

    if (isUpdate) {
        return <Block className="w-full">
            <p className="font-bold text-center mb-5">Update profile picture</p>

            {isError && <BlockNotification className="mb-5" text={errorText}/>}

            <ImageUpload onChange={handleImageUpload} />

            <ActiveButton className="mt-4" onClick={handleOnSave}>Save selected image</ActiveButton>
            <MinorButton className="mt-4" onClick={handleOnChange}>Cancel</MinorButton>
        </Block>
    } else {
        return <div className="relative">
            <div className="h-full w-full">
                <img className="w-[250px] h-[250px] rounded-[0.5rem] border-2 border-outline" 
                    src={"/api/images/chat-avatar/" + chat.imageId + "?size=3"} 
                    alt="profile-avatar" />
            </div>
            {canEdit && <IconButton 
                className="absolute top-0 right-0 m-3"
                inverted={true}
                iconSrc={EditIcon}
                onClick={handleOnChange} />
            }
        </div>
    }
}