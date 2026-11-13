const STORAGE_KEY = "HEM_ACTIVE_TAB";

const HEARTBEAT_INTERVAL = 1000;
const OWNER_TIMEOUT = 5000;

/* const TAB_ID = crypto.randomUUID();*/
let TAB_ID = sessionStorage.getItem("HEM_SESSION_TAB_ID");
if (!TAB_ID) {
    TAB_ID = crypto.randomUUID();
    sessionStorage.setItem("HEM_SESSION_TAB_ID", TAB_ID);
}

let heartbeatTimer = null;

/* -------------------------- */

function now() {
    return Date.now();
}

function readOwner() {

    const raw = localStorage.getItem(STORAGE_KEY);

    if (!raw)
        return null;

    try {

        return JSON.parse(raw);

    }
    catch {

        return null;

    }

}

function writeOwner() {

    localStorage.setItem(

        STORAGE_KEY,

        JSON.stringify({

            tabId: TAB_ID,

            heartbeat: now()

        })

    );

}

/* -------------------------- */

export function isOwner() {

    const owner = readOwner();

    return owner?.tabId === TAB_ID;

}

export function acquireOwnership() {

    const owner = readOwner();

    // No owner

    if (!owner) {

        writeOwner();

        return true;

    }

    // Already owner

    if (owner.tabId === TAB_ID) {

        writeOwner();

        return true;

    }

    // Owner expired

    if (

        now() - owner.heartbeat >

        OWNER_TIMEOUT

    ) {

        writeOwner();

        return true;

    }

    // Someone else alive

    return false;

}

/* -------------------------- */

export function startHeartbeat() {

    stopHeartbeat();

    writeOwner();

    heartbeatTimer = setInterval(() => {

        if (isOwner()) {

            writeOwner();

        }

    },

        HEARTBEAT_INTERVAL

    );

}

export function stopHeartbeat() {

    if (!heartbeatTimer)
        return;

    clearInterval(heartbeatTimer);

    heartbeatTimer = null;

}

/* -------------------------- */

export function initializeTabManager() {

    window.addEventListener(

        "beforeunload",

        () => {

            stopHeartbeat();

        }

    );

}