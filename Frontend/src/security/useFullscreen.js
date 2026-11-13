import { useCallback, useEffect } from "react";

export default function useFullscreen(
    addWarning,
    onFullscreenExit,
    onFullscreenRestored
) {


    // =====================================================
    // REQUEST FULLSCREEN
    // =====================================================

    const requestFullscreen = useCallback(async () => {

        try {

            if (!document.fullscreenElement) {

                await document.documentElement.requestFullscreen();

            }

            return true;

        }
        catch (error) {

            console.log(
                "Unable to enter fullscreen:",
                error
            );

            return false;

        }

    }, []);


    // =====================================================
    // FULLSCREEN CHANGE
    // =====================================================

    useEffect(() => {

        const handleFullscreenChange = () => {

            // ---------------------------------------------
            // USER EXITED FULLSCREEN
            // ---------------------------------------------

            if (!document.fullscreenElement) {

                addWarning(
                    "Fullscreen mode is mandatory."
                );

                if (onFullscreenExit) {

                    onFullscreenExit();

                }

                // IMPORTANT:
                // DO NOT automatically call requestFullscreen()
                //
                // Browser requires a user gesture.
                //
                // User will click:
                // "Return to Fullscreen"

                return;

            }


            // ---------------------------------------------
            // FULLSCREEN RESTORED
            // ---------------------------------------------

            if (onFullscreenRestored) {

                onFullscreenRestored();

            }

        };


        document.addEventListener(
            "fullscreenchange",
            handleFullscreenChange
        );


        return () => {

            document.removeEventListener(
                "fullscreenchange",
                handleFullscreenChange
            );

        };

    }, [
        addWarning,
        onFullscreenExit,
        onFullscreenRestored
    ]);


    return {

        requestFullscreen

    };

}