import { useEffect, useState, type ReactNode } from "react";
import SideMenu from "./SideMenu";
import MenuIcon from "../../assets/icons/menu.svg"

interface MainLayoutProps {
    children: ReactNode
}
export default function MainLayout({ children }: MainLayoutProps) {
    const width = useWindowWidth();

    return <div className="flex overflow-y-auto h-screen">
        { width < 900 
            ? <SideMenuSm/>
            : <div className="w-[350px]"><SideMenu /></div>
        }
        <div className="flex-1 overflow-y-auto">
            {children}
        </div>
    </div>
}

function useWindowWidth() {
    const [width, setWidth] = useState(window.innerWidth);
    useEffect(() => {
        const onResize = () => {
            setWidth(window.innerWidth);
        }

        window.addEventListener('resize', onResize);

        return () => {
            window.removeEventListener('resize', onResize);
        }
    }, []);
    return width;
}

function SideMenuSm() {
    const [showMenu, setShowMenu] = useState(false);

    const toggleSideMenu = () => {
        setShowMenu(!showMenu);
    }
    
    if(showMenu){
        return <>
            <div className="bg-black opacity-90 z-100 fixed top-0 left-0 right-0 bottom-0" onClick={toggleSideMenu}></div>
            <div className="fixed z-101 top-0 left-0 bottom-0">
                <SideMenu></SideMenu>
            </div>
        </>
    } else {
        return <div onClick={toggleSideMenu} className="menu-button rounded p-2 mt-2 ml-2">
            <img src={MenuIcon} alt="" className="invert" height={32} width={32}/>
        </div>
    }
}