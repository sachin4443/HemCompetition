export default function DisqualifiedModal({
    data,
    onClose
}) {
    if (!data) {
        return null;
    }

    return (
        <div
            className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
            style={{
                background: 'rgba(0, 0, 0, 0.75)',
                zIndex: 999999,
                padding: '20px'
            }}
        >
            <div
                className="bg-white rounded-4 shadow-lg text-center p-4 p-sm-5"
                style={{
                    width: '460px',
                    maxWidth: '100%',
                    animation:
                        'fadeInScale 0.25s ease'
                }}
            >

                {/* =================================================
                    ICON
                ================================================= */}

                <div
                    className="mx-auto mb-3 d-flex align-items-center justify-content-center rounded-circle bg-danger-subtle"
                    style={{
                        width: '80px',
                        height: '80px'
                    }}
                >
                    <i
                        className="bi bi-shield-x text-danger"
                        style={{
                            fontSize: '2.8rem'
                        }}
                    ></i>
                </div>

                {/* =================================================
                    TITLE
                ================================================= */}

                <h4 className="fw-bold text-danger mb-2">
                    You are blocked due to a violation of the rules.
                </h4>

                {/* =================================================
                    MAIN MESSAGE
                ================================================= */}

               {/* English Version (Single Line) */}


{/* Shuddh Hindi Variant (Agar pure Hindi script mein chahiye) */}
<p className="text-secondary small mb-4 px-2">
    प्रतियोगिता के दौरान सुरक्षा नियमों के उल्लंघन के कारण आप इसमें पुनः भाग लेने के पात्र नहीं हैं।
</p>
                {/* =================================================
                    REASON
                ================================================= */}

                {(
                    data.violationType ||
                    data.violationMessage
                ) && (

                    <div
                        className="text-start rounded-4 p-3 mb-4"
                        style={{
                            background:
                                '#fff5f5',
                            border:
                                '1px solid #f5c2c7'
                        }}
                    >

                        <div className="small text-muted fw-semibold mb-1">
                            Disqualification Reason
                        </div>

                        {data.violationType && (

                            <div className="fw-bold text-danger mb-1">
                                {formatViolationType(
                                    data.violationType
                                )}
                            </div>

                        )}

                        {data.violationMessage && (

                            <div className="small text-dark">
                                {
                                    data.violationMessage
                                }
                            </div>

                        )}

                    </div>
                )}

                {/* =================================================
                    WARNING
                ================================================= */}

                <div className="alert alert-danger border-0 rounded-4 small mb-4">

                    <i className="bi bi-exclamation-triangle-fill me-2"></i>

                    Your competition access has been permanently restricted.

                </div>

                {/* =================================================
                    CLOSE
                ================================================= */}

                <button
                    type="button"
                    className="btn btn-dark rounded-pill px-5 py-2 fw-semibold"
                    onClick={onClose}
                >
                    Close
                </button>

            </div>

            {/* =================================================
                ANIMATION
            ================================================= */}

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


// =====================================================
// FORMAT VIOLATION TYPE
// =====================================================

function formatViolationType(type) {
    if (!type) {
        return '';
    }

    return String(type)
        .replaceAll('_', ' ')
        .replaceAll('-', ' ')
        .toLowerCase()
        .replace(/\b\w/g, (char) =>
            char.toUpperCase()
        );
}