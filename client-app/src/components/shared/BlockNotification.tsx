interface NotificationProps {
    text: string;
    className?:string
}
export default function BlockNotification({text, className}:NotificationProps){
    let colors = "bg-active border-outline text-black";
    return <div className={"p-4 text-center rounded " + colors + " " + className}>
        {text}
    </div>    
}