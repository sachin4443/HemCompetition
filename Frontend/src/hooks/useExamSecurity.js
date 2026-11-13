import { useCallback, useEffect, useRef, useState } from "react";

import useFullscreen from "../security/useFullscreen";
import useKeyboard from "../security/useKeyboard";
import useVisibility from "../security/useVisibility";

export default function useExamSecurity(onTerminate) {

    const [warningCount, setWarningCount] = useState(0);
    const [warningMessage, setWarningMessage] = useState("");
    const [fullscreenExited, setFullscreenExited] = useState(false);

    const terminateStartedRef = useRef(false);
    const warningTimerRef = useRef(null);


    // =====================================================
    // ADD SECURITY WARNING
    // =====================================================

    const addWarning = useCallback((message) => {

        if (terminateStartedRef.current) {
            return;
        }

        console.log("Security Warning:", message);

        setWarningMessage(message);

        // Clear previous timer
        if (warningTimerRef.current) {
            clearTimeout(warningTimerRef.current);
        }

        // Normal warning popup stays for 3 seconds
        warningTimerRef.current = setTimeout(() => {

            setWarningMessage("");

        }, 3000);


        setWarningCount((prev) => {

            return prev + 1;

        });

    }, []);


    // =====================================================
    // FULLSCREEN EXIT
    // =====================================================

    const handleFullscreenExit = useCallback(() => {

        if (terminateStartedRef.current) {
            return;
        }

        setFullscreenExited(true);

    }, []);


    // =====================================================
    // FULLSCREEN RESTORED
    // =====================================================

    const handleFullscreenRestored = useCallback(() => {

        setFullscreenExited(false);

    }, []);


    // =====================================================
    // FULLSCREEN SECURITY
    // =====================================================

    const {
        requestFullscreen
    } = useFullscreen(
        addWarning,
        handleFullscreenExit,
        handleFullscreenRestored
    );


    // =====================================================
    // KEYBOARD SECURITY
    // =====================================================

    useKeyboard(addWarning);


    // =====================================================
    // VISIBILITY SECURITY
    // =====================================================

    useVisibility(addWarning);


    // =====================================================
    // TERMINATE WHEN 10 WARNINGS REACHED
    // =====================================================

    useEffect(() => {

        if (warningCount < 10) {
            return;
        }

        if (terminateStartedRef.current) {
            return;
        }

        terminateStartedRef.current = true;

        console.log(
            "Maximum security warnings (10) reached."
        );


        // Give user time to see 10 / 10
        const timer = setTimeout(() => {

            if (onTerminate) {

                onTerminate();

            }

        }, 1800);


        return () => {

            clearTimeout(timer);

        };

    }, [
        warningCount,
        onTerminate
    ]);


    // =====================================================
    // CLEANUP
    // =====================================================

    useEffect(() => {

        return () => {

            if (warningTimerRef.current) {

                clearTimeout(
                    warningTimerRef.current
                );

            }

        };

    }, []);


    return {

        warningCount,
        warningMessage,

        fullscreenExited,

        requestFullscreen

    };

}