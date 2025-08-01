import { useEffect, useState } from "react";
import LoadingScreen from "../shared/LoadingScreen";
import type UserProfile from "../../data-model/Profile";
import AvatarImage from "../shared/AvatarImage";
import EditIcon from "../../assets/icons/edit.svg"
import { useNavigate, useParams } from "react-router-dom";
import { FormTextBox } from "../shared/TextBoxes";
import { IconButton, MinorButton } from "../shared/Buttons";
import { Block } from "../shared/Blocks";

export default function ProfilePage() {
    const {username} = useParams();
    const [profile, setProfile] = useState<UserProfile | null>(null);
    const [isEditMode, setIsEditMode] = useState(false);
    const [newUsername, setNewUsername] = useState("");
    const [newDisplayName, setNewDisplayName] = useState("");
    const [newBio, setNewBio] = useState("");
    const isCurrentUser = true;

    const navigate = useNavigate();
    useEffect(() => {
        // reset
        setProfile(null);
        setNewUsername("");
        setNewDisplayName("");
        setNewBio("");

        // fetch new
        fetchProfile().then((result) => {
            setProfile(result);
            setNewUsername(result.username);
            setNewDisplayName(result.displayName);
            setNewBio(result.bio);
        }).catch((error) => {
            console.log(error);
        })
    }, [username])

    const handleOnSave = () => {
        console.log("Save ");
        setIsEditMode(false);
        navigate("/profile/" + newUsername)
    }

    const handleOnCancel = () => {
        if(!profile) return;
        setNewUsername(profile.username);        
        setNewDisplayName(profile.displayName);        
        setNewBio(profile.bio);        
        setIsEditMode(false);
    }

    const handleOnEdit = () => {
        setIsEditMode(true);
    }

    const handleInput = (value:string, param:string) => {
        if(param === "username")
            setNewUsername(value);
        if(param === "displayName")
            setNewDisplayName(value);
        if(param === "bio")
            setNewBio(value);
    }


    if (profile === null) {
        return <LoadingScreen />
    }

    if(isEditMode){
        return <div className="w-[500px] mx-auto">
            <Block className="flex flex-col mt-15">
                <p className="font-bold text-center mb-5">Edit profile</p>
                <label className="mb-2">Username</label> 
                <FormTextBox value={newUsername} onChange={(e)=>handleInput(e, "username")}></FormTextBox>
                <label className="mt-5 mb-2">DisplayName</label>
                <FormTextBox value={newDisplayName} onChange={(e)=>handleInput(e, "displayName")}></FormTextBox>
                <label className="mt-5 mb-2">Bio</label>
                <FormTextBox className="" value={newBio} onChange={(e)=>handleInput(e, "bio")}></FormTextBox>
                <MinorButton className="mt-5 mb-2 w-full" onClick={handleOnSave}>Save</MinorButton>
                <MinorButton className="w-full" onClick={handleOnCancel}>Cancel</MinorButton>
            </Block>
        </div>
    }
    return <div className="flex flex-col mx-auto w-[500px] p-2">
        <Block className="mt-15 flex flex-col items-center">
            <AvatarImage size="extra-large" imgUrl={profile.imageUrl} />
            <div className="flex mb-1 mt-3">
                <p className="text-2xl">{profile.displayName}</p>
                { isCurrentUser && <div className="ms-auto">
                    <IconButton className="ms-2" inverted={true} onClick={handleOnEdit} iconSrc={EditIcon}/>
                </div>}
            </div>
            <p className="text-minor-text mb-5">username: {profile.username}</p>
            <p className="">{profile.bio}</p>
        </Block>
    </div>
}

async function fetchProfile(): Promise<UserProfile> {
    const profile: UserProfile = {
        username: "1",
        displayName: "User Display Name",
        imageUrl: "",
        bio: "Tell something about myself"
    };
    return profile;
}