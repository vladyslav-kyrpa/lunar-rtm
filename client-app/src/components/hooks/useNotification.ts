import { useState } from "react"

type UseNotificationReturn = [
  string,
  boolean,
  (text: string) => void,
  () => void
];
export default function useNotification(): UseNotificationReturn {
    const [text, setText] = useState("");
    const [isRised, setIsRised] = useState(false);

    const onShow = (text:string) => {
        setText(text);
        setIsRised(true);
    }
    const onHide = () => {
        setText("");
        setIsRised(false); 
    }

    return [ text, isRised, onShow, onHide ];
}
