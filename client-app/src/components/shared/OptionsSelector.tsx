import { useState } from "react"

interface Option<T> {
    value:T,
    label:string
}

interface OptionsSelectorProps<T> {
    options: Option<T>[],
    onSelected: (value: T) => void
    className?: string
}
export default function OptionsSelector<T>({ options, onSelected, className }: OptionsSelectorProps<T>) {
    const [selectedItem, setSelectedItem] = useState(0);

    const handleSelection = (index: number) => {
        if (index == selectedItem) return;
        setSelectedItem(index);
        onSelected(options[index].value);
    }

    return <div className={className}>
        {options.map((item, key) => {
            return <div key={key} className="flex p-2 items-center rounded hover:bg-on-surface" onClick={() => handleSelection(key)}>
                {selectedItem == key
                    ? <div className="h-3 w-3 rounded-full bg-active"></div>
                    : <div className="h-3 w-3 rounded-full border-2 border-active"></div>
                }
                <p className="ms-2">{item.label}</p>
            </div>
        })}
    </div>;
}