import { useState } from "react";
import { Block } from "../shared/Blocks";
import { FormTextBox } from "../shared/TextBoxes";
import { ActiveButton } from "../shared/Buttons";
import api from "../../api/ChatsApi";
import { useNavigate } from "react-router-dom";

export default function NewChatPage() {
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const navigate = useNavigate();
        
    const handleCreate = () => {
        api.createChat(title, description).then((r)=>{
            console.log(r);
            navigate("/chats");
        }).catch((e)=>{
            console.error(e);
        });
        console.log("Create new conversation")
    }

    return <div className="flex justify-center items-center">
        <Block className="mt-15 flex flex-col w-[400px]">
            <p className="text-center mb-5">Create new chat</p>
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
            <label className="mb-2">Image</label>
            <ActiveButton  onClick={handleCreate}>Create</ActiveButton>
        </Block>
    </div>
}