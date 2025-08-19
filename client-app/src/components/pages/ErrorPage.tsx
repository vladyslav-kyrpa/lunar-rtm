
interface ErrorPageProps {
    message:string
}
export default function ErrorPage({message}:ErrorPageProps) {
    return <div className="h-screen flex flex-col items-center justify-center">
        <p className="text-2x1 font-bold mb-5 text-center">Oops. Something went wrong</p>
        <p className="text-center">{message}</p>
    </div>
}