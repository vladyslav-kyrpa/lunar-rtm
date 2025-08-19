import { useEffect, useState, type ReactNode } from "react";
import api from "../../api/AuthApi";
import LoadingScreen from "./LoadingScreen";
import { Navigate, useLocation } from "react-router-dom";
import ErrorPage from "../pages/ErrorPage";

interface ProtectedRouteProps {
    children: ReactNode;
}

export default function ProtectedRoute({children}:ProtectedRouteProps) {
    const [authState, setState]= useState<"loading"|"valid"|"not-valid"|"error">("loading");
    const location = useLocation();

    useEffect(()=>{
        api.isAuthenticated().then((result)=>{
            setState(result? "valid" : "not-valid");
        }).catch((error)=>{
            setState("error");
            console.error("Failed to check authentication", error);
        });
    },[location]);

    if(authState == "loading") return <LoadingScreen/>
    if(authState == "valid") return children;
    if(authState == "error") return <ErrorPage 
        message="Failed to check authentication. Try again a bit later."/>

    return <Navigate to={"/log-in"} replace/>
}