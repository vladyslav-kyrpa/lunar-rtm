import type { ReactNode } from "react";

interface BlockProps {
    children: ReactNode,
    className?:string
}
export function Block({children, className}:BlockProps){
    return <div className={"bg-surface border border-surface-outline p-5 rounded " + className}>
        {children}
    </div>
}

export function OnSurfaceBlock({children, className}:BlockProps){
    return <div className={"bg-on-surface border border-on-surface-outline p-5 rounded " + className}>
        {children}
    </div>
}

export default { Block, OnSurfaceBlock }