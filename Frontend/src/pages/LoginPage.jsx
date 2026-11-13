import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import DisqualifiedModal from '../components/auth/DisqualifiedModal';
import LoginErrorModal from '../components/auth/LoginErrorModal';
export default function LoginPage() {
    const navigate = useNavigate();
    const [loginError, setLoginError] = useState('');
    const [usernameOrEmail, setUsernameOrEmail] = useState('');
    const [password, setPassword] = useState('');

    const [showPuzzle, setShowPuzzle] = useState(false);
    const [num1, setNum1] = useState(0);
    const [num2, setNum2] = useState(0);
    const [userAnswer, setUserAnswer] = useState('');
    const [puzzleSolved, setPuzzleSolved] = useState(false);
    const [puzzleError, setPuzzleError] = useState('');

    const [competition, setCompetition] = useState(null);
    const [competitionLoading, setCompetitionLoading] = useState(true);
    const [competitionStatus, setCompetitionStatus] = useState('');
    const [countdown, setCountdown] = useState('');

    const [conflictData, setConflictData] = useState(null);
    const [disqualifiedData, setDisqualifiedData] = useState(null);
    const [loginLoading, setLoginLoading] = useState(false);

    const updateCompetitionStatus = (data) => {
        if (!data) {
            return;
        }

        const start = new Date(
            data.startDateTimeUtc
        ).getTime();

        const end = new Date(
            data.endDateTimeUtc
        ).getTime();

        const serverTime = data.serverTimeUtc
            ? new Date(data.serverTimeUtc).getTime()
            : Date.now();

        if (serverTime < start) {
            setCompetitionStatus('NOT_STARTED');
        }
        else if (
            serverTime >= start &&
            serverTime <= end
        ) {
            setCompetitionStatus('STARTED');
        }
        else {
            setCompetitionStatus('ENDED');
        }
    };

    const formatCountdown = (milliseconds) => {
        if (milliseconds <= 0) {
            return '00:00:00';
        }

        const totalSeconds = Math.floor(
            milliseconds / 1000
        );

        const days = Math.floor(
            totalSeconds / 86400
        );

        const hours = Math.floor(
            (totalSeconds % 86400) / 3600
        );

        const minutes = Math.floor(
            (totalSeconds % 3600) / 60
        );

        const seconds = totalSeconds % 60;

        const hh = String(hours).padStart(2, '0');
        const mm = String(minutes).padStart(2, '0');
        const ss = String(seconds).padStart(2, '0');

        if (days > 0) {
            return `${days}d ${hh}:${mm}:${ss}`;
        }

        return `${hh}:${mm}:${ss}`;
    };

    useEffect(() => {
        localStorage.clear();
        sessionStorage.clear();

        window.history.pushState(
            null,
            '',
            window.location.href
        );

        const handlePopState = () => {
            window.history.pushState(
                null,
                '',
                window.location.href
            );
        };

        window.addEventListener(
            'popstate',
            handlePopState
        );

        return () => {
            window.removeEventListener(
                'popstate',
                handlePopState
            );
        };
    }, []);

    useEffect(() => {
        let mounted = true;

        const loadCompetition = async () => {
            try {
                setCompetitionLoading(true);

                const response = await api.get(
                    '/CompetitionSetting/active'
                );

                if (!mounted) {
                    return;
                }

                setCompetition(response.data);

                updateCompetitionStatus(
                    response.data
                );
            }
            catch (error) {
                console.error(
                    'Competition setting error:',
                    error
                );

                if (mounted) {
                    setCompetition(null);
                    setCompetitionStatus('ERROR');
                }
            }
            finally {
                if (mounted) {
                    setCompetitionLoading(false);
                }
            }
        };

        loadCompetition();

        return () => {
            mounted = false;
        };
    }, []);

    useEffect(() => {
        if (!competition) {
            return;
        }

        const start = new Date(
            competition.startDateTimeUtc
        ).getTime();

        const end = new Date(
            competition.endDateTimeUtc
        ).getTime();

        const serverTime = competition.serverTimeUtc
            ? new Date(
                  competition.serverTimeUtc
              ).getTime()
            : Date.now();

        const serverOffset =
            serverTime - Date.now();

        const updateTimer = () => {
            const now =
                Date.now() + serverOffset;


            if (now < start) {
                const remaining =
                    start - now;

                setCompetitionStatus(
                    'NOT_STARTED'
                );

                setCountdown(
                    formatCountdown(remaining)
                );

                return;
            }
  
            if (
                now >= start &&
                now <= end
            ) {
                setCompetitionStatus(
                    'STARTED'
                );

                setCountdown('');

                return;
            }

            setCompetitionStatus('ENDED');
            setCountdown('');
        };

        updateTimer();

        const interval = setInterval(
            updateTimer,
            1000
        );

        return () => {
            clearInterval(interval);
        };
    }, [competition]);

    // const handleCredentialBlur = () => {
    //     if (
    //         usernameOrEmail.trim() !== '' &&
    //         password.trim() !== ''
    //     ) {
    //         setNum1(
    //             Math.floor(
    //                 Math.random() * 10
    //             ) + 1
    //         );

    //         setNum2(
    //             Math.floor(
    //                 Math.random() * 10
    //             ) + 1
    //         );

    //         setUserAnswer('');
    //         setPuzzleSolved(false);
    //         setPuzzleError('');
    //         setShowPuzzle(true);
    //     }
    // };

    const handleVerifyPuzzle = (e) => {
        e.preventDefault();

        const answer =
            parseInt(
                userAnswer,
                10
            );

        if (
            !Number.isNaN(answer) &&
            answer === num1 + num2
        ) {
            setPuzzleSolved(true);
            setPuzzleError('');
        }
        else {
            setPuzzleSolved(false);
            setPuzzleError(
                'Incorrect answer, try again!'
            );
        }
    };

    const handleFinalLogin = async (e) => {
        if (e) {
            e.preventDefault();
        }

      
        if (!puzzleSolved) {
            return;
        }

      

        if (loginLoading) {
            return;
        }

        setLoginLoading(true);

        try {
           

            const response = await api.post(
                '/auth/login',
                {
                    usernameOrEmail:
                        usernameOrEmail.trim(),

                    password
                }
            );

            

            if (
                response.data &&
                response.data.token
            ) {
                localStorage.setItem(
                    'jwt_token',
                    response.data.token
                );
            }

            if (response.data?.profile) {
                localStorage.setItem(
                    'user_profile',
                    JSON.stringify(
                        response.data.profile
                    )
                );
            }


            navigate(
                '/competition-security',
                {
                    replace: true
                }
            );
        }
        catch (err) {
            console.error(
                'Login error:',
                err
            );

            const responseData =
                err?.response?.data;

           

            if (
                responseData?.disqualified === true ||
                responseData?.status ===
                    'DISQUALIFIED'
            ) {
                setDisqualifiedData({
                    disqualified: true,

                    status:
                        responseData?.status ||
                        'DISQUALIFIED',

                    message:
                        responseData?.message ||
                        'You are disqualified from this competition and cannot login again.',

                    violationType:
                        responseData?.violationType ||
                        null,

                    violationMessage:
                        responseData?.violationMessage ||
                        null,

                    disqualifiedAtUtc:
                        responseData?.disqualifiedAtUtc ||
                        null
                });

            

                setLoginLoading(false);

                return;
            }

            

            if (
                responseData?.requiresSessionTermination ===
                true
            ) {
                setConflictData(
                    responseData
                );

                setLoginLoading(false);

                return;
            }

        

           const message =
    responseData?.message ||
    'Login failed. Please check your credentials.';

setLoginError(message);

setLoginLoading(false);

        }
    };

    const handleTerminateAndContinue =  async () => {

            if (!conflictData) {
                return;
            }

            try {
                setLoginLoading(true);

                await api.post(
                    '/auth/terminate-session',
                    {
                        userId:
                            conflictData.userId
                    }
                );

                setConflictData(null);

                await handleFinalLogin();
            }
            catch (error) {
                console.error(
                    'Terminate session error:',
                    error
                );

                setLoginLoading(false);

                alert(
                    error?.response?.data
                        ?.message ||
                    'Failed to terminate session. Please try again.'
                );
            }
        };

     const handleCloseDisqualifiedModal =
        () => {

            setDisqualifiedData(null);

            
        };
  
    if (competitionLoading) {
        return (
            <div
                className="min-vh-100 d-flex justify-content-center align-items-center bg-light"
            >
                <div className="text-center">
                    <div
                        className="spinner-border text-success mb-3"
                        role="status"
                    >
                        <span className="visually-hidden">
                            Loading...
                        </span>
                    </div>

                    <div className="fw-semibold text-muted">
                        Checking competition schedule...
                    </div>
                </div>
            </div>
        );
    }

    if (
        competition &&
        competitionStatus ===
            'NOT_STARTED'
    ) {
        return (
            <div
                className="min-vh-100 d-flex justify-content-center align-items-center p-3"
                style={{
                    background:
                        'linear-gradient(135deg,#f8f9fa,#e9f7ef)'
                }}
            >
                <div
                    className="card border-0 shadow-lg rounded-4 text-center"
                    style={{
                        maxWidth: '600px',
                        width: '100%'
                    }}
                >
                    <div className="card-body p-5">

                        <img
                            src="/logo.png"
                            alt="HEM Competition"
                            height="60"
                            className="mb-4"
                        />

                        <div
                            className="bg-success-subtle rounded-circle d-flex align-items-center justify-content-center mx-auto mb-4"
                            style={{
                                width: 80,
                                height: 80
                            }}
                        >
                            <i
                                className="bi bi-clock-history text-success"
                                style={{
                                    fontSize:
                                        '2rem'
                                }}
                            ></i>
                        </div>

                        <h3 className="fw-bold text-dark">
                            {
                                competition.competitionName
                            }
                        </h3>

                        <p className="text-muted mb-4">
                            Competition has not started yet.
                        </p>

                        <div className="bg-dark text-white rounded-4 p-4 mb-4">

                            <small className="text-white-50 d-block mb-2">
                                COMPETITION STARTS IN
                            </small>

                            <div
                                className="fw-bold"
                                style={{
                                    fontSize:
                                        '2.8rem',
                                    letterSpacing:
                                        '3px'
                                }}
                            >
                                {countdown}
                            </div>

                        </div>

                        <div className="alert alert-info border-0 rounded-4">

                            <i className="bi bi-info-circle-fill me-2"></i>

                            Please prepare yourself.
                            The login page will become
                            available when the competition
                            starts.

                        </div>

                        <div className="small text-muted mt-3">

                            Start:{' '}

                            <strong>
                                {new Date(
                                    competition.startDateTimeUtc
                                ).toLocaleString()}
                            </strong>

                        </div>

                    </div>
                </div>
            </div>
        );
    }


    if (
        competition &&
        competitionStatus ===
            'ENDED'
    ) {
        return (
            <div className="min-vh-100 d-flex justify-content-center align-items-center bg-light p-3">

                <div
                    className="card border-0 shadow-lg rounded-4 text-center"
                    style={{
                        maxWidth: '520px',
                        width: '100%'
                    }}
                >

                    <div className="card-body p-5">

                        <img
                            src="/logo.png"
                            alt="HEM Competition"
                            height="55"
                            className="mb-4"
                        />

                        <i
                            className="bi bi-calendar-x text-danger d-block mb-3"
                            style={{
                                fontSize:
                                    '3rem'
                            }}
                        ></i>

                        <h3 className="fw-bold">
                            Competition Closed
                        </h3>

                        <p className="text-muted">
                            The competition time window has ended.
                        </p>

                        <div className="alert alert-danger border-0 rounded-4">

                            <strong>
                                {
                                    competition.competitionName
                                }
                            </strong>

                            <br />

                            Login is no longer available.

                        </div>

                    </div>

                </div>

            </div>
        );
    }

    return (
        <div
            className="min-vh-100 d-flex justify-content-center align-items-center bg-light p-3"
        >

            
            <DisqualifiedModal
    data={disqualifiedData}
    onClose={handleCloseDisqualifiedModal}
/>

<LoginErrorModal
    message={loginError}
    onClose={() => setLoginError('')}
/>
            

            <div
                className="card border-0 shadow-lg rounded-4 w-100"
                style={{
                    maxWidth: '440px'
                }}
            >

                <div className="card-body p-4 p-sm-5">

                    {/* =================================================
                        LOGO & TITLE
                    ================================================= */}

                    <div className="text-center mb-4">

                        <img
                            src="/logo.png"
                            alt="Foundation"
                            height="50"
                            className="object-fit-contain animated-logo mb-3"
                            onError={(e) => {
                                e.currentTarget.style.display =
                                    'none';
                            }}
                        />

                        <h4 className="fw-bold text-dark mb-1">
                            Welcome Back
                        </h4>

                        <p className="text-muted small">
                            Please enter your portal credentials.
                        </p>

                        {competition && (
                            <div className="badge bg-success-subtle text-success rounded-pill px-3 py-2">

                                <i
                                    className="bi bi-circle-fill me-2"
                                    style={{
                                        fontSize:
                                            '7px'
                                    }}
                                ></i>

                                Competition is Live

                            </div>
                        )}

                    </div>

                    {/* =================================================
                        SESSION CONFLICT
                    ================================================= */}

                    {conflictData ? (

                        <div className="alert alert-warning text-center rounded-4 p-4 mb-4 border-0 bg-warning-subtle">

                            <i className="bi bi-exclamation-triangle-fill fs-2 text-warning d-block mb-2"></i>

                            <h6 className="fw-bold text-dark">
                                Already Logged In!
                            </h6>

                            <p className="small text-muted mb-3">
                                {
                                    conflictData.message
                                }
                            </p>

                            <button
                                type="button"
                                onClick={
                                    handleTerminateAndContinue
                                }
                                disabled={
                                    loginLoading
                                }
                                className="btn btn-danger w-100 py-2 rounded-pill fw-bold shadow-sm"
                            >

                                {loginLoading ? (
                                    <>
                                        <span
                                            className="spinner-border spinner-border-sm me-2"
                                            role="status"
                                        ></span>

                                        Processing...
                                    </>
                                ) : (
                                    <>
                                        <i className="bi bi-box-arrow-right me-2"></i>

                                        Terminate Other Session & Continue
                                    </>
                                )}

                            </button>

                        </div>

                    ) : (

                        <>
                            {/* =================================================
                                LOGIN FORM
                            ================================================= */}

                            <form
                                onSubmit={
                                    handleFinalLogin
                                }
                            >

                                {/* USERNAME / EMAIL */}

                                <div className="mb-3 text-start">

                                    <label
                                        htmlFor="loginEmail"
                                        className="form-label small fw-semibold text-muted ms-1"
                                    >
                                        Username or Email
                                    </label>

                                    <input
                                        type="text"
                                        className="form-control rounded-4 bg-light border-0 py-3 px-3 shadow-none"
                                        id="loginEmail"
                                        placeholder="Enter your username or email"
                                        value={
                                            usernameOrEmail
                                        }
                                        onChange={(e) => {
                                            setUsernameOrEmail(
                                                e.target.value
                                            );

                                            // Reset puzzle when
                                            // username changes.
                                            setShowPuzzle(
                                                false
                                            );

                                            setPuzzleSolved(
                                                false
                                            );

                                            setPuzzleError(
                                                ''
                                            );
                                        }}
                                        required
                                        autoComplete="username"
                                    />

                                </div>

                                {/* PASSWORD */}

                                <div className="mb-3 text-start">

                                    <label
                                        htmlFor="loginPassword"
                                        className="form-label small fw-semibold text-muted ms-1"
                                    >
                                        Password
                                    </label>

                                    <input
                                        type="password"
                                        className="form-control rounded-4 bg-light border-0 py-3 px-3 shadow-none"
                                        id="loginPassword"
                                        placeholder="Enter your password"
                                        value={
                                            password
                                        }
                                        onChange={(e) => {
        const value = e.target.value;

        setPassword(value);

        if (
            usernameOrEmail.trim() !== '' &&
            value.trim() !== ''
        ) {
            setNum1(
                Math.floor(
                    Math.random() * 10
                ) + 1
            );

            setNum2(
                Math.floor(
                    Math.random() * 10
                ) + 1
            );

            setUserAnswer('');
            setPuzzleSolved(false);
            setPuzzleError('');
            setShowPuzzle(true);
        } else {
            setShowPuzzle(false);
            setPuzzleSolved(false);
            setPuzzleError('');
        }
    }}
                                        required
                                        autoComplete="current-password"
                                    />

                                </div>

                                {/* =================================================
                                    PUZZLE
                                ================================================= */}

                                {showPuzzle && (

                                    <div className="bg-light p-3 rounded-4 mb-3 border fade-slide-in">

                                        <div className="d-flex justify-content-between align-items-center mb-2">

                                            <span className="small fw-bold text-secondary">
                                                Quick Verification
                                            </span>

                                            <span className="badge bg-success-subtle text-success px-2 py-1">
                                                Security Check
                                            </span>

                                        </div>

                                        <div className="row g-2 align-items-center">

                                            <div className="col-7">

                                                <span className="fw-bold text-dark small">
                                                    Solve:{' '}
                                                    {num1}{' '}
                                                    +{' '}
                                                    {num2}{' '}
                                                    = ?
                                                </span>

                                            </div>

                                            <div className="col-5">

                                                <input
                                                    type="number"
                                                    className="form-control form-control-sm rounded-3 text-center fw-bold shadow-none py-2"
                                                    value={
                                                        userAnswer
                                                    }
                                                    onChange={(e) =>
                                                        setUserAnswer(
                                                            e.target.value
                                                        )
                                                    }
                                                    placeholder="Ans"
                                                    disabled={
                                                        puzzleSolved
                                                    }
                                                    autoFocus
                                                    required
                                                />

                                            </div>

                                            <div className="col-12 mt-2">

                                                {!puzzleSolved ? (

                                                    <button
                                                        type="button"
                                                        onClick={
                                                            handleVerifyPuzzle
                                                        }
                                                        className="btn btn-dark btn-sm w-100 rounded-pill fw-semibold py-2"
                                                    >
                                                        Verify Answer
                                                    </button>

                                                ) : (

                                                    <div className="text-success bg-success-subtle small fw-bold text-center py-2 rounded-3">
                                                        ✓ Verified Successfully!
                                                    </div>

                                                )}

                                                {puzzleError && (

                                                    <div className="text-danger small mt-1 text-center">
                                                        {
                                                            puzzleError
                                                        }
                                                    </div>

                                                )}

                                            </div>

                                        </div>

                                    </div>
                                )}

                                {/* =================================================
                                    LOGIN BUTTON
                                ================================================= */}

                                <button
                                    type="submit"
                                    className={`btn w-100 py-3 rounded-pill fw-bold shadow-sm mt-2 ${
                                        puzzleSolved
                                            ? 'btn-success hover-scale'
                                            : 'btn-secondary opacity-50'
                                    }`}
                                    disabled={
                                        !puzzleSolved ||
                                        loginLoading
                                    }
                                >

                                    {loginLoading ? (
                                        <>
                                            <span
                                                className="spinner-border spinner-border-sm me-2"
                                                role="status"
                                                aria-hidden="true"
                                            ></span>

                                            Signing In...
                                        </>
                                    ) : (
                                        <>
                                            <i className="bi bi-box-arrow-in-right me-2"></i>

                                            Sign In to Dashboard
                                        </>
                                    )}

                                </button>

                            </form>
                        </>
                    )}

                </div>
            </div>
        </div>
    );
}