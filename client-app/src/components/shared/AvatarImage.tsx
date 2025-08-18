interface AvatarImageProp {
    imgUrl: string
    size: "extra-large" | "large" | "medium" | "small"
    className?: string
}

export default function AvatarImage({ imgUrl: iconUrl, size, className }: AvatarImageProp) {
    let sizeStyle = "h-8 w-8";
    let sizeIndex = 1;
    if (size === "small") {
        sizeStyle = "h-8 w-8";
        sizeIndex = 1;
    }
    if (size === 'medium') {
        sizeStyle = "h-12 w-12"
        sizeIndex = 2;
    }
    if (size === 'large') {
        sizeStyle = "h-[72px] w-[72px]"
        sizeIndex = 3;
    }
    if (size === 'extra-large') {
        sizeStyle = "h-[208px] w-[208px]"
        sizeIndex = 3;
    }
    return <img src={iconUrl + "?size=" + sizeIndex.toString()}
        className={sizeStyle + " rounded-full bg-black border-2 cus-outline " + className} />
}
