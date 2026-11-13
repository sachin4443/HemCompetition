import { Navigate, Outlet } from "react-router-dom";
import CameraSecurityProvider from "./CameraSecurityProvider";

export default function AuthenticatedSecurityLayout() {

    const token =
        localStorage.getItem("jwt_token");

    // No login session
    if (!token) {

        return (
            <Navigate
                to="/"
                replace
            />
        );

    }

    return (

        <CameraSecurityProvider>

            <Outlet />

        </CameraSecurityProvider>

    );

}