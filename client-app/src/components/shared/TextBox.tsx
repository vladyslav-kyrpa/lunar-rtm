interface TextBoxProps {
    placeholder?: string,
    style?: string,
    value: string,
    isError?: boolean,
    onChange?: (value: string) => void
}

export default function TextBox({ placeholder, value, onChange, style, isError }: TextBoxProps) {
    return <input
        type="text"
        value={value}
        onChange={(e) => { if (onChange) onChange(e.currentTarget.value) }}
        placeholder={placeholder}
        className={"p-3 rounded w-full border-0 text-box-colors "
            + (style ?? '') + " "
            + (isError && "text-box-error-colors")} />
}