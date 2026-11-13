import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const rules = [
    {
        icon: 'bi-camera-video-fill',
        title: 'Camera Based Competition / कैमरा-आधारित प्रतियोगिता',
        text: 'This competition uses camera-based monitoring to maintain a fair and secure competition environment.\nयह प्रतियोगिता निष्पक्ष और सुरक्षित माहौल बनाए रखने के लिए कैमरे की निगरानी का उपयोग करती है।'
    },
    {
        icon: 'bi-eye-fill',
        title: 'Live Monitoring / लाइव निगरानी',
        text: 'Your camera may be monitored during the competition. Please ensure that your camera remains active and your face remains visible.\nप्रतियोगिता के दौरान आपके कैमरे की निगरानी की जा सकती है। कृपया सुनिश्चित करें कि आपका कैमरा चालू रहे और आपका चेहरा दिखाई देता रहे।'
    },
    {
        icon: 'bi-person-fill',
        title: 'Only One Person / केवल एक व्यक्ति',
        text: 'Only you should be present in front of the camera during the competition. No other person should appear in the camera view.\nप्रतियोगिता के दौरान कैमरे के सामने केवल आप ही मौजूद होने चाहिए। कैमरे के फ्रेम में कोई अन्य व्यक्ति नहीं दिखना चाहिए।'
    },
    {
        icon: 'bi-person-walking',
        title: 'Remain Seated / अपनी सीट पर बैठे रहें',
        text: 'Once the competition starts, remain seated at your place. Leaving your seat may be treated as a competition rule violation.\nएक बार प्रतियोगिता शुरू होने के बाद, अपनी जगह पर बैठे रहें। अपनी सीट छोड़ने को नियम का उल्लंघन माना जा सकता है।'
    },
    {
        icon: 'bi-shield-exclamation',
        title: 'Automatic Rule Enforcement / स्वचालित नियम प्रवर्तन',
        text: 'If another person is detected, or you remain outside the camera view for the allowed time, the system may automatically disqualify you from the competition.\nयदि कोई अन्य व्यक्ति दिखाई देता है, या आप निर्धारित समय के लिए कैमरे की नजर से बाहर रहते हैं, तो सिस्टम आपको स्वचालित रूप से अयोग्य (disqualify) कर सकता है।'
    }
];

