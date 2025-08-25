import { useState } from "react";
import LoadingScreen from "../../shared/LoadingScreen";
import { useNavigate, useParams } from "react-router-dom";
import { MinorButton } from "../../shared/Buttons";
import { Block } from "../../shared/Blocks";
import EditProfilePage from "./EditProfilePage";
import { ProfileProvider, useProfileContext } from "./ProfileContext";
import { useAuth } from "../../hooks/AuthContext";
import { UpdatableAvatar } from "../../shared/UpdatableAvatar";
import { logout } from "../../../api/AuthApi";
import useNotification from "../../hooks/UseNotification";
import ConfirmationDialog from "../../shared/ConfimationDialog";

export default function ProfilePage(){
    const { username } = useParams();
    return <ProfileProvider username={username ?? ""}>
        <ProfilePageContent/>
    </ProfileProvider>
}

export function ProfilePageContent() {
    const { profile, updateImage, deleteProfile } = useProfileContext();
    const [isEditMode, setIsEditMode] = useState(false);
    const [deleteConfText, isConfirmationRised, showConfirmation, hideConfirmation] = useNotification();
    const navigate = useNavigate();
    const { username } = useAuth();

    const isCurrentUser = () => profile != null 
        && profile.username == username;

    const handleOnSave = (newUsername:string) => {
        setIsEditMode(false);
        if (isCurrentUser()) {
            logout().then((_)=>navigate("/log-in"));
        } else {
            navigate("/profile/" + newUsername);
        }
    }

    const handleOnCancel = () => {
        if (!profile) return;
        setIsEditMode(false);
    }

    const handleOnEdit = () => {
        setIsEditMode(true);
    }

    const handleOnDeleteConfirmed = (isConfirmed:boolean) => {
        if(isConfirmed)
            deleteProfile().then((_)=>navigate("/"));
        else hideConfirmation();
    }
    const handleOnDelete = () => {
        showConfirmation("Do you want to delete this profile?");
    }

    const handleOnImageUpdate = async (image:File) => {
        return updateImage(image);
    }

    if (!profile) return <LoadingScreen />
    if (isEditMode) return <EditProfilePage onSave={handleOnSave} 
        onClose={handleOnCancel} />

    return <>
        <div className="mx-auto p-2 max-w-[500px]">
            <Block className="mt-15 flex-col flex items-center">
                <p className="font-bold text-center mb-5">User Profile</p>
                <UpdatableAvatar imageId={profile.imageId} 
                    type={"profile-avatar"}
                    canEdit={isCurrentUser()} 
                    onSave={handleOnImageUpdate}/>
                <p className="text-2xl mt-5 text-center">{profile.displayName}</p>
                <p className="text-minor-text mb-5">username: {profile.username}</p>
                <p className="mb-5">{profile.bio}</p>
                {isCurrentUser() && <div>
                    <MinorButton onClick={handleOnEdit}>Edit profile</MinorButton>
                    <MinorButton onClick={handleOnDelete}>Delete profile</MinorButton>
                </div>}
            </Block>
        </div>
        <ConfirmationDialog show={isConfirmationRised} 
            onResponse={handleOnDeleteConfirmed} title="Confirmation"
            text={deleteConfText}/>
    </>
}


