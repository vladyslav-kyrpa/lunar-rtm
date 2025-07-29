import { useNavigate } from "react-router-dom"
import AvatarImage from "./AvatarImage"
import NewChatIcon from "../../assets/icons/add-profile.svg"
import ChatListIcon from "../../assets/icons/chat-list.svg"
import SettingsIcon from "../../assets/icons/settings.svg"
import LogoutIcon from "../../assets/icons/logout.svg"

interface MenuItem {
    title:string,
    iconSrc:string
    handler:()=>void,
}

export default function SideMenu() {
    const navigate = useNavigate();
    const handleOnUserProfile = (username:string) => {
        navigate("/profile/"+username);
    }

    const menuItems:MenuItem[] = [
        { title:"New chat", iconSrc:NewChatIcon, handler:()=>{} },
        { title:"All chats", iconSrc:ChatListIcon, handler:()=>{ navigate("/chats")} },
        { title:"Settings", iconSrc:SettingsIcon, handler:()=>{} },
        { title:"Logout", iconSrc:LogoutIcon, handler:()=>{} },
    ] 

    return <div className="bg-surface h-full flex flex-col p-5">
        <UserProfileHeader onClick={handleOnUserProfile}/>
        <div className="flex flex-col gap-2 mt-4">
            { menuItems.map((item, key)=>{
                return <div key={key} className= "p-3 flex flex-row rounded flex-1 transition duration-200 ease-in-out hover:bg-on-surface" onClick={item.handler}>
                    <img className="me-2 invert" src={item.iconSrc} alt="create-new-chat" width={24} height={24} />
                    <p>{item.title}</p>
                </div>
            })}
        </div>
    </div>
}

interface UserProfileHeaderProps {
    onClick:(username:string)=>void
}
function UserProfileHeader({onClick}:UserProfileHeaderProps) {
    const username = "username";
    return <div className="flex flex-row items-center p-2" onClick={()=>onClick(username)}>
        <AvatarImage iconUrl="localhost" size="large"/>
        <div className="ms-4">
            <p className="mb-1 font-bold">Public Display Name</p>
            <p>@username</p>
        </div>
    </div>
}