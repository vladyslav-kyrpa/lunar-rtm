import type { ReactNode } from "react";

interface TextButtonProps {
    children: ReactNode,
    onClick?: () => void,
    style?: string,
}

export default function TextButton({ children, onClick, style }: TextButtonProps) {
    return <button onClick={onClick}
        className={"bg-transparent text-white " + (style ?? '')}>{children}
    </button>

}