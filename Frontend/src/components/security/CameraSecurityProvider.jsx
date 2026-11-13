import {
    useCallback,
    useEffect,
    useRef,
    useState
} from "react";

import {
    FaceDetector,
    FilesetResolver
} from "@mediapipe/tasks-vision";

import api from "../../services/api";

const MODEL_PATH =
    "/models/blaze_face_short_range.tflite";

const WASM_PATH =
    "https://cdn.jsdelivr.net/npm/@mediapipe/tasks-vision@0.10.21/wasm";


// ============================================================
// TIMINGS
// ============================================================

const DETECTION_INTERVAL_MS = 300;

// Multiple person -> 10 seconds
const MULTIPLE_PERSON_LIMIT_MS = 10 * 1000;

// No person -> 30 seconds
const NO_PERSON_LIMIT_MS = 10 * 1000;


// ============================================================
// COMPONENT
// ============================================================

export default function CameraSecurityProvider({ children }) {

    // ========================================================
    // REFS
    // ========================================================
    const videoRef = useRef(null);

    const streamRef = useRef(null);

    const detectorRef = useRef(null);

    const animationRef = useRef(null);

    const mountedRef = useRef(true);

    const lastDetectionRef = useRef(0);

    const noFaceSinceRef = useRef(null);

    const multipleFaceSinceRef = useRef(null);

    const disqualifyingRef = useRef(false);

    const disqualifiedRef = useRef(false);

    const lastServerCameraStateRef = useRef(null);

    const lastServerPersonCountRef = useRef(null);

    const cameraStatusRequestInProgressRef =
        useRef(false);


    // ========================================================
    // STATE
    // ========================================================

    const [cameraState, setCameraState] =
        useState("initializing");

    const [securityMessage, setSecurityMessage] =
        useState("Requesting camera permission...");

    const [violationCountdown, setViolationCountdown] =
        useState(null);

    const [disqualified, setDisqualified] =
        useState(false);


    // ========================================================
    // UPDATE CAMERA STATUS IN DATABASE
    //
    // IMPORTANT:
    // This does NOT call API every 300ms.
    //
    // It only calls when camera/person state actually changes.
    // ========================================================

    const updateCameraStatus = useCallback(
        async (
            cameraStatus,
            personCount
        ) => {

            if (!mountedRef.current) {
                return;
            }

            if (disqualifiedRef.current) {
                return;
            }


            // ------------------------------------------------
            // Avoid duplicate requests
            // ------------------------------------------------

            if (
                lastServerCameraStateRef.current === cameraStatus &&
                lastServerPersonCountRef.current === personCount
            ) {
                return;
            }


            // If previous request still running,
            // don't create another request.
            if (
                cameraStatusRequestInProgressRef.current
            ) {
                return;
            }


            cameraStatusRequestInProgressRef.current =
                true;


            try {

                await api.post(
                    "/CompetitionSecurity/camera-status",
                    {
                        cameraStatus,
                        personCount
                    }
                );


                lastServerCameraStateRef.current =
                    cameraStatus;

                lastServerPersonCountRef.current =
                    personCount;

            }
            catch (error) {

                console.error(
                    "Camera status update failed:",
                    error
                );

            }
            finally {

                cameraStatusRequestInProgressRef.current =
                    false;

            }

        },
        []
    );


    // ========================================================
    // DISQUALIFY USER
    // ========================================================

    const disqualifyUser = useCallback(
        async ({
            violationType,
            violationMessage,
            cameraStatus,
            personCount
        }) => {

            // ------------------------------------------------
            // Prevent duplicate disqualification calls
            // ------------------------------------------------

            if (disqualifyingRef.current) {
                return;
            }

            if (disqualifiedRef.current) {
                return;
            }


            disqualifyingRef.current = true;


            try {

                setSecurityMessage(
                    "Rule violation detected. You are being disqualified..."
                );


                const response = await api.post(
                    "/CompetitionSecurity/disqualify",
                    {
                        violationType,
                        violationMessage,
                        cameraStatus,
                        personCount
                    }
                );


                // ------------------------------------------------
                // Mark locally as disqualified
                // ------------------------------------------------

                disqualifiedRef.current = true;

                setDisqualified(true);

                setCameraState(
                    "disqualified"
                );

                setViolationCountdown(null);


                // ------------------------------------------------
                // Stop camera
                // ------------------------------------------------

                if (streamRef.current) {

                    streamRef.current
                        .getTracks()
                        .forEach(track => {
                            try {
                                track.stop();
                            }
                            catch {
                                // Ignore cleanup error
                            }
                        });

                    streamRef.current = null;

                }


                // ------------------------------------------------
                // Stop detection
                // ------------------------------------------------

                if (animationRef.current) {

                    cancelAnimationFrame(
                        animationRef.current
                    );

                    animationRef.current = null;

                }


                // ------------------------------------------------
                // Check API response
                // ------------------------------------------------

                if (
                    response?.data?.status ===
                    "DISQUALIFIED"
                ) {

                    setSecurityMessage(
                                "Your access to this competition has been suspended due to a security rule violation."

                    );

                }
                else {

                    setSecurityMessage(
        "Your access to this competition has been suspended due to a security rule violation."
                    );

                }

            }
            catch (error) {

                console.error(
                    "Disqualification API failed:",
                    error
                );


                // ------------------------------------------------
                // Even if network fails, don't repeatedly
                // hammer the server.
                // ------------------------------------------------

                setSecurityMessage(
                    "Security violation detected. Please contact the competition administrator."
                );

            }

        },
        []
    );


    // ========================================================
    // HANDLE NO FACE
    // ========================================================

    const handleNoFace = useCallback(
        () => {

            const now =
                Date.now();


            // ------------------------------------------------
            // Start timer
            // ------------------------------------------------

            if (!noFaceSinceRef.current) {

                noFaceSinceRef.current =
                    now;

            }


            // Multiple-person timer reset
            multipleFaceSinceRef.current =
                null;


            const elapsed =
                now -
                noFaceSinceRef.current;


            const remaining =
                Math.max(
                    0,
                    Math.ceil(
                        (
                            NO_PERSON_LIMIT_MS -
                            elapsed
                        ) / 1000
                    )
                );


            setViolationCountdown(
                remaining
            );


            // ------------------------------------------------
            // Before disqualification
            // ------------------------------------------------

            if (
                elapsed <
                NO_PERSON_LIMIT_MS
            ) {

                setCameraState(
                    "no-face"
                );

                setSecurityMessage(
                    `No person detected. Please return to your seat. You will be disqualified in ${remaining} seconds.`
                );

                return;

            }


            // ------------------------------------------------
            // Time exceeded
            // ------------------------------------------------

            if (
                !disqualifyingRef.current &&
                !disqualifiedRef.current
            ) {

                disqualifyUser({
                    violationType:
                        "NO_PERSON_DETECTED",

                    violationMessage:
                        "No person was detected in front of the camera for 10 seconds.",

                    cameraStatus:
                        "NO_PERSON",

                    personCount:
                        0
                });

            }

        },
        [disqualifyUser]
    );


    // ========================================================
    // HANDLE MULTIPLE PEOPLE
    // ========================================================

    const handleMultipleFaces = useCallback(
        (faceCount) => {

            const now =
                Date.now();


            // ------------------------------------------------
            // Start timer
            // ------------------------------------------------

            if (!multipleFaceSinceRef.current) {

                multipleFaceSinceRef.current =
                    now;

            }


            // No-face timer reset
            noFaceSinceRef.current =
                null;


            const elapsed =
                now -
                multipleFaceSinceRef.current;


            const remaining =
                Math.max(
                    0,
                    Math.ceil(
                        (
                            MULTIPLE_PERSON_LIMIT_MS -
                            elapsed
                        ) / 1000
                    )
                );


            setViolationCountdown(
                remaining
            );


            // ------------------------------------------------
            // Before disqualification
            // ------------------------------------------------

            if (
                elapsed <
                MULTIPLE_PERSON_LIMIT_MS
            ) {

                setCameraState(
                    "multiple-faces"
                );

                setSecurityMessage(
                    `Break Rule — Multiple persons detected. Only you are allowed. You will be disqualified in ${remaining} seconds.`
                );

                return;

            }


            // ------------------------------------------------
            // Time exceeded
            // ------------------------------------------------

            if (
                !disqualifyingRef.current &&
                !disqualifiedRef.current
            ) {

                disqualifyUser({
                    violationType:
                        "MULTIPLE_PERSONS",

                    violationMessage:
                        "Multiple persons were detected in front of the camera for 10 seconds.",

                    cameraStatus:
                        "MULTIPLE_PERSONS",

                    personCount:
                        faceCount
                });

            }

        },
        [disqualifyUser]
    );


    // ========================================================
    // HANDLE EXACTLY ONE PERSON
    // ========================================================

    const handleSingleFace = useCallback(
        () => {

            // ------------------------------------------------
            // Reset violation timers
            // ------------------------------------------------

            noFaceSinceRef.current =
                null;

            multipleFaceSinceRef.current =
                null;


            setViolationCountdown(
                null
            );


            if (
                disqualifyingRef.current ||
                disqualifiedRef.current
            ) {
                return;
            }


            setCameraState(
                "verified"
            );

            setSecurityMessage(
                ""
            );

        },
        []
    );


    // ========================================================
    // FACE DETECTION
    // ========================================================

    const startFaceDetection =
        useCallback(
            () => {

                const detect =
                    () => {

                        if (
                            !mountedRef.current ||
                            disqualifiedRef.current
                        ) {
                            return;
                        }


                        const video =
                            videoRef.current;

                        const detector =
                            detectorRef.current;


                        if (
                            !video ||
                            !detector
                        ) {

                            animationRef.current =
                                requestAnimationFrame(
                                    detect
                                );

                            return;

                        }


                        const now =
                            performance.now();


                        // ------------------------------------------------
                        // Detection throttling
                        // ------------------------------------------------

                        if (
                            now -
                            lastDetectionRef.current >=
                            DETECTION_INTERVAL_MS
                        ) {

                            lastDetectionRef.current =
                                now;


                            try {

                                if (
                                    video.readyState >=
                                    HTMLMediaElement.HAVE_CURRENT_DATA
                                ) {

                                    const result =
                                        detector.detectForVideo(
                                            video,
                                            now
                                        );


                                    const detections =
                                        result?.detections || [];


                                    const faceCount =
                                        detections.length;


                                    // ====================================
                                    // ZERO PERSON
                                    // ====================================

                                    if (
                                        faceCount === 0
                                    ) {

                                        handleNoFace();

                                    }


                                    // ====================================
                                    // MULTIPLE PERSON
                                    // ====================================

                                    else if (
                                        faceCount > 1
                                    ) {

                                        handleMultipleFaces(
                                            faceCount
                                        );

                                    }


                                    // ====================================
                                    // EXACTLY ONE PERSON
                                    // ====================================

                                    else {

                                        handleSingleFace();

                                    }


                                    // ------------------------------------------------
                                    // Server status
                                    //
                                    // Only state changes reach API.
                                    // ------------------------------------------------

                                    let serverCameraStatus;

                                    if (
                                        faceCount === 0
                                    ) {
                                        serverCameraStatus =
                                            "NO_PERSON";
                                    }
                                    else if (
                                        faceCount > 1
                                    ) {
                                        serverCameraStatus =
                                            "MULTIPLE_PERSONS";
                                    }
                                    else {
                                        serverCameraStatus =
                                            "VERIFIED";
                                    }


                                    updateCameraStatus(
                                        serverCameraStatus,
                                        faceCount
                                    );

                                }

                            }
                            catch (error) {

                                console.error(
                                    "Face detection error:",
                                    error
                                );

                            }

                        }


                        animationRef.current =
                            requestAnimationFrame(
                                detect
                            );

                    };


                detect();

            },
            [
                handleNoFace,
                handleMultipleFaces,
                handleSingleFace,
                updateCameraStatus
            ]
        );


    // ========================================================
    // CAMERA INITIALIZATION
    // ========================================================

    useEffect(
        () => {

            mountedRef.current =
                true;


            let localStream =
                null;


            const initializeSecurity =
                async () => {

                    try {

                        // ============================================
                        // INITIAL STATE
                        // ============================================

                        setCameraState(
                            "initializing"
                        );

                        setSecurityMessage(
                            "Requesting camera permission..."
                        );


                        // ============================================
                        // CAMERA SUPPORT
                        // ============================================

                        if (
                            !navigator.mediaDevices ||
                            !navigator.mediaDevices.getUserMedia
                        ) {

                            throw new Error(
                                "Camera API is not supported by this browser."
                            );

                        }


                        // ============================================
                        // CAMERA PERMISSION
                        // ============================================

                        localStream =
                            await navigator.mediaDevices.getUserMedia({

                                video: {

                                    facingMode:
                                        "user",

                                    width: {
                                        ideal: 640
                                    },

                                    height: {
                                        ideal: 480
                                    },

                                    frameRate: {
                                        ideal: 15,
                                        max: 20
                                    }

                                },

                                audio: false

                            });


                        if (
                            !mountedRef.current
                        ) {

                            localStream
                                .getTracks()
                                .forEach(
                                    track =>
                                        track.stop()
                                );

                            return;

                        }


                        streamRef.current =
                            localStream;


                        // ============================================
                        // CAMERA TRACK MONITOR
                        // ============================================

                        const videoTrack =
                            localStream.getVideoTracks()[0];


                        if (videoTrack) {

                            videoTrack.addEventListener(
                                "ended",
                                () => {

                                    if (
                                        !mountedRef.current ||
                                        disqualifiedRef.current
                                    ) {
                                        return;
                                    }


                                    setCameraState(
                                        "camera-stopped"
                                    );

                                    setSecurityMessage(
                                        "Camera was stopped. Please enable the camera to continue."
                                    );

                                }
                            );

                        }


                        // ============================================
                        // ATTACH VIDEO
                        // ============================================

                        if (
                            videoRef.current
                        ) {

                            const video =
                                videoRef.current;


                            video.srcObject =
                                localStream;


                            await new Promise(
                                resolve => {

                                    if (
                                        video.readyState >= 2
                                    ) {

                                        resolve();

                                        return;

                                    }


                                    video.onloadedmetadata =
                                        () => {
                                            resolve();
                                        };

                                }
                            );


                            if (
                                !mountedRef.current
                            ) {
                                return;
                            }


                            try {

                                await video.play();

                            }
                            catch (error) {

                                if (
                                    error?.name !==
                                    "AbortError"
                                ) {

                                    throw error;

                                }

                                console.warn(
                                    "Video play interrupted during initialization."
                                );

                            }

                        }


                        // ============================================
                        // MEDIAPIPE
                        // ============================================

                        setCameraState(
                            "checking"
                        );

                        setSecurityMessage(
                            "Loading camera security..."
                        );


                        const vision =
                            await FilesetResolver.forVisionTasks(
                                WASM_PATH
                            );


                        // ============================================
                        // FACE DETECTOR
                        // ============================================

                        detectorRef.current =
                            await FaceDetector.createFromOptions(
                                vision,
                                {

                                    baseOptions: {

                                        modelAssetPath:
                                            MODEL_PATH

                                    },

                                    runningMode:
                                        "VIDEO",

                                    minDetectionConfidence:
                                        0.5,

                                    minSuppressionThreshold:
                                        0.3

                                }
                            );


                        if (
                            !mountedRef.current
                        ) {
                            return;
                        }


                        // ============================================
                        // START DETECTION
                        // ============================================

                        setCameraState(
                            "checking"
                        );

                        setSecurityMessage(
                            "Checking your face..."
                        );


                        startFaceDetection();

                    }
                    catch (error) {

                        console.error(
                            "Camera security initialization failed:",
                            error
                        );


                        if (
                            !mountedRef.current
                        ) {
                            return;
                        }


                        // ============================================
                        // PERMISSION DENIED
                        // ============================================

                        if (
                            error?.name ===
                            "NotAllowedError"
                        ) {

                            setCameraState(
                                "denied"
                            );

                            setSecurityMessage(
                                "Camera permission is required to continue."
                            );

                        }


                        // ============================================
                        // CAMERA NOT FOUND
                        // ============================================

                        else if (
                            error?.name ===
                            "NotFoundError"
                        ) {

                            setCameraState(
                                "camera-unavailable"
                            );

                            setSecurityMessage(
                                "No camera was found on this device."
                            );

                        }


                        // ============================================
                        // OTHER ERROR
                        // ============================================

                        else {

                            setCameraState(
                                "error"
                            );

                            setSecurityMessage(
                                "Camera security could not be initialized."
                            );

                        }

                    }

                };


            initializeSecurity();


            // ========================================================
            // CLEANUP
            // ========================================================

            return () => {

                mountedRef.current =
                    false;


                // --------------------------------------------
                // Stop animation
                // --------------------------------------------

                if (
                    animationRef.current
                ) {

                    cancelAnimationFrame(
                        animationRef.current
                    );

                    animationRef.current =
                        null;

                }


                // --------------------------------------------
                // Close detector
                // --------------------------------------------

                if (
                    detectorRef.current
                ) {

                    try {

                        detectorRef.current.close();

                    }
                    catch {
                        // Ignore cleanup error
                    }

                    detectorRef.current =
                        null;

                }


                // --------------------------------------------
                // Stop local stream
                // --------------------------------------------

                if (
                    localStream
                ) {

                    localStream
                        .getTracks()
                        .forEach(
                            track => {

                                try {
                                    track.stop();
                                }
                                catch {
                                    // Ignore
                                }

                            }
                        );

                }


                // --------------------------------------------
                // Stop stored stream
                // --------------------------------------------

                if (
                    streamRef.current
                ) {

                    streamRef.current
                        .getTracks()
                        .forEach(
                            track => {

                                try {
                                    track.stop();
                                }
                                catch {
                                    // Ignore
                                }

                            }
                        );

                    streamRef.current =
                        null;

                }

            };

        },
        [startFaceDetection]
    );

const handleSuspendedScreenClick = () => {

    if (!disqualifiedRef.current) {
        return;
    }

    // Normal competition-user authentication clear karo
    localStorage.removeItem("jwt_token");
    localStorage.removeItem("user_profile");

    sessionStorage.removeItem("jwt_token");
    sessionStorage.removeItem("user_profile");

    // Login page par redirect
    window.location.replace("/");
};
    // ========================================================
    // RETRY CAMERA
    // ========================================================

    const retryCamera =
        () => {

            window.location.reload();

        };


    // ========================================================
    // SECURITY BLOCK
    // ========================================================

    const securityBlocked =
        cameraState !== "verified" ||
        disqualified;


    // ========================================================
    // UI
    // ========================================================

    return (
        <>

            {/* ==================================================
                HIDDEN CAMERA
            ================================================== */}

            <video
                ref={videoRef}
                autoPlay
                muted
                playsInline
                aria-hidden="true"
                style={{
                    position: "fixed",
                    width: "1px",
                    height: "1px",
                    opacity: 0,
                    pointerEvents: "none",
                    zIndex: -1
                }}
            />


            {/* ==================================================
                SECURITY BLOCKING SCREEN
            ================================================== */}

            {securityBlocked && (

    <div
    onClick={
        disqualified
            ? handleSuspendedScreenClick
            : undefined
    }
    className="
        position-fixed
        top-0
        start-0
        w-100
        h-100
        d-flex
        align-items-center
        justify-content-center
    "
    style={{
        background: "rgba(0, 0, 0, 0.78)",
        zIndex: 999999,
        cursor: disqualified
            ? "pointer"
            : "default"
    }}
>

                    <div
                        className="
                            bg-white
                            rounded-4
                            shadow-lg
                            p-4
                            text-center
                        "
                        style={{
                            width: "430px",
                            maxWidth: "92%"
                        }}
                    >

                        {/* ----------------------------------------
                            ICON
                        ----------------------------------------- */}

                        <div className="mb-3">

                            <i
                                className={
                                    disqualified
                                        ? "bi bi-shield-x text-danger"
                                        : cameraState === "verified"
                                            ? "bi bi-camera-video-fill text-success"
                                            : "bi bi-camera-video-off-fill text-danger"
                                }
                                style={{
                                    fontSize: "3rem"
                                }}
                            />

                        </div>


                        {/* ----------------------------------------
                            TITLE
                        ----------------------------------------- */}

                        <h5 className="fw-bold mb-2">

    {disqualified
        ? "Competition Access Suspended"
        : "Camera Security Required"}

</h5>


                        {/* ----------------------------------------
                            DESCRIPTION
                        ----------------------------------------- */}

                        <p className="text-muted small mb-3">

    {disqualified
        ? "Your access to this competition has been suspended due to a security rule violation."
        : "Camera access must remain enabled while you are logged in to the competition portal."}

</p>


                        {/* ----------------------------------------
                            COUNTDOWN
                        ----------------------------------------- */}

                        {!disqualified &&
                            violationCountdown !== null &&
                            violationCountdown > 0 && (

                                <div
                                    className="
                                        alert
                                        alert-danger
                                        fw-bold
                                        rounded-4
                                    "
                                >

                                    <div
                                        className="fs-2"
                                    >
                                        {violationCountdown}
                                    </div>

                                    <div className="small">

                                        Security action will be
                                        taken automatically.

                                    </div>

                                </div>

                            )}


                        {/* ----------------------------------------
                            MESSAGE
                        ----------------------------------------- */}

                        <div
    className={
        disqualified
            ? "alert alert-danger small"
            : "alert alert-warning small"
    }
>
    {securityMessage ||
        "Please allow camera access and keep only yourself in the camera frame."}

    {disqualified && (
        <div className="mt-2 fw-semibold">
            Click anywhere to return to the login page.
        </div>
    )}
</div>


                        {/* ----------------------------------------
                            RETRY CAMERA
                        ----------------------------------------- */}

                        {!disqualified &&
                            (
                                cameraState === "denied" ||
                                cameraState === "camera-stopped" ||
                                cameraState === "camera-unavailable" ||
                                cameraState === "error"
                            ) && (

                                <button
                                    type="button"
                                    className="
                                        btn
                                        btn-primary
                                        rounded-pill
                                        px-4
                                        fw-semibold
                                    "
                                    onClick={retryCamera}
                                >

                                    <i
                                        className="
                                            bi
                                            bi-camera
                                            me-2
                                        "
                                    />

                                    Enable Camera

                                </button>

                            )}

                    </div>

                </div>

            )}


            {/* ==================================================
                ACTUAL APPLICATION
            ================================================== */}

            {!securityBlocked &&
                children}

        </>
    );
}