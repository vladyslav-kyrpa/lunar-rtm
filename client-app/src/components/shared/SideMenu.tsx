import { useNavigate } from "react-router-dom"
import NewChatIcon from "../../assets/icons/add-profile.svg"
import ChatListIcon from "../../assets/icons/chat-list.svg"
import SettingsIcon from "../../assets/icons/settings.svg"
import LogoutIcon from "../../assets/icons/logout.svg"
import ProfileIcon from "../../assets/icons/profile.svg"

interface MenuItem {
    title: string,
    iconSrc: string
    handler: () => void,
}

export default function SideMenu() {
    const navigate = useNavigate();

    const menuItems: MenuItem[] = [
        { title: "Profile", iconSrc: ProfileIcon, handler: () => { navigate("/profile/me") } },
        { title: "New chat", iconSrc: NewChatIcon, handler: () => { navigate("/create-chat") } },
        { title: "All chats", iconSrc: ChatListIcon, handler: () => { navigate("/chats") } },
        { title: "Settings", iconSrc: SettingsIcon, handler: () => { navigate("/settings") } },
        { title: "Logout", iconSrc: LogoutIcon, handler: () => { } },
    ]

    return <div className="bg-surface h-full flex flex-col p-5">
        <div className="flex flex-col gap-2 mt-4">
            {menuItems.map((item, key) => {
                return <div key={key} className="p-3 flex flex-row rounded flex-1 transition duration-200 ease-in-out hover:bg-on-surface" onClick={item.handler}>
                    <img className="me-2 invert" src={item.iconSrc} alt="create-new-chat" width={24} height={24} />
                    <p>{item.title}</p>
                </div>
            })}
        </div>
    </div>
}