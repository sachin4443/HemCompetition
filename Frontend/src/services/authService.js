import api from "./api";

export const verifyAuthToken = async () => {

    const token = localStorage.getItem("jwt_token");

    if (!token)
        return false;

    try {

        await api.post("/auth/heartbeat");

        return true;

    }
    catch {

        localStorage.clear();
        sessionStorage.clear();

        return false;
    }

};