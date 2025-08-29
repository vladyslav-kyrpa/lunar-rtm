import Logo from "../../assets/logo.png";

export default function LoadingScreen() {
    return <div className="h-screen flex items-center justify-center">
        <img src={Logo} height={200} width={200} className="mb-5"/>
    </div>
}