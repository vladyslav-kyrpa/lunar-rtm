import type { ReactNode } from "react";

interface ButtonProps {
    onClick: () => void;
    children: ReactNode;
    className?: string;
} 

export function ActiveButton({onClick, children, className}:ButtonProps) {
    return <div onClick={onClick} className={"text-center bg-active border-active-outline p-2 rounded border hover:bg-active-outline " + className}>
        <div className="text-inverted-text">{children}</div>
    </div>
}

export function MinorButton({onClick, children, className}:ButtonProps) {
    return <div onClick={onClick} className={"text-center bg-transparent border-active-outline p-2 rounded border hover:bg-on-surface-outline " + className}>
        <div className="text-main-text">{children}</div>
    </div>
}

interface TextButtonProps{
  onClick: () => void;
  text: string; 
  className?: string;
}
export function TextButton({className, text, onClick}:TextButtonProps) {
    return <div className={"text-center rounded p-1 underline text-main-text hover:bg-on-surface-outline " + className} onClick={onClick}>
        {text}
    </div>
}

interface IconButtonProps {
  onClick: () => void;
  iconSrc: string;
  className?: string;
  inverted: boolean;
}
export function IconButton({iconSrc, onClick, className, inverted}:IconButtonProps) {
    return <div className={"w-fit p-1 rounded border border-transparent hover:bg-on-surface hover:border-on-surface-outline " + className}>
        <img onClick={onClick} src={iconSrc} height={28} width={28} 
            className={inverted ? "invert" : ""}/>
    </div>
}

export function IconButtonSm({iconSrc, onClick, className, inverted}:IconButtonProps) {
    return <div className={"w-fit p-1 rounded border border-transparent hover:bg-on-surface hover:border-on-surface-outline " + className}>
        <img onClick={onClick} src={iconSrc} height={20} width={20} 
            className={inverted ? "invert" : ""}/>
    </div>
}

export default {ActiveButton, MinorButton, TextButton, IconButton, IconButtonSm}