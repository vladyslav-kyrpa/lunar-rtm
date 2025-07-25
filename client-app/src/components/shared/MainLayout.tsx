import { useEffect, useState, type ReactNode } from "react";
import SideMenu from "./SideMenu";

interface MainLayoutProps {
    children: ReactNode
}
export default function MainLayout({ children }: MainLayoutProps) {
    const width = useWindowWidth();

    return <div className="flex">
        { width < 900 ? <></>
            : <div className="w-[300px]"><SideMenu /></div>
        }
        <div className="flex-1">{children}</div>
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