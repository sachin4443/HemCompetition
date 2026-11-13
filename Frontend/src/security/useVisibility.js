import { useEffect } from "react";

export default function useVisibility(addWarning) {

    useEffect(() => {

        // =================================================
        // TAB SWITCH
        // =================================================

        const handleVisibility = () => {

            if (document.hidden) {

                addWarning(
                    "Tab switching detected."
                );

            }

        };


        // =================================================
        // WINDOW FOCUS LOST
        // =================================================

        const handleBlur = () => {

            addWarning(
                "Window focus lost."
            );

        };


        document.addEventListener(
            "visibilitychange",
            handleVisibility
        );

        window.addEventListener(
            "blur",
            handleBlur
        );


        return () => {

            document.removeEventListener(
                "visibilitychange",
                handleVisibility
            );

            window.removeEventListener(
                "blur",
                handleBlur
            );

        };

    }, [addWarning]);

}