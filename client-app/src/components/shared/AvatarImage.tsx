interface AvatarImageProp {
    imgUrl:string
    size: "extra-large" | "large" | "medium" | "small"
}

export default function AvatarImage({imgUrl: iconUrl, size}:AvatarImageProp) {
    let sizeStyle = "h-8 w-8"
    if(size === "small")
        sizeStyle = "h-8 w-8"
    if(size === 'medium')
        sizeStyle = "h-12 w-12"
    if(size === 'large')
        sizeStyle = "h-[72px] w-[72px]"
    if(size === 'extra-large')
        sizeStyle = "h-[96px] w-[96px]"

    return <img src={iconUrl} 
        className={sizeStyle + " rounded-full bg-black border-2 cus-outline"}/> 
}


