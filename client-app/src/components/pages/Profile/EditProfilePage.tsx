import { useEffect, useState } from "react";
import useNotification from "../../hooks/UseNotification";
import { Block } from "../../shared/Blocks";
import { FormTextBox } from "../../shared/TextBoxes";
import BlockNotification from "../../shared/BlockNotification";
import { MinorButton } from "../../shared/Buttons";
import { useProfileContext } from "./ProfileContext";
import LoadingScreen from "../../shared/LoadingScreen";
import DefaultPageLayout from "../../shared/DefaultPageLayout";
import RestrictedPageBody from "../../shared/RestrictedPageBody";

interface EditProfilePageProps {
    onSave: (newUsername: string) => void;
    onClose: () => void;
}
export default function EditProfilePage({ onSave, onClose }: EditProfilePageProps) {
    const { profile, updateProfile } = useProfileContext();
    const [errorText, isError, showError, hideError] = useNotification();
    const [newUsername, setNewUsername] = useState("");
    const [newDisplayName, setNewDisplayName] = useState("");
    const [newBio, setNewBio] = useState("");
    
    useEffect(()=>{
        if(!profile) return;
        setNewBio(profile.bio);
        setNewUsername(profile.username);
        setNewDisplayName(profile.displayName);
    },[profile])

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
        updateProfile(newUsername, newDisplayName, newBio)
            .then((_)=>onSave(newUsername));
    }

    if(!profile) return <LoadingScreen/>

    return <DefaultPageLayout title="Edit profile">
        <RestrictedPageBody>
            <Block className="flex flex-col mt-15">
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
        </RestrictedPageBody>
    </DefaultPageLayout>
}