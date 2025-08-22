import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type UserProfile from "../../../data-model/UserProflie";
import api from "../../../api/ProfilesApi";
import { useAuth } from "../../hooks/AuthContext";

type UseProfileReturn = {
    profile: UserProfile | null,
    updateProfile: (username:string, displayName:string, bio:string) => Promise<void>,
    deleteProfile: () => Promise<void>,
    updateImage: (image:File) => Promise<void> 
}

const ProfileContext = createContext<UseProfileReturn | null>(null);

interface ProfileProviderProps {
    username: string,
    children: ReactNode
}

export function ProfileProvider({username, children}:ProfileProviderProps) {
    const profile = useProfile(username);
    return <ProfileContext.Provider value={profile}>
        {children}
    </ProfileContext.Provider>
}


export function useProfileContext(): UseProfileReturn {
    const context = useContext(ProfileContext);
    if(!context) throw Error("context should be inside ProfileProvider");
    return context;
}

export function useProfile(username?:string): UseProfileReturn {
    const [profile, setProfile] = useState<UserProfile|null>(null);
    const { username:currentUser } = useAuth();

    useEffect(()=>{
        if (username === null) {
            api.fetchCurrentProfile().then((r) => {
                setProfile(r);
            }).catch((e) => {
                console.error(e);
            });
        } else {
            api.fetchProfile(username ?? "").then((r) => {
                setProfile(r);
            }).catch((e) => {
                console.error(e);
            });
        }
    },[]);
   
    const onUpdate = async (username:string, displayName:string, bio:string) => {
        let userToUpdate = !username ? currentUser : profile?.username;
        if(!userToUpdate) return;

        api.updateProfile(userToUpdate, username, displayName, bio).then((result) => {
            console.log(result);
        }).catch((error) => {
            console.error(error);
        });
    }

    const onDelete = async () => {
        let userToDelete = !username ? currentUser : profile?.username;
        if(!userToDelete) return;

        api.deleteProfile(userToDelete).then((result)=>{
            console.log(result);
        }).catch((error)=>{
            console.error(error);
        });
    }

    const onUpdateImage = async (image:File) => {
        if(!profile) return;
        api.updateImage(profile?.username, image).then((result) => {
            console.log(result)
        }).catch((e) => {
            console.error(e);
        });
    }

    return { profile, 
        updateProfile:onUpdate, 
        deleteProfile:onDelete,
        updateImage: onUpdateImage
    };
}
