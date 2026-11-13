import {
    useEffect,
    useMemo,
    useState
} from "react";

import {
    enterFullscreen,
    isFullscreen,
    onFullscreenChange
} from "./FullscreenManager";

import { SecurityContext } from "./SecurityContext";

import SecurityWarningModal from "./SecurityWarningModal";

import useKeyboard from "./useKeyboard";
import useVisibility from "./useVisibility";

export default function ExamSecurityProvider({ children }) {

    const [warnings, setWarnings] = useState(0);

    const [message, setMessage] = useState("");

    const [open, setOpen] = useState(false);

    const addWarning = async (reason) => {

        setMessage(reason);

        setOpen(true);

        setTimeout(() => {

            setOpen(false);

        },3000);

        setWarnings(prev=>{

            const total=prev+1;

            console.log(reason);

            if(total>=3){

                console.log("Maximum Warnings");

                // Later Auto Submit

            }

            return total;

        });

        await enterFullscreen();

    };

    useKeyboard(addWarning);

    useVisibility(addWarning);

    useEffect(()=>{

        enterFullscreen();

        const remove=onFullscreenChange(()=>{

            if(!isFullscreen()){

                addWarning("Fullscreen exited");

            }

        });

        return ()=>remove();

    },[]);

    const value=useMemo(()=>({

        warnings,

        addWarning

    }),[warnings]);

    return(

        <SecurityContext.Provider value={value}>

            {children}

            <SecurityWarningModal

                open={open}

                warnings={warnings}

                title="Security Warning"

                message={message}

            />

        </SecurityContext.Provider>

    );

}