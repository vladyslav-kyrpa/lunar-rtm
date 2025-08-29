import type { ReactNode } from "react"

interface RestrictedPageBodyProps {
    maxWidthPx?: number,
    children: ReactNode
}

export default function RestrictedPageBody({ maxWidthPx, children }: RestrictedPageBodyProps) {
    const width = !maxWidthPx ? 500 : maxWidthPx;
    return <div className={`h-[100dvh] mx-auto w-full max-w-[${width}px]`}>
        {children}
    </div>
}