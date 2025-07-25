import type { ReactNode } from "react";

interface MinorButtonProps {
    children: ReactNode,
    onClick?: () => void,
    style?: string,
}

export default function MinorButton({ children, onClick, style }: MinorButtonProps) {
    return <button onClick={onClick}
        className={"bg-transparent text-white " + (style ?? '')}>{children}
    </button>

}