import { useEffect, useMemo } from "react";
import { Navigate } from "react-router-dom";

import {
    initializeTabManager,
    acquireOwnership,
    startHeartbeat,
    stopHeartbeat
} from "../../security/ActiveTabManager";

export default function ProtectedRoute({ children }) {
    // Synchronous Auth Check
    const status = useMemo(() => {
        const token = localStorage.getItem("jwt_token");
        if (!token) return "unauthorized";

        initializeTabManager();
        const owner = acquireOwnership();
        if (!owner) return "duplicate";

        return "authorized";
    }, []);

    useEffect(() => {
        if (status === "authorized") {
            startHeartbeat();
        }

        return () => {
            stopHeartbeat();
        };
    }, [status]);

    if (status === "unauthorized") {
        return <Navigate to="/" replace />;
    }

    if (status === "duplicate") {
        return <Navigate to="/duplicate-tab" replace />;
    }

    return children;
}