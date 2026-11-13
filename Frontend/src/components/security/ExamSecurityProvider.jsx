import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import {
    initializeTabManager,
    acquireOwnership,
    isOwner
} from "../../security/ActiveTabManager";

import {
    sendMessage,
    subscribe
} from "../../security/BroadcastManager";

export default function ExamSecurityProvider({ children }) {

    const navigate = useNavigate();

    const [ready, setReady] = useState(false);

    useEffect(() => {

        initializeTabManager();

        sendMessage("CHECK_OWNER");

        const timer = setTimeout(() => {

            const owner = acquireOwnership();

            if (!owner) {

                navigate("/duplicate-tab", { replace: true });

                return;

            }

            setReady(true);

        }, 500);

        subscribe((message) => {

            switch (message.type) {

                case "OWNER_PRESENT":

                    if (!isOwner()) {

                        navigate("/duplicate-tab", { replace: true });

                    }

                    break;

                default:

                    break;

            }

        });

        return () => {

            clearTimeout(timer);

        };

    }, [navigate]);

    if (!ready) {

        return (

            <div className="container d-flex justify-content-center align-items-center min-vh-100">

                <div className="spinner-border text-success"/>

            </div>

        );

    }

    return children;

}