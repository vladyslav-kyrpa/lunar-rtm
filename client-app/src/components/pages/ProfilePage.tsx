import { useEffect, useState } from "react";
import LoadingScreen from "../shared/LoadingScreen";
import type UserProfile from "../../data-model/UserProflie";
import { useNavigate, useParams } from "react-router-dom";
import { FormTextBox } from "../shared/TextBoxes";
import { ActiveButton, IconButton, MinorButton } from "../shared/Buttons";
import { Block } from "../shared/Blocks";
import ImageUpload from "../shared/ImageUpload";
import api from "../../api/ProfilesApi";
import EditIcon from "../../assets/icons/edit.svg"
import useNotification from "../hooks/useNotification";
import BlockNotification from "../shared/BlockNotification";

export default function ProfilePage() {
    const { username } = useParams();
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [isEditMode, setIsEditMode] = useState(false);
    const isCurrentUser = username === undefined || username === "me";

    const navigate = useNavigate();
    useEffect(() => {
        // reset
        setProfile(null);
        // fetch new
        if (username === null) {
            api.fetchCurrentProfile().then((r) => {
                setProfile(r);
            }).catch((e) => {
                console.error(e);
            })
        } else {
            api.fetchProfile(username ?? "").then((r) => {
                setProfile(r);
            }).catch((e) => {
                console.log(e);
            })
        }
    }, [username]);

    const handleOnSave = (username: string) => {
        setIsEditMode(false);
        if (isCurrentUser) {
            navigate("/profile/me");
        } else {
            navigate("/profile/" + username);
        }
    }

    const handleOnCancel = () => {
        if (!profile) return;
        setIsEditMode(false);
    }

    const handleOnEdit = () => {
        setIsEditMode(true);
    }

    if (profile === null) {
        return <LoadingScreen />
    }
    if (isEditMode) {
        return <EditProfilePage user={profile} onSave={handleOnSave} onClose={handleOnCancel} />
    }
    return <div className="mx-auto p-2 max-w-[500px]">
        <Block className="mt-15 flex-col flex items-center">
            <p className="font-bold text-center mb-5">User Profile</p>
            <ProfilePicture profile={profile} isCurrent={isCurrentUser} />
                <p className="text-2xl mt-5 text-center">{profile.displayName}</p>
                <p className="text-minor-text mb-5">username: {profile.username}</p>
                <p className="mb-5">{profile.bio}</p>
                {isCurrentUser && <MinorButton onClick={handleOnEdit}>Edit profile</MinorButton>}
        </Block>
    </div>
}
interface ProfilePictureProps {
    profile: UserProfile,
    isCurrent: boolean
}
function ProfilePicture({ profile, isCurrent }: ProfilePictureProps) {
    const [isUpdate, setIsUpdate] = useState(false);
    const [image, setImage] = useState<File | undefined>();
    const [errorText, isError, showError, hideError] = useNotification();

    const handleOnSave = () => {
        if (!image) {
            showError("Select image first");
            return;
        }

        api.updateImage(profile.username, image).then((_) => {
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
                <img className="w-[250px] h-[250px] rounded-[0.5rem] border-2 border-outline" src={"/api/images/profile-avatar/" + profile.imageId + "?size=3"} alt="profile-avatar" />
            </div>
            {isCurrent && <IconButton 
                className="absolute top-0 right-0 m-3"
                inverted={true}
                iconSrc={EditIcon}
                onClick={handleOnChange} />
            }
        </div>
    }
}


interface EditProfilePageProps {
    user: UserProfile;
    onSave: (newUsername: string) => void;
    onClose: () => void;
}
function EditProfilePage({ user, onSave, onClose }: EditProfilePageProps) {
    const [errorText, isError, showError, hideError] = useNotification();
    const [newUsername, setNewUsername] = useState(user.username);
    const [newDisplayName, setNewDisplayName] = useState(user.displayName);
    const [newBio, setNewBio] = useState(user.bio);

    const handleInput = (value: string, param: string) => {
        hideError();
        if (param === "username")
            setNewUsername(value);
        if (param === "displayName")
            setNewDisplayName(value);
        if (param === "bio")
            setNewBio(value);
    }

    const handleOnSave = () => {
        if (newUsername === "" || newDisplayName === "" || newBio === "") {
            showError("Fill all required fields");
            return;
        }

        api.updateProfile(user.username, newUsername, newDisplayName, newBio).then((_) => {
            onSave(newUsername);
        }).catch((_) => {
            showError("Failed to update profile");
        });
    }

    return <div className="w-[500px] mx-auto">
        <Block className="flex flex-col mt-15">
            <p className="font-bold text-center mb-5">Edit profile</p>

            {isError && <BlockNotification className="mb-5" text={errorText} />}

            <label className="mb-2">Username</label>
            <FormTextBox value={newUsername}
                className="mb-5"
                isError={newUsername === ""}
                placeholder="Enter username..."
                onChange={(e) => handleInput(e, "username")} />

            <label className="mb-2">Display Name</label>
            <FormTextBox value={newDisplayName}
                className="mb-5"
                isError={newDisplayName === ""}
                placeholder="Enter display name..."
                onChange={(e) => handleInput(e, "displayName")} />

            <label className="mb-2">Bio</label>
            <FormTextBox value={newBio}
                className="mb-5"
                isError={newBio === ""}
                placeholder="Enter bio..."
                onChange={(e) => handleInput(e, "bio")} />

            <MinorButton className="mb-2 w-full" onClick={handleOnSave}>Save</MinorButton>
            <MinorButton className="w-full" onClick={onClose}>Cancel</MinorButton>
        </Block>
    </div>
}