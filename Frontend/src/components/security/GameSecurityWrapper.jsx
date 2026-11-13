import { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import useExamSecurity from "../../hooks/useExamSecurity";
import SecurityWarningModal from "./SecurityWarningModal";

import {
    initializeTabManager,
    acquireOwnership,
    isOwner,
    startHeartbeat,
    stopHeartbeat
} from "../../security/ActiveTabManager";

import {
    sendMessage,
    subscribe
} from "../../security/BroadcastManager";


export default function GameSecurityWrapper({
    children
}) {

    const navigate = useNavigate();

    const [ready, setReady] = useState(false);


    // =====================================================
    // DUPLICATE TAB PROTECTION
    // =====================================================

    useEffect(() => {

        initializeTabManager();

        sendMessage("CHECK_OWNER");


        const timer = setTimeout(() => {

            const owner = acquireOwnership();


            if (!owner) {

                navigate(
                    "/duplicate-tab",
                    {
                        replace: true
                    }
                );

                return;

            }


            startHeartbeat();

            setReady(true);

        }, 300);


        const unsubscribe = subscribe((message) => {

            switch (message.type) {

                case "OWNER_PRESENT":

                    if (!isOwner()) {

                        stopHeartbeat();

                        navigate(
                            "/duplicate-tab",
                            {
                                replace: true
                            }
                        );

                    }

                    break;


                case "OWNER_CHANGED":

                    if (!isOwner()) {

                        stopHeartbeat();

                        navigate(
                            "/duplicate-tab",
                            {
                                replace: true
                            }
                        );

                    }

                    break;


                default:

                    break;

            }

        });


        return () => {

            clearTimeout(timer);

            stopHeartbeat();

            unsubscribe();

        };

    }, [navigate]);


    // =====================================================
    // TERMINATE EXAM
    // =====================================================

    const handleTerminateExam = useCallback(() => {

        console.log(
            "Exam terminated due to security violations."
        );


        // ---------------------------------------------
        // Stop active tab heartbeat
        // ---------------------------------------------

        stopHeartbeat();


        // ---------------------------------------------
        // Exit fullscreen
        // ---------------------------------------------

        if (document.fullscreenElement) {

            document.exitFullscreen()
                .catch(() => {});

        }


        // ---------------------------------------------
        // Clear authentication
        // ---------------------------------------------

        localStorage.clear();

        sessionStorage.clear();


        // ---------------------------------------------
        // Go to LOGIN
        // ---------------------------------------------

        navigate(
            "/",
            {
                replace: true
            }
        );

    }, [navigate]);


    // =====================================================
    // EXAM SECURITY
    // =====================================================

    const {
        warningCount,
        warningMessage,
        fullscreenExited,
        requestFullscreen
    } = useExamSecurity(
        handleTerminateExam
    );


    // =====================================================
    // WAIT FOR TAB OWNERSHIP
    // =====================================================

    if (!ready) {

        return null;

    }


    // =====================================================
    // RENDER
    // =====================================================

    return (

        <>

            {/* =================================================
                SECURITY WARNING POPUP
            ================================================= */}

            {warningMessage && (

                <SecurityWarningModal
                    open={true}
                    title="Security Warning"
                    message={warningMessage}
                    warnings={warningCount}
                    maxWarnings={10}
                    fullscreenLost={fullscreenExited}
                    onEnterFullscreen={requestFullscreen}
                />

            )}


            {/* =================================================
                FULLSCREEN LOST
            ================================================= */}

            {fullscreenExited &&
                !warningMessage && (

                    <SecurityWarningModal
                        open={true}
                        title="Fullscreen Required"
                        message="You exited fullscreen mode."
                        warnings={warningCount}
                        maxWarnings={10}
                        fullscreenLost={true}
                        onEnterFullscreen={requestFullscreen}
                    />

                )}


            {/* =================================================
                GAME CONTENT
            ================================================= */}

            {children}

        </>

    );

}