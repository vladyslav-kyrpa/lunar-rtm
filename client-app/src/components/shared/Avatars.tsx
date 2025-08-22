import PlaceholderImage from "../../assets/img-placeholder.jpg";

export const AvatarSize = {
    Small: 1,
    Medium: 2,
    Large: 3,
    ExtraLarge: 4
} as const;
export type AvatarSize = typeof AvatarSize[keyof typeof AvatarSize];

interface ImageProps {
    imageId: string,
    size: AvatarSize,
    className?: string,
}
export function ChatImage({ imageId, size, className }: ImageProps) {
    let imageUrl = PlaceholderImage;
    if (imageId !== "" && imageId !== "empty")
        imageUrl = `/api/images/chat-avatar/${imageId}?size=${size.toString()}`;

    return <div className={className}>
        <StaticAvatarImage imgUrl={imageUrl} size={size} />
    </div>
}
export function ProfileImage({ imageId, size, className }: ImageProps) {
    let imageUrl = PlaceholderImage;
    if (imageId !== "" && imageId !== "empty")
        imageUrl = `/api/images/profile-avatar/${imageId}?size=${size.toString()}`;

    return <div className={className}>
        <StaticAvatarImage imgUrl={imageUrl} size={size} />
    </div>
}

interface StaticAvatarImageProps {
    imgUrl: string
    size: AvatarSize,
}
export function StaticAvatarImage({ imgUrl, size }: StaticAvatarImageProps) {
    let sizeStyle = "";

    if (size === AvatarSize.Small)
        sizeStyle = "h-[48px] w-[48px]";
    if (size === AvatarSize.Medium)
        sizeStyle = "h-[72px] w-[72px]"
    if (size === AvatarSize.Large)
        sizeStyle = "h-[208px] w-[208px]"

    return <img src={imgUrl}
        className={`${sizeStyle} rounded-full bg-black border-2 cus-outline`} />
}


export default { AvatarSize, ChatImage, ProfileImage };