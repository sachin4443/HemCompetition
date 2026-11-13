let fullscreenEnabled = false;

export async function enterFullscreen() {
    const el = document.documentElement;

    try {
        if (!document.fullscreenElement) {
            await el.requestFullscreen();
        }

        fullscreenEnabled = true;
        return true;
    } catch {
        return false;
    }
}

export async function exitFullscreen() {
    try {
        if (document.fullscreenElement) {
            await document.exitFullscreen();
        }

        fullscreenEnabled = false;
    } catch { }
}

export function isFullscreen() {
    return !!document.fullscreenElement;
}

export function onFullscreenChange(callback) {
    document.addEventListener("fullscreenchange", callback);

    return () =>
        document.removeEventListener(
            "fullscreenchange",
            callback
        );
}

export function isFullscreenEnabled() {
    return fullscreenEnabled;
}