import { useEffect } from "react";

import { useNavigate } from "react-router-dom";

import {

    acquireOwnership,

    startHeartbeat,

    isOwner

} from "../security/ActiveTabManager";

export default function DuplicateTabPage() {

    const navigate = useNavigate();

    useEffect(() => {

        const timer = setInterval(() => {

            const owner = acquireOwnership();

            if (owner && isOwner()) {

                startHeartbeat();

                navigate("/dashboard", {

                    replace: true

                });

            }

        }, 2000);

        return () => clearInterval(timer);

    }, [navigate]);

    return (

        <div className="container d-flex justify-content-center align-items-center min-vh-100">

            <div className="card shadow-lg p-5 text-center">

                <h2 className="text-danger">

                    Another Exam Window Detected

                </h2>

                <hr />

                <p>

                    Your examination is already running in another browser tab.

                </p>

                <p>

                    Please close the existing exam window.

                </p>

                <div className="mt-4">

                    <div className="spinner-border text-primary" />

                </div>

                <p className="mt-3 text-muted">

                    Waiting for ownership...

                </p>

            </div>

        </div>

    );

}