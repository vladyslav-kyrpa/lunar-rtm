import { useEffect, useRef, useState, type ReactNode } from "react";

interface MenuItem {
    label: string,
    onClick: ()=>void
}
interface DropDownMenuProps {
    button : ReactNode,
    menuItems: MenuItem[]
}
export default function DropDownMenu({button, menuItems}:DropDownMenuProps) {
    const [isOpen, setIsOpen] = useState(false);
    const [position, setPosition] = useState<"left"|"right">("left");
    const menuRef = useRef<HTMLDivElement>(null);

    useEffect(()=>{
        if (isOpen && menuRef.current) {
            const rect = menuRef.current.getBoundingClientRect();
            console.log(rect.right);
            if (rect.right > window.innerWidth)
                setPosition("right");
            if(rect.left < 0)
                setPosition("left");
        }
    },[isOpen])

    const handleToggleMenu = () => {
        setIsOpen(!isOpen);
    }
    return <div className="relative">
        <div onClick={handleToggleMenu}>{button}</div>
        {isOpen &&
        <div ref={menuRef} className={`absolute ${position}-0 mt-2 bg-on-surface border-1 border-on-surface-outline rounded p-1`}>{
            menuItems.map((item, key)=><div key={key} 
                onClick={item.onClick} className="p-3 hover:bg-on-surface-outline rounded">{item.label}</div>)
        }</div>}
    </div>
}