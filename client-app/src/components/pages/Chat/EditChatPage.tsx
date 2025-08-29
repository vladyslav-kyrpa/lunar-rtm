import { useEffect, useState } from "react";
import { Block } from "../../shared/Blocks";
import { FormTextBox } from "../../shared/TextBoxes";
import { MinorButton } from "../../shared/Buttons";
import { useChatContext } from "./ChatContext";
import LoadingScreen from "../../shared/LoadingScreen";
import DefaultPageLayout from "../../shared/DefaultPageLayout";
import RestrictedPageBody from "../../shared/RestrictedPageBody";

interface EditChatPageProps {
    onClose: () => void;
    onSubmit: () => void;
}

export default function EditChatPage({onClose, onSubmit}: EditChatPageProps) {
    const { chat, update } = useChatContext();
    const [newTitle, setNewTitle] = useState(chat?.title ?? "");
    const [newDescription, setNewDescription] = useState(chat?.description ?? "");

    useEffect(()=>{
        if(!chat) return;
        setNewTitle(chat?.title);
        setNewDescription(chat?.description);
    },[chat]);

    const handleInput = (value: string, param: string) => {
        if (param === "title")
            setNewTitle(value);
        if (param === "description")
            setNewDescription(value);
    }

    const handleOnSubmit = () => {
        if(newTitle === "" || newDescription === ""){
            console.error("Form values should be not empty");
            return;
        }
        update(newTitle, newDescription)
            .then((_)=>onSubmit());
    }

    if(!chat) return <LoadingScreen/>

    return <DefaultPageLayout title="Edit chat">
        <RestrictedPageBody>
            <Block className="mt-15 flex flex-col">
                <label className="mb-2">Title</label>
                <FormTextBox value={newTitle}
                    onChange={(e) => handleInput(e, "title")}
                    placeholder="Enter title..."
                    isError={newTitle === ""} />

                <label className="mt-5 mb-2">Description</label>
                <FormTextBox value={newDescription}
                    onChange={(e) => handleInput(e, "description")}
                    placeholder="Enter description..."
                    isError={newDescription === ""} />

                <MinorButton className="mt-5 mb-2 w-full" onClick={handleOnSubmit}>Save</MinorButton>
                <MinorButton className="w-full" onClick={onClose}>Cancel</MinorButton>
            </Block>
        </RestrictedPageBody>
    </DefaultPageLayout>
}