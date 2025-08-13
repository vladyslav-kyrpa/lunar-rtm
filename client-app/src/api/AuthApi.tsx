export async function register(username: string, displayName: string, password: string, email: string) {
    var result = await fetch("/api/auth/register", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            userName: username,
            displayName: displayName,
            password: password,
            email: email
        })
    });
    if (!result.ok) {
        throw Error(result.statusText);
    }
}

export async function login(username: string, password: string) {
    var result = await fetch("/api/auth/log-in", {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            userName: username,
            password: password
        })
    });
    if (!result.ok) {
        throw Error(result.statusText);
    }
}

export default { register, login }