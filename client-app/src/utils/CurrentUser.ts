export function getCurrentUsername() {
    return localStorage.getItem("current-username");
}

export function setCurrentUsername(value:string) {
    return localStorage.setItem("current-username", value);
}

export default { getCurrentUsername, setCurrentUsername };