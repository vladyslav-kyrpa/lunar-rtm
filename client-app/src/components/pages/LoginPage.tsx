import { useState } from "react"
import { useNavigate } from "react-router-dom";
import TextBox from "../shared/TextBox";
import ActiveButton from "../shared/ActiveButton";
import TextButton from "../shared/TextButton";
import GoogleIcon from "../../assets/icons/google.svg";

export default function RegisterPage() {
    const navigate = useNavigate();
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [isError, setIsError] = useState(false);

    const handleSubmit = () => {
        if (password !== '' && login !== '') {
            // todo: make an API call
        } else {
            setIsError(true);
        }
    }

    const handleToRegister = () => {
        navigate('/register');
    }

    return <div className="h-screen flex flex-col justify-center items-center">
        <div className="flex flex-col w-[400px]">
            <p className="text-center font-bold mb-5">Log-in</p>

            <label className="mb-3">Login</label>
            <TextBox value={login} onChange={setLogin}
                placeholder="Enter your username or email" style="mb-5"
                isError={isError && !isUsernameValid(login)} />

            <label className="mb-3">Password</label>
            <TextBox value={password} onChange={setPassword}
                placeholder="Enter password" style="mb-10"
                isError={isError && !isPasswordValid(password)} />

            <ActiveButton onClick={handleSubmit} style="mb-5">Log-in</ActiveButton>

            <p className="text-center mb-3">--- or authenticate with ---</p>
            <ActiveButton onClick={handleSubmit} style="mb-3"> 
                <div className="flex items-center justify-center space-x-3">
                    <img src={GoogleIcon} alt="google-icon" width={24} height={24} />
                    <p>Google Account</p>
                </div>
            </ActiveButton>
            <TextButton onClick={handleToRegister}>I don't have an account</TextButton>
        </div>
    </div>
}

function isUsernameValid(value: string): boolean {
    return value !== "";
}

function isPasswordValid(value: string): boolean {
    return value !== "";
}