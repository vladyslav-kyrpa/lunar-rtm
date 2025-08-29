import type { ReactNode } from "react"

interface DefaultPageLayoutProps {
    onTitleClick?:()=>void,
    title:string,
    children?:ReactNode
    bottom?:ReactNode
}
export default function DefaultPageLayout({onTitleClick, title, children, bottom}:DefaultPageLayoutProps) {
    return <div className="flex flex-col h-[100dvh]">
        <div className="sticky top-0 h-16 flex items-center justify-center bg-surface"
            onClick={onTitleClick}>
            <p className="font-bold">{title}</p>
        </div>
        <div className="flex flex-col flex-1 p-2 overflow-y-auto">{children}</div>
        {bottom}
    </div>
}