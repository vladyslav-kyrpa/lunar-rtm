import { useEffect, useState } from "react";
import LoadingScreen from "../shared/LoadingScreen";
import type UserProfile from "../../data-model/UserProflie";
import { useNavigate, useParams } from "react-router-dom";
import { FormTextBox } from "../shared/TextBoxes";
import { IconButton, MinorButton } from "../shared/Buttons";
import { Block } from "../shared/Blocks";
import ImageUpload from "../shared/ImageUpload";
import api from "../../api/ProfilesApi";
import AvatarImage from "../shared/AvatarImage";
import EditIcon from "../../assets/icons/edit.svg"

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
    return <div className="flex flex-col mx-auto w-[500px] p-2">
        <Block className="mt-15 flex flex-col items-center">
            <AvatarImage size="extra-large" imgUrl={profile.avatarUrl} />
            <div className="flex mb-1 mt-3">
                <p className="text-2xl">{profile.displayName}</p>
                {isCurrentUser && <div className="ms-auto">
                    <IconButton className="ms-2" inverted={true} onClick={handleOnEdit} iconSrc={EditIcon} />
                </div>}
            </div>
            <p className="text-minor-text mb-5">username: {profile.username}</p>
            <p className="">{profile.bio}</p>
        </Block>
    </div>
}


interface EditProfilePageProps {
    user: UserProfile;
    onSave: (newUsername: string) => void;
    onClose: () => void;
}
function EditProfilePage({ user, onSave, onClose }: EditProfilePageProps) {
    const [newUsername, setNewUsername] = useState(user.username);
    const [newDisplayName, setNewDisplayName] = useState(user.displayName);
    const [newBio, setNewBio] = useState(user.bio);
    const [image, setImage] = useState<File>();

    const handleInput = (value: string, param: string) => {
        if (param === "username")
            setNewUsername(value);
        if (param === "displayName")
            setNewDisplayName(value);
        if (param === "bio")
            setNewBio(value);
    }

    const handleOnSave = () => {
        if (newUsername === "" || newDisplayName === "" || newBio) {
            console.error("Form values should be not empty");
            return;
        }

        api.updateProfile(user.username, newUsername, newDisplayName, newBio, image).then((r) => {
            console.log(r);
            onSave(newUsername);
        }).catch((e) => {
            console.error(e);
        })
    }

    return <div className="w-[500px] mx-auto">
        <Block className="flex flex-col mt-15">
            <p className="font-bold text-center mb-5">Edit profile</p>
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

            <label className="mb-2">Image</label>
            <ImageUpload className="mb-5" onChange={setImage} />

            <MinorButton className="mb-2 w-full" onClick={handleOnSave}>Save</MinorButton>
            <MinorButton className="w-full" onClick={onClose}>Cancel</MinorButton>
        </Block>
    </div>
}