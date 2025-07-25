import type { ReactNode } from "react"

interface ActiveButtonProps {
    children: ReactNode,
    style?: string,
    onClick: () => void
}

export default function ActiveButton({ children, onClick, style }: ActiveButtonProps) {
    return <button
        onClick={onClick}
        className={"bg-active text-black rounded p-3 " + (style ?? '')}>
        {children}
    </button>
}