interface IconButtonProps {
    onClick: () => void,
    iconSrc: string,
    isInverted?: boolean
}
export default function IconButton({ onClick, iconSrc, isInverted }: IconButtonProps) {
    return <div onClick={onClick} className="p-2 rounded hover:bg-on-surface">
        <img height={24} width={24} src={iconSrc} 
            className={isInverted ? "invert" : ""} />
    </div>
}