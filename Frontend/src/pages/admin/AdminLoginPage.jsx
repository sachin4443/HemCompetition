import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../services/api";

export default function AdminLoginPage() {

    const navigate = useNavigate();

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const handleLogin = async (e) => {

        e.preventDefault();

        setError("");

        if (!username.trim() || !password.trim()) {
            setError("Username and password are required.");
            return;
        }

        try {

            setLoading(true);

            const response = await api.post(
                "/AdminAuth/login",
                {
                    username: username.trim(),
                    password
                }
            );

            console.log(
                "ADMIN LOGIN RESPONSE:",
                response.data
            );

            // -------------------------------------------------
            // SAVE ADMIN AUTH
            // -------------------------------------------------

            localStorage.setItem(
                "admin_jwt_token",
                response.data.token
            );

            localStorage.setItem(
                "admin_profile",
                JSON.stringify(response.data.profile)
            );

            // -------------------------------------------------
            // GO ADMIN DASHBOARD
            // -------------------------------------------------

            navigate(
                "/admin/dashboard",
                {
                    replace: true
                }
            );

        } catch (err) {

            console.error(
                "Admin login error:",
                err
            );

            setError(
                err?.response?.data?.message ||
                "Invalid admin username or password."
            );

        } finally {

            setLoading(false);

        }
    };

    return (
        <div
            className="min-vh-100 d-flex align-items-center justify-content-center p-3"
            style={{
                background:
                    "linear-gradient(135deg, #f8f9fa 0%, #e9f7ef 100%)"
            }}
        >

            <div
                className="card border-0 shadow-lg rounded-4"
                style={{
                    width: "100%",
                    maxWidth: "430px"
                }}
            >

                <div className="card-body p-4 p-sm-5">

                    {/* LOGO */}

                    <div className="text-center mb-4">

                        <img
                            src="/logo.png"
                            alt="HEM Competition"
                            height="55"
                            className="object-fit-contain mb-3"
                            onError={(e) => {
                                e.currentTarget.style.display = "none";
                            }}
                        />

                        <div
                            className="bg-success-subtle rounded-circle d-flex align-items-center justify-content-center mx-auto mb-3"
                            style={{
                                width: "70px",
                                height: "70px"
                            }}
                        >
                            <i
                                className="bi bi-shield-lock-fill text-success"
                                style={{
                                    fontSize: "1.8rem"
                                }}
                            />
                        </div>

                        <h4 className="fw-bold text-dark mb-1">
                            Admin Portal
                        </h4>

                        <p className="text-muted small mb-0">
                            Sign in to access the competition dashboard
                        </p>

                    </div>


                    {/* ERROR */}

                    {error && (
                        <div className="alert alert-danger border-0 rounded-4 small">
                            <i className="bi bi-exclamation-circle-fill me-2" />
                            {error}
                        </div>
                    )}


                    {/* FORM */}

                    <form onSubmit={handleLogin}>

                        {/* USERNAME */}

                        <div className="mb-3">

                            <label
                                className="form-label small fw-semibold text-muted"
                            >
                                Username
                            </label>

                            <div className="input-group">

                                <span className="input-group-text bg-light border-0 rounded-start-4">
                                    <i className="bi bi-person-fill text-success" />
                                </span>

                                <input
                                    type="text"
                                    className="form-control bg-light border-0 py-3 shadow-none rounded-end-4"
                                    placeholder="Enter admin username"
                                    value={username}
                                    onChange={(e) =>
                                        setUsername(e.target.value)
                                    }
                                    autoComplete="username"
                                    disabled={loading}
                                />

                            </div>

                        </div>


                        {/* PASSWORD */}

                        <div className="mb-4">

                            <label
                                className="form-label small fw-semibold text-muted"
                            >
                                Password
                            </label>

                            <div className="input-group">

                                <span className="input-group-text bg-light border-0 rounded-start-4">
                                    <i className="bi bi-key-fill text-success" />
                                </span>

                                <input
                                    type="password"
                                    className="form-control bg-light border-0 py-3 shadow-none rounded-end-4"
                                    placeholder="Enter admin password"
                                    value={password}
                                    onChange={(e) =>
                                        setPassword(e.target.value)
                                    }
                                    autoComplete="current-password"
                                    disabled={loading}
                                />

                            </div>

                        </div>


                        {/* BUTTON */}

                        <button
                            type="submit"
                            className="btn btn-success w-100 py-3 rounded-pill fw-bold shadow-sm"
                            disabled={loading}
                        >

                            {loading ? (
                                <>
                                    <span
                                        className="spinner-border spinner-border-sm me-2"
                                    />

                                    Signing In...
                                </>
                            ) : (
                                <>
                                    <i className="bi bi-box-arrow-in-right me-2" />
                                    Admin Sign In
                                </>
                            )}

                        </button>

                    </form>


                    <div className="text-center mt-4">

                        <span className="small text-muted">
                            Authorized personnel only
                        </span>

                    </div>

                </div>

            </div>

        </div>
    );
}