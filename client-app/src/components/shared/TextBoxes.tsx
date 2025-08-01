interface TextBoxProps {
    onChange: (value: string) => void;
    placeholder?: string;
    value: string;
    isSecret?: boolean;
    className?: string;
    isError?:boolean;
}

export function FormTextBox({ onChange, placeholder, value, isSecret, className, isError }: TextBoxProps) {
    const basicStyles = "p-2 bg-background outline-2 focus:outline-active outline-on-surface-outline rounded text-active " + className;
    return <input
        className={ (isError ? "outline-red-500 " : "outline-on-surface-outline ") + basicStyles }
        type={isSecret ? "password" : "text"}
        placeholder={placeholder}
        value={value}
        onChange={(e) => onChange(e.currentTarget.value)}
    />
}

export function HiddenTextBox({ onChange, placeholder, value, isSecret, className }: TextBoxProps) {
    return <input
        className={"p-2 bg-transparent border-b-transparent focus:border-b-active outline-0 text-active " + className}
        type={isSecret ? "password" : "text"}
        placeholder={placeholder}
        value={value}
        onChange={(e) => onChange(e.currentTarget.value)}
    />
}

export default { FormTextBox, HiddenTextBox }
