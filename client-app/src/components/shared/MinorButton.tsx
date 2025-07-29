import type { ReactNode } from "react"

interface MinorButtonProps {
    children: ReactNode,
    style?: string,
    onClick: () => void
}

export default function MinorButton({ children, onClick, style }: MinorButtonProps) {
    return <button
        onClick={onClick}
        className={"bg-transparent text-white border-2 border-white rounded p-3 " + (style ?? '')}>
        {children}
    </button>
}