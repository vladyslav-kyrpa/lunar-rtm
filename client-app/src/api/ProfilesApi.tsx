import type UserProfile from "../data-model/UserProflie";

async function fetchProfile(username: string): Promise<UserProfile> {
    const profile:UserProfile = {
        username:"reglar-user",
        displayName: "Regular-user",
        avatarUrl: "",
        bio: "I'am just a user"
    }
    return profile;
}

async function fetchCurrentProfile(): Promise<UserProfile> {
    const profile:UserProfile = {
        username:"reglar-user",
        displayName: "Regular-user",
        avatarUrl: "",
        bio: "I'am just a user"
    }
    return profile;
}

async function updateProfile(username:string, newUsername?:string, newDisplayName?:string, newBio?:string, image?:File) {
    console.log("update profile"); 
}

async function createProfile(username:string, displayName:string, bio:string, image?:File){
    console.log("register new user");
}

export default { fetchProfile, fetchCurrentProfile, updateProfile, createProfile }