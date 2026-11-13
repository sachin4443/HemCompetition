export default function LoginErrorModal({
    message,
    onClose
}) {
    if (!message) {
        return null;
    }

    return (
        <div
            className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
            style={{
                background: 'rgba(0, 0, 0, 0.70)',
                zIndex: 999999,
                padding: '20px'
            }}
        >
            <div
                className="bg-white rounded-4 shadow-lg text-center p-4 p-sm-5"
                style={{
                    width: '440px',
                    maxWidth: '100%',
                    animation: 'fadeInScale 0.25s ease'
                }}
            >

                {/* ICON */}
                <div
                    className="mx-auto mb-3 d-flex align-items-center justify-content-center rounded-circle bg-danger-subtle"
                    style={{
                        width: '75px',
                        height: '75px'
                    }}
                >
                    <i
                        className="bi bi-exclamation-circle text-danger"
                        style={{
                            fontSize: '2.5rem'
                        }}
                    ></i>
                </div>

                {/* TITLE */}
                <h4 className="fw-bold text-danger mb-2">
                    Login Failed
                </h4>

                {/* MESSAGE */}
                <p className="text-secondary small mb-4 px-2">
                    {message}
                </p>

                {/* CLOSE */}
                <button
                    type="button"
                    className="btn btn-dark rounded-pill px-5 py-2 fw-semibold"
                    onClick={onClose}
                >
                    Close
                </button>

            </div>

            {/* ANIMATION */}
            <style>
                {`
                    @keyframes fadeInScale {
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