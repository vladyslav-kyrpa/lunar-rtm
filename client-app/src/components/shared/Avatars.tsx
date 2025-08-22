import { useState } from "react";
import useNotification from "../hooks/UseNotification";
import BlockNotification from "./BlockNotification";
import { Block } from "./Blocks";
import { ActiveButton, IconButton, MinorButton } from "./Buttons";
import ImageUpload from "./ImageUpload";
import EditIcon from "../../assets/icons/edit.svg";

interface StaticAvatarImageProp {
    imgUrl: string
    size: "extra-large" | "large" | "medium" | "small"
    className?: string
}
export function StaticAvatarImage({ imgUrl: iconUrl, size, className }: StaticAvatarImageProp) {
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

interface UpdatableAvatarProps {
    imageId: string,
    type:"chat-avatar" | "profile-avatar"
    canEdit: boolean,
    onSave: (image: File) => Promise<void>
}
export function UpdatableAvatar({ imageId, canEdit, type, onSave }: UpdatableAvatarProps) {
    const [isUpdate, setIsUpdate] = useState(false);
    const [image, setImage] = useState<File | undefined>();
    const [errorText, isError, showError, hideError] = useNotification();

    const handleOnSave = () => {
        if (!image) {
            showError("Select image first");
            return;
        }
        onSave(image).then((_) => handleOnChange());
    }

    const handleImageUpload = (image: File) => {
        setImage(image);
        hideError();
    }
    const handleOnChange = () => {
        setIsUpdate(!isUpdate);
        setImage(undefined);
        hideError();
    }

    if (isUpdate) {
        return <Block className="w-full">
            <p className="font-bold text-center mb-5">Update picture</p>

            {isError && <BlockNotification className="mb-5" text={errorText} />}

            <ImageUpload onChange={handleImageUpload} />

            <ActiveButton className="mt-4" onClick={handleOnSave}>Save selected image</ActiveButton>
            <MinorButton className="mt-4" onClick={handleOnChange}>Cancel</MinorButton>
        </Block>
    } else {
        return <div className="relative">
            <div className="h-full w-full">
                <img className="w-[250px] h-[250px] rounded-[0.5rem] border-2 border-outline"
                    src={"/api/images/"+ type +"/" + imageId + "?size=3"}
                    alt="profile-avatar" />
            </div>
            {canEdit && <IconButton
                className="absolute top-0 right-0 m-3"
                inverted={true}
                iconSrc={EditIcon}
                onClick={handleOnChange} />
            }
        </div>
    }
}

export default { StaticAvatarImage, UpdatableAvatar };