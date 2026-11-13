import { useEffect } from "react";

export default function useKeyboard(addWarning) {

    useEffect(() => {

        const handleKeyDown = (e) => {

            // =================================================
            // F12
            // =================================================

            if (e.key === "F12") {

                e.preventDefault();

                addWarning(
                    "F12 is not allowed."
                );

                return;
            }


            // =================================================
            // CTRL + SHIFT + I / J / C
            // =================================================

            if (
                e.ctrlKey &&
                e.shiftKey &&
                ["I", "J", "C"].includes(
                    e.key.toUpperCase()
                )
            ) {

                e.preventDefault();

                addWarning(
                    "Developer tools are not allowed."
                );

                return;
            }


            // =================================================
            // CTRL + U
            // =================================================

            if (
                e.ctrlKey &&
                e.key.toUpperCase() === "U"
            ) {

                e.preventDefault();

                addWarning(
                    "View Source is not allowed."
                );

                return;
            }

        };


        // =================================================
        // RIGHT CLICK
        // =================================================

        const handleContextMenu = (e) => {

            e.preventDefault();

            addWarning(
                "Right click is disabled."
            );

        };


        window.addEventListener(
            "keydown",
            handleKeyDown
        );

        document.addEventListener(
            "contextmenu",
            handleContextMenu
        );


        return () => {

            window.removeEventListener(
                "keydown",
                handleKeyDown
            );

            document.removeEventListener(
                "contextmenu",
                handleContextMenu
            );

        };

    }, [addWarning]);

}