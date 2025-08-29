import { useState } from "react";
import { Block } from "../shared/Blocks";
import { FormTextBox } from "../shared/TextBoxes";
import { ActiveButton } from "../shared/Buttons";
import api from "../../api/ChatsApi";
import { useNavigate } from "react-router-dom";
import OptionsSelector from "../shared/OptionsSelector";
import { ChatType } from "../../data-model/Chat";
import DefaultPageLayout from "../shared/DefaultPageLayout";
import RestrictedPageBody from "../shared/RestrictedPageBody";

export default function NewChatPage() {
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [chatType, setChatType] = useState<ChatType>(ChatType.MONOLOGUE);
    const navigate = useNavigate();
        
    const handleCreate = () => {
        api.createChat(title, description, chatType).then((r)=>{
            console.log(r);
            navigate("/chats");
        }).catch((e)=>{
            console.error(e);
        });
        console.log("Create new conversation")
    }

    const chatTypeOptions = [
       { value: ChatType.MONOLOGUE, label: "Monologue"}, 
       { value: ChatType.DIALOGUE, label: "Dialogue"}, 
       { value: ChatType.POLYLOGUE, label: "Polylogue"}, 
    ];
    const handleOnTypeSelected = (value:ChatType) => {
        console.log("selected type: ", value);
        setChatType(value);
    }

    return <DefaultPageLayout title="New chat">
        <RestrictedPageBody>
            <Block className="flex flex-col">
                <label className="mb-2">Title</label>
                <FormTextBox 
                    placeholder="New conversation title..." 
                    className="mb-5" value={title} 
                    isError={title === ""}
                    onChange={setTitle}/>

                <label className="mb-2">Description</label>
                <FormTextBox 
                    placeholder="What conversation is about..." 
                    className="mb-5" 
                    value={description} 
                    isError={description === ""}
                    onChange={setDescription}/>

                <label className="mb-2">Chat type</label>
                <OptionsSelector className="mb-5" options={chatTypeOptions} 
                    onSelected={handleOnTypeSelected}/>

                <ActiveButton  onClick={handleCreate}>Create</ActiveButton>
            </Block>
        </RestrictedPageBody>
    </DefaultPageLayout> 
}