import "bootstrap-icons/font/bootstrap-icons.css";

export default function DashboardHeader({
    dashboardData,
    currentDate,
    initials,
    handleLogout,
    onOpenGuide 
}) {

    return (
        <header className="bg-white rounded-4 shadow-sm border px-3 px-md-4 py-3 mb-4">
            <div className="d-flex align-items-center justify-content-between flex-nowrap gap-2">

                {/* Logo */}
                <div className="d-flex align-items-center flex-shrink-0">
                    <img
                        src="/logo.png"
                        alt="Logo"
                        className="img-fluid"
                        style={{
                            maxHeight: '40px',
                            objectFit: "contain"
                        }}
                    />
                </div>

                {/* Right Side: User Guide Button & Profile Dropdown */}
                <div className="d-flex align-items-center gap-2 flex-shrink-0">
                    <button
                        onClick={onOpenGuide}
                        className="btn btn-info text-white rounded-pill px-2 px-md-3 py-2 d-flex align-items-center fw-semibold shadow-sm text-nowrap"
                        style={{ fontSize: "0.85rem" }}
                    >
                        <i className="bi bi-journal-bookmark-fill me-1 me-md-2"></i>
                        <span className="d-none d-md-inline">User Guide</span>
                    </button>

                    {/* Profile Dropdown */}
                    <div className="dropdown">
                        <button
                            className="btn btn-light border rounded-pill d-flex align-items-center px-2 py-1"
                            data-bs-toggle="dropdown"
                        >
                            <span
                                className="bg-success rounded-circle text-white fw-bold d-flex align-items-center justify-content-center"
                                style={{
                                    width: 36,
                                    height: 36,
                                    fontSize: "0.9rem"
                                }}
                            >
                                {initials}
                            </span>
                        </button>

                        <ul
                            className="dropdown-menu dropdown-menu-end border-0 shadow rounded-4 mt-2"
                            style={{
                                minWidth: 260
                            }}
                        >
                            <li className="px-3 py-3">
                                <div className="d-flex align-items-center">
                                    <span
                                        className="bg-success rounded-circle text-white fw-bold d-flex align-items-center justify-content-center me-3"
                                        style={{
                                            width: 45,
                                            height: 45
                                        }}
                                    >
                                        {initials}
                                    </span>
                                    <div>
                                        <div className="fw-bold">
                                            {dashboardData?.firstName} {dashboardData?.lastName}
                                        </div>
                                        <small className="text-muted">
                                            {dashboardData?.email}
                                        </small>
                                    </div>
                                </div>
                            </li>
                            <li>
                                <hr className="my-2"/>
                            </li>
                            <li>
                                <button
                                    className="dropdown-item py-2"
                                    onClick={handleLogout}
                                >
                                    <i className="bi bi-box-arrow-right text-danger me-2"></i>
                                    Logout
                                </button>
                            </li>
                        </ul>
                    </div>
                </div>

            </div>
        </header>
    );
}