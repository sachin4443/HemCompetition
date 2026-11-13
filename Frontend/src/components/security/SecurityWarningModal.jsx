export default function SecurityWarningModal({
    open,
    title = "Security Warning",
    message,
    warnings = 0,
    maxWarnings = 10,
    fullscreenLost = false,
    onEnterFullscreen
}) {

    if (!open) {
        return null;
    }


    return (

        <div
            className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
            style={{
                zIndex: 999999,
                background: "rgba(0, 0, 0, 0.72)",
                backdropFilter: fullscreenLost
                    ? "blur(14px)"
                    : "none",
                WebkitBackdropFilter: fullscreenLost
                    ? "blur(14px)"
                    : "none",
                padding: "20px"
            }}
        >

            <div
                className="bg-white rounded-4 shadow-lg text-center p-4 p-sm-5"
                style={{
                    width: "440px",
                    maxWidth: "100%",
                    animation: "securityModalIn 0.2s ease"
                }}
            >

                {/* =================================================
                    ICON
                ================================================= */}

                <div
                    className="mx-auto mb-3 d-flex align-items-center justify-content-center rounded-circle bg-danger-subtle"
                    style={{
                        width: "78px",
                        height: "78px"
                    }}
                >

                    <i
                        className="bi bi-shield-exclamation text-danger"
                        style={{
                            fontSize: "2.6rem"
                        }}
                    />

                </div>


                {/* =================================================
                    TITLE
                ================================================= */}

                <h4 className="fw-bold text-danger mb-2">

                    {title}

                </h4>


                {/* =================================================
                    MESSAGE
                ================================================= */}

                <p className="text-dark mb-3">

                    {message}

                </p>


                {/* =================================================
                    WARNING COUNT
                ================================================= */}

                <div
                    className="rounded-4 p-3 mb-3"
                    style={{
                        background: "#fff5f5",
                        border: "1px solid #f5c2c7"
                    }}
                >

                    <div className="small text-muted mb-1">

                        Total Security Violations

                    </div>

                    <div className="fw-bold text-danger fs-4">

                        Warning {warnings} / {maxWarnings}

                    </div>

                </div>


                {/* =================================================
                    WARNING MESSAGE
                ================================================= */}

                {warnings < maxWarnings && (

                    <div className="alert alert-warning border-0 rounded-4 small mb-3">

                        <i className="bi bi-exclamation-triangle-fill me-2" />

                        Please follow all competition security rules.

                    </div>

                )}


                {/* =================================================
                    10 / 10
                ================================================= */}

                {warnings >= maxWarnings && (

                    <div className="alert alert-danger border-0 rounded-4 small mb-3">

                        <i className="bi bi-x-circle-fill me-2" />

                        Maximum security warnings reached.
                        Your competition is being terminated.

                    </div>

                )}


                {/* =================================================
                    FULLSCREEN LOST
                ================================================= */}

                {fullscreenLost && (

                    <div className="alert alert-warning border-0 rounded-4 small mb-3">

                        <i className="bi bi-fullscreen me-2" />

                        You exited fullscreen mode.
                        Fullscreen is mandatory to continue.

                    </div>

                )}


                {/* =================================================
                    RETURN FULLSCREEN
                ================================================= */}

                {fullscreenLost &&
                    onEnterFullscreen &&
                    warnings < maxWarnings && (

                        <button
                            type="button"
                            className="btn btn-dark rounded-pill px-4 py-2 fw-semibold"
                            onClick={onEnterFullscreen}
                        >

                            <i className="bi bi-fullscreen me-2" />

                            Return to Fullscreen

                        </button>

                    )}


                {/* =================================================
                    10 / 10
                ================================================= */}

                {warnings >= maxWarnings && (

                    <div className="text-muted small mt-2">

                        Redirecting to login...

                    </div>

                )}

            </div>


            {/* =================================================
                ANIMATION
            ================================================= */}

            <style>
                {`

                    @keyframes securityModalIn {

                        from {
                            opacity: 0;
                            transform: scale(0.92);
                        }

                        to {
                            opacity: 1;
                            transform: scale(1);
                        }

                    }

                `}
            </style>

        </div>

    );

}