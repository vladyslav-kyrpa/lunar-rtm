import { useState } from "react"
import TextBox from "../shared/TextBox";
import ActiveButton from "../shared/ActiveButton";
import MinorButton from "../shared/MinorButton";
import { useNavigate } from "react-router-dom";

export default function RegisterPage() {
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [displayName, setDisplayName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmedPassword, setConfirmedPassword] = useState('');
    const [isError, setIsError] = useState(false);

    const handleSubmit = () => {
        if (isFormValid()) {
            console.log("Make an API call")
        } else {
            setIsError(true);
        }
    }

    const isFormValid = (): boolean => {
        return isUsernameValid(username)
            && isDisplayNameValid(displayName)
            && isEmailValid(email)
            && isPasswordValid(password)
            && password === confirmedPassword;
    }

    const handleToLogIn = () => {
        navigate('/log-in')
    }


    return <div className="h-screen flex flex-col justify-center items-center">
        <div className="flex flex-col p-10 bg-surface rounded-2xl w-[500px]">
            <p className="text-center font-bold mb-5">Register</p>

            <label className="mb-3">Username</label>
            <TextBox value={username} onChange={setUsername}
                placeholder="Enter username" style="mb-5"
                isError={isError && !isUsernameValid(username)} />

            <label className="mb-3">Display Name</label>
            <TextBox value={displayName} onChange={setDisplayName}
                placeholder="Enter Display Name" style="mb-5"
                isError={isError && !isDisplayNameValid(displayName)} />

            <label className="mb-3">Email</label>
            <TextBox value={email} onChange={setEmail}
                placeholder="Example: box@mail.com" style="mb-5"
                isError={isError && !isEmailValid(email)} />

            <label className="mb-3">Password</label>
            <TextBox value={password} onChange={setPassword}
                placeholder="Enter password" style="mb-5"
                isError={isError && !isPasswordValid(password)} />

            <label className="mb-3">Confirmed password</label>
            <TextBox value={confirmedPassword} onChange={setConfirmedPassword}
                placeholder="Enter password again" style="mb-10"
                isError={isError && password !== confirmedPassword || confirmedPassword === ''} />

            <ActiveButton onClick={handleSubmit} style="mb-3">Register</ActiveButton>
            <MinorButton onClick={handleToLogIn}>I have an account</MinorButton>
        </div>
    </div>
}

function isUsernameValid(value: string): boolean {
    return value !== "";
}

function isDisplayNameValid(value: string): boolean {
    return value !== "";
}

function isEmailValid(value: string): boolean {
    return value !== "";
}

function isPasswordValid(value: string): boolean {
    return value !== "";
}