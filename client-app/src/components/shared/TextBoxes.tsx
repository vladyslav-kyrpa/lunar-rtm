import { useState } from "react";
import { IconButton } from "./Buttons";
import OpenEye from "../../assets/icons/eye-open.svg";
import ClosedEye from "../../assets/icons/eye-closed.svg";

interface TextBoxProps {
    onChange: (value: string) => void;
    placeholder?: string;
    value: string;
    isSecret?: boolean;
    className?: string;
    isError?: boolean;
}

export function FormTextBox({ onChange, placeholder, value, isSecret, className, isError }: TextBoxProps) {
    const [isHidden, setIsHidden] = useState<boolean>(true);

    const basicStyles = `flex p-1 bg-background outline-2 focus:outline-active 
        outline-on-surface-outline rounded text-active ${className}`;

    return <div className={`${basicStyles} ${isError ? "outline-red-500 " : "outline-on-surface-outline "}`}>
        <input
            className={`flex-1 p-2 rounded`}
            type={isSecret && isHidden ? "password" : "text"}
            placeholder={placeholder}
            value={value}
            onChange={(e) => onChange(e.currentTarget.value)}
        />
        {isSecret && <IconButton className="flex ms-2"
            inverted={true} iconSrc={isHidden ? ClosedEye : OpenEye}
            onClick={() => { setIsHidden(!isHidden) }} />
        }
    </div>
}

export function HiddenTextBox({ onChange, placeholder, value, isSecret, className }: TextBoxProps) {
    return <input
        className={`p-2 bg-transparent border-b-transparent 
            focus:border-b-active outline-0 text-active ${className}`}
        type={isSecret ? "password" : "text"}
        placeholder={placeholder}
        value={value}
        onChange={(e) => onChange(e.currentTarget.value)}
    />
}

export default { FormTextBox, HiddenTextBox }
