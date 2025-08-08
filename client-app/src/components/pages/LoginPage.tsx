import { useState } from "react"
import { useNavigate } from "react-router-dom";
import { ActiveButton, TextButton } from "../shared/Buttons";
import { FormTextBox } from "../shared/TextBoxes";
import { Block } from "../shared/Blocks";
import { GoogleLogin, GoogleOAuthProvider, type CredentialResponse } from "@react-oauth/google";
import api from "../../api/AuthApi";

export default function RegisterPage() {
    const navigate = useNavigate();
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [isError, setIsError] = useState(false);

    const clientId: string = import.meta.env.VITE_GOOGLE_CLIENT_ID;
    console.log(clientId);

    const handleGoogleLogin = (credentialResponse: CredentialResponse) => {
        if(!credentialResponse.credential){
            console.error("No credentials");
            return;
        }
        api.googleSignIn(credentialResponse.credential).then((_)=>{
            navigate("/");
        }).catch((error)=>{
            console.error(error);
        }); 
    }

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
        <Block className="flex flex-col w-[400px]">
            <p className="text-center font-bold mb-5">Log-in</p>

            <label className="mb-3">Login</label>
            <FormTextBox value={login} onChange={setLogin}
                placeholder="Enter your username or email" className="mb-5"
                isError={isError && !isUsernameValid(login)} />

            <label className="mb-3">Password</label>
            <FormTextBox value={password} onChange={setPassword}
                placeholder="Enter password" className="mb-10"
                isError={isError && !isPasswordValid(password)} />

            <ActiveButton onClick={handleSubmit} className="mb-3">Log-in</ActiveButton>
            <p className="text-center mb-3">--- or authenticate with ---</p>
            <GoogleOAuthProvider clientId={clientId}>
                <div className="w-full flex items-center justify-center mb-3">
                    <GoogleLogin
                        onSuccess={handleGoogleLogin} 
                        onError={()=>{console.log("Failed to auth with google")}}/>
                </div>
            </GoogleOAuthProvider>
            <TextButton onClick={handleToRegister} text="I don't have an account"/>
        </Block>
    </div>
}

function isUsernameValid(value: string): boolean {
    return value !== "";
}

function isPasswordValid(value: string): boolean {
    return value !== "";
}