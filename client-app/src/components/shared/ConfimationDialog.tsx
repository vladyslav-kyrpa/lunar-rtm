import { Block } from "./Blocks"
import { ActiveButton, MinorButton } from "./Buttons"

interface ConfirmationDialogProps {
    show:boolean,
    title:string,
    text:string,
    onResponse:(confirmed:boolean)=>void
}
export default function ConfirmationDialog({title, text, onResponse, show}:ConfirmationDialogProps) {
    if(!show) return <></>
    return <div className="w-screen h-screen absolute left-0 rigth-0 top-0 bottom-0 flex flex-col items-center justify-center">
        <div className="bg-black opacity-80 w-full h-full absolute">

        </div>
        <Block className="w-[300px] z-100">
            <p className="text-center font-bold mb-2">{title}</p>
            <p className="text-center mb-5">{text}</p>
            <div className="flex gap-2">
            <ActiveButton className="flex-1" onClick={()=>onResponse(false)}>Cancel</ActiveButton>
            <MinorButton className="flex-1" onClick={()=>onResponse(true)}>Confirm</MinorButton>
            </div>
        </Block>
    </div>
}