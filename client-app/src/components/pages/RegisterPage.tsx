import { useState } from "react"
import { useNavigate } from "react-router-dom";
import { FormTextBox } from "../shared/TextBoxes";
import { ActiveButton, TextButton } from "../shared/Buttons";
import { Block } from "../shared/Blocks";
import api from "../../api/AuthApi";
import useNotification from "../hooks/UseNotification";
import BlockNotification from "../shared/BlockNotification";

export default function RegisterPage() {
    const navigate = useNavigate();

    const [username, setUsername] = useState('');
    const [displayName, setDisplayName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmedPassword, setConfirmedPassword] = useState('');

    const [errorText, isRised, showError] = useNotification();

    const handleSubmit = () => {
        if (isFormValid()) {
            api.register(username, displayName, password, email).then((result) => {
                console.log(result);
                navigate("/log-in");
            }).catch((error: Error) => {
                console.error(error.message);
                showError(error.message);
            });
        } 
        else showError("Fill all the required fields with correct values");
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


    return <div className="h-screen flex justify-center items-center">
        <Block className="flex flex-col w-[400px]">
            <p className="text-center font-bold mb-5">Register</p>

            {isRised && <BlockNotification className="mb-3" text={errorText} />}
            <form>
                <label className="mb-3">Username</label>
                <FormTextBox value={username} onChange={setUsername}
                    placeholder="Enter username" className="mb-5"
                    isError={!isUsernameValid(username)} />

                <label className="mb-3">Display Name</label>
                <FormTextBox value={displayName} onChange={setDisplayName}
                    placeholder="Enter Display Name" className="mb-5"
                    isError={!isDisplayNameValid(displayName)} />

                <label className="mb-3">Email</label>
                <FormTextBox value={email} onChange={setEmail}
                    placeholder="Example: box@mail.com" className="mb-5"
                    isError={!isEmailValid(email)} />

                <label className="mb-3">Password</label>
                <FormTextBox value={password} onChange={setPassword}
                    isSecret={true}
                    placeholder="Enter password" className="mb-5"
                    isError={!isPasswordValid(password)} />

                <label className="mb-3">Confirmed password</label>
                <FormTextBox value={confirmedPassword} onChange={setConfirmedPassword}
                    isSecret={true}
                    placeholder="Enter password again" className="mb-10"
                    isError={password !== confirmedPassword || confirmedPassword === ''} />

                <ActiveButton onClick={handleSubmit} className="mb-3">Register</ActiveButton>
                <TextButton onClick={handleToLogIn} text="I have an account" />
            </form>
        </Block>
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