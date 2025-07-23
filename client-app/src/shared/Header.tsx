import type { ReactNode } from "react"

interface HeaderProps {
    children: ReactNode
}

export function Header({children}:HeaderProps){
    return <h1 className="header">
        {children}
    </h1>
}