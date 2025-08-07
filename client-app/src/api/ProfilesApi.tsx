import type UserProfile from "../data-model/UserProflie";

const url = "http://localhost:5219";

async function fetchProfile(username: string): Promise<UserProfile> {
    const result = await fetch(url+"/profiles/" + username);
    if(result.ok){
        return (await result.json()) as UserProfile; 
    } else {
        throw Error("Failed to get current user profile: " + result.statusText);
    }
}

async function fetchCurrentProfile(): Promise<UserProfile> {
    const result = await fetch(url+"/profiles/me");
    if(result.ok){
        return (await result.json()) as UserProfile; 
    } else {
        throw Error("Failed to get current user profile: " + result.statusText);
    }
}


async function updateProfile(username:string, newUsername:string, newDisplayName:string, newBio:string) {
    const result = await fetch(url+"/profiles/" + username + "/update",{
        method:"POST",
        headers:{
            "Content-Type":"application/json"
        },
        body: JSON.stringify({
            username:newUsername, 
            displayName:newDisplayName, 
            bio:newBio
        })
    });
    if(!result.ok){
        throw Error("Failed to update profile: " + result.statusText);
    }
}

async function createProfile(username:string, displayName:string, email:string,  password:string){
    const result = await fetch(url+"/profiles/create", {
        method:"POST",
        headers: {
            "Content-Type":"application/json"
        },
        body: JSON.stringify({username, displayName, password, email})
    });
    if(!result.ok){
        throw Error("Failed to create new profile: " + result.statusText);
    }
}

async function updateImage(username:string, image:File) {
    const formData = new FormData();
    formData.append("image", image);
    const result = await fetch(url + "/profiles/" + username + "/update-image", {
        method:"POST",
        headers: {
            //"Content-Type":"multipart/form-data"
        },
        body: formData 
    });
    if(!result.ok){
        throw Error("Failed to update profile image: " + result.statusText);
    }
}

export default { fetchProfile, fetchCurrentProfile, updateProfile, createProfile, updateImage}