import { useEffect, useState } from "react";
import LoadingScreen from "../shared/LoadingScreen";
import type UserProfile from "../../data-model/Profile";
import AvatarImage from "../shared/AvatarImage";
import EditIcon from "../../assets/icons/edit.svg"
import MinorButton from "../shared/MinorButton";
import IconButton from "../shared/IconButton";
import TextBox from "../shared/TextBox";
import { useNavigate, useParams } from "react-router-dom";

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
            <div className="flex flex-col p-10 rounded mt-15 bg-surface">
                <p className="font-bold text-center mb-5">Edit profile</p>
                <label className="mb-2">Username</label> 
                <TextBox value={newUsername} onChange={(e)=>handleInput(e, "username")}></TextBox>
                <label className="mt-5 mb-2">DisplayName</label>
                <TextBox value={newDisplayName} onChange={(e)=>handleInput(e, "displayName")}></TextBox>
                <label className="mt-5 mb-2">Bio</label>
                <TextBox style="" value={newBio} onChange={(e)=>handleInput(e, "bio")}></TextBox>
                <MinorButton style="mt-5 mb-2 w-full" onClick={handleOnSave}>Save</MinorButton>
                <MinorButton style="w-full" onClick={handleOnCancel}>Cancel</MinorButton>
            </div>
        </div>
    }
    return <div className="flex flex-col mx-auto w-[500px] p-2">
        <div className="flex p-5 mt-15 mb-3 bg-surface rounded">
            <AvatarImage size="large" imgUrl={profile.imageUrl} />
            <div className="ms-3">
                <p className="text-2xl mb-2">{profile.displayName}</p>
                <p>username: {profile.username}</p>
            </div>
            { isCurrentUser && <div className="ms-auto">
                <IconButton isInverted={true} onClick={handleOnEdit} iconSrc={EditIcon}/>
            </div>
            }
        </div>
        <div className="p-5 bg-surface rounded mb-3">
            <p className="text-center font-bold mb-3">Bio</p>
            <p className="">{profile.bio}</p>
        </div>
    </div>
}

async function fetchProfile(): Promise<UserProfile> {
    const profile: UserProfile = {
        username: "1",
        displayName: "user",
        imageUrl: "",
        bio: "Tell something about myself"
    };
    return profile;
}