export default function CompetitionSecurityPage() {

    const navigate = useNavigate();

    const [currentStep, setCurrentStep] = useState(0);

    const [accepted, setAccepted] = useState(false);

    const [loading, setLoading] = useState(true);

    const [submitting, setSubmitting] = useState(false);

    const [error, setError] = useState('');

    const isLastStep = currentStep === rules.length;

    // =====================================================
    // CHECK SECURITY STATUS
    // =====================================================

    useEffect(() => {

        const checkSecurity = async () => {

            try {

                const response =
                    await api.get('/CompetitionSecurity/status');

                const data = response.data;

                // Already accepted
                if (
                    data.termsAccepted &&
                    data.status === 'ACTIVE'
                ) {
                    navigate('/dashboard', {
                        replace: true
                    });

                    return;
                }

                // Already disqualified
                if (data.status === 'DISQUALIFIED') {

                    setError(
                        'You are disqualified from this competition.'
                    );
                }

            }
            catch (err) {

                console.error(
                    'Competition security status error:',
                    err
                );

                setError(
                    'Unable to load competition security status.'
                );

            }
            finally {

                setLoading(false);

            }
        };

        checkSecurity();

    }, [navigate]);


    // =====================================================
    // NEXT
    // =====================================================

    const handleNext = () => {

        if (currentStep < rules.length) {

            setCurrentStep(
                currentStep + 1
            );

        }

    };


    // =====================================================
    // PREVIOUS
    // =====================================================

    const handlePrevious = () => {

        if (currentStep > 0) {

            setCurrentStep(
                currentStep - 1
            );

        }

    };


    // =====================================================
    // ACCEPT
    // =====================================================

    const handleAccept = async () => {

        if (!accepted) {
            return;
        }

        try {

            setSubmitting(true);

            setError('');

            const response =
                await api.post(
                    '/CompetitionSecurity/accept'
                );

            if (response.data?.success) {

                navigate('/dashboard', {
                    replace: true
                });

            }

        }
        catch (err) {

            console.error(
                'Terms acceptance error:',
                err
            );

            setError(
                err.response?.data?.message ||
                'Unable to save your acceptance. Please try again.'
            );

        }
        finally {

            setSubmitting(false);

        }
    };


    // =====================================================
    // LOADING
    // =====================================================

    if (loading) {

        return (
            <div className="min-vh-100 d-flex align-items-center justify-content-center bg-light">

                <div className="text-center">

                    <div
                        className="spinner-border text-success mb-3"
                        role="status"
                    />

                    <div className="text-muted">
                        Preparing competition security...
                    </div>

                </div>

            </div>
        );
    }


    // =====================================================
    // UI
    // =====================================================

    return (

        <div
            className="min-vh-100 d-flex align-items-center justify-content-center bg-light p-3"
        >

            <div
                className="card border-0 shadow-lg rounded-4 w-100"
                style={{
                    maxWidth: '650px'
                }}
            >

                <div className="card-body p-4 p-md-5">

                    {/* HEADER */}

                    <div className="text-center mb-4">

                        <img
                            src="/logo.png"
                            alt="HEM Competition"
                            height="55"
                            className="mb-3"
                        />

                        <h3 className="fw-bold mb-1">
                            Competition Security / प्रतियोगिता सुरक्षा
                        </h3>

                        <p className="text-muted small mb-0">
                            Please read each rule carefully before continuing. <br />
                            कृपया आगे बढ़ने से पहले प्रत्येक नियम को ध्यान से पढ़ें।
                        </p>

                    </div>


                    {/* PROGRESS */}

                    <div className="mb-4">

                        <div className="d-flex justify-content-between small text-muted mb-2">

                            <span>
                                Rule {currentStep + 1} of {rules.length + 1}
                            </span>

                            <span>
                                {Math.round(
                                    ((currentStep + 1) /
                                        (rules.length + 1)) * 100
                                )}%
                            </span>

                        </div>

                        <div
                            className="progress"
                            style={{
                                height: '7px'
                            }}
                        >

                            <div
                                className="progress-bar bg-success"
                                style={{
                                    width:
                                        `${((currentStep + 1) /
                                            (rules.length + 1)) * 100}%`
                                }}
                            />

                        </div>

                    </div>


                    {/* ERROR */}

                    {error && (

                        <div className="alert alert-danger rounded-4 border-0">

                            <i className="bi bi-exclamation-triangle-fill me-2"></i>

                            {error}

                        </div>

                    )}


                    {/* RULE */}

                    {!isLastStep ? (

                        <div
                            className="text-center py-4"
                            style={{
                                minHeight: '300px'
                            }}
                        >

                            <div
                                className="bg-success-subtle rounded-circle d-flex align-items-center justify-content-center mx-auto mb-4"
                                style={{
                                    width: 90,
                                    height: 90
                                }}
                            >

                                <i
                                    className={`bi ${rules[currentStep].icon} text-success`}
                                    style={{
                                        fontSize: '2.3rem'
                                    }}
                                />

                            </div>


                            <h4 className="fw-bold mb-3">

                                {rules[currentStep].title}

                            </h4>


                            <p
                                className="text-muted"
                                style={{
                                    fontSize: '1.05rem',
                                    lineHeight: '1.8',
                                    whiteSpace: 'pre-line'
                                }}
                            >

                                {rules[currentStep].text}

                            </p>

                        </div>

                    ) : (

                        /* FINAL ACCEPTANCE */

                        <div
                            className="py-4"
                            style={{
                                minHeight: '300px'
                            }}
                        >

                            <div className="text-center mb-4">

                                <div
                                    className="bg-success-subtle rounded-circle d-flex align-items-center justify-content-center mx-auto mb-3"
                                    style={{
                                        width: 80,
                                        height: 80
                                    }}
                                >

                                    <i
                                        className="bi bi-check2-circle text-success"
                                        style={{
                                            fontSize: '2.3rem'
                                        }}
                                    />

                                </div>

                                <h4 className="fw-bold">
                                    Final Confirmation / अंतिम पुष्टि
                                </h4>

                            </div>


                            <div
                                className="alert alert-warning border-0 rounded-4"
                            >

                                <i className="bi bi-exclamation-triangle-fill me-2"></i>

                                Please make sure you have read and understood all the competition rules. <br />
                                कृपया सुनिश्चित करें कि आपने प्रतियोगिता के सभी नियमों को पढ़ और समझ लिया है।

                            </div>


                            <label
                                className="d-flex align-items-start gap-3 p-3 border rounded-4"
                                style={{
                                    cursor: 'pointer'
                                }}
                            >

                                <input
                                    type="checkbox"
                                    className="form-check-input mt-1"
                                    checked={accepted}
                                    onChange={(e) =>
                                        setAccepted(e.target.checked)
                                    }
                                    style={{
                                        width: 20,
                                        height: 20
                                    }}
                                />

                                <span className="small">

                                    I have read and understood all the competition rules and agree to the camera-based monitoring and security conditions.
                                    <br />
                                    मैंने प्रतियोगिता के सभी नियमों को पढ़ और समझ लिया है और कैमरा-आधारित निगरानी एवं सुरक्षा शर्तों से सहमत हूँ।

                                </span>

                            </label>

                        </div>

                    )}


                    {/* BUTTONS */}

                    <div className="d-flex justify-content-between gap-2 mt-3">

                        <button
                            type="button"
                            className="btn btn-light border rounded-pill px-4"
                            onClick={handlePrevious}
                            disabled={currentStep === 0}
                        >
                            <i className="bi bi-arrow-left me-2"></i>
                            Previous / पिछला
                        </button>


                        {!isLastStep ? (

                            <button
                                type="button"
                                className="btn btn-success rounded-pill px-4 fw-semibold"
                                onClick={handleNext}
                            >
                                Next / अगला
                                <i className="bi bi-arrow-right ms-2"></i>
                            </button>

                        ) : (

                            <button
                                type="button"
                                className="btn btn-success rounded-pill px-4 fw-semibold"
                                disabled={
                                    !accepted ||
                                    submitting ||
                                    error
                                }
                                onClick={handleAccept}
                            >

                                {submitting ? (

                                    <>
                                        <span
                                            className="spinner-border spinner-border-sm me-2"
                                        />
                                        Saving...
                                    </>

                                ) : (

                                    <>
                                        <i className="bi bi-shield-check me-2"></i>
                                        Accept & Continue / स्वीकार करें और आगे बढ़ें
                                    </>

                                )}

                            </button>

                        )}

                    </div>

                </div>

            </div>

        </div>
    );
}