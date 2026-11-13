import { useCallback, useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../services/api";
import DeveloperMetricsView from "../../components/admin/DeveloperMetricsView";
import DeveloperRealtimeTelemetry from "../../components/admin/DeveloperRealtimeTelemetry";
import DeveloperReportDownloads from "../../components/admin/DeveloperReportDownloads";
import LiveSummaryView from "../../components/admin/LiveCompetitionSummary";
import SystemDiagnosticsView from "../../components/admin/SystemDiagnosticsView";
const formatNumber = (value) => Number(value || 0).toLocaleString("en-IN");

const formatTime = (date) => {
  if (!date) return "--";
  return new Intl.DateTimeFormat("en-IN", {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  }).format(date);
};

// =====================================================
// MULTI-METRIC REALTIME GRAPH
// =====================================================
// =====================================================
// LIVE USER STATUS GRAPH
// =====================================================
function LiveUserStatusChart({ data }) {
  const width = 760;
  const height = 280;

  const [hoverIndex, setHoverIndex] = useState(null);

  if (!data || data.length === 0) {
    return (
      <div
        className="d-flex align-items-center justify-content-center text-muted"
        style={{ height: "280px" }}
      >
        Collecting user activity data...
      </div>
    );
  }

  const maxValue = Math.max(
    ...data.flatMap((item) => [
      Number(item.online || 0),
      Number(item.offline || 0),
      Number(item.inactive || 0),
      Number(item.disqualified || 0),
    ]),
    10
  );

  const chartTop = 20;
  const chartBottom = 225;
  const chartHeight = chartBottom - chartTop;

  const getX = (index) => {
    if (data.length === 1) {
      return width / 2;
    }

    return (index / (data.length - 1)) * width;
  };

  const getY = (value) => {
    return (
      chartBottom -
      (Number(value || 0) / maxValue) * chartHeight
    );
  };

  const createPoints = (key) => {
    return data
      .map((item, index) => {
        return `${getX(index)},${getY(item[key])}`;
      })
      .join(" ");
  };

  const onlinePoints = createPoints("online");
  const offlinePoints = createPoints("offline");
  const inactivePoints = createPoints("inactive");
  const disqualifiedPoints = createPoints("disqualified");

  const hovered = hoverIndex !== null
    ? data[hoverIndex]
    : null;

  return (
    <div style={{ width: "100%" }}>

      {/* =================================================
          LEGEND
      ================================================= */}
      <div className="d-flex flex-wrap gap-3 mb-3 small">

        <div className="d-flex align-items-center gap-2">
          <span
            className="rounded-circle bg-success"
            style={{
              width: "9px",
              height: "9px",
            }}
          />
          <span className="text-muted">
            Online
          </span>
        </div>

        <div className="d-flex align-items-center gap-2">
          <span
            className="rounded-circle bg-warning"
            style={{
              width: "9px",
              height: "9px",
            }}
          />
          <span className="text-muted">
            Offline
          </span>
        </div>

        <div className="d-flex align-items-center gap-2">
          <span
            className="rounded-circle bg-danger"
            style={{
              width: "9px",
              height: "9px",
            }}
          />
          <span className="text-muted">
            Inactive / Deactivated
          </span>
        </div>

        <div className="d-flex align-items-center gap-2">
          <span
            className="rounded-circle bg-purple"
            style={{
              width: "9px",
              height: "9px",
              background: "#6f42c1",
            }}
          />
          <span className="text-muted">
            Disqualified
          </span>
        </div>

      </div>

      {/* =================================================
          CHART
      ================================================= */}
      <div
        style={{
          position: "relative",
          width: "100%",
        }}
      >

        <svg
          viewBox={`0 0 ${width} ${height}`}
          width="100%"
          height="280"
          preserveAspectRatio="none"
          style={{
            overflow: "visible",
          }}
        >

          {/* GRID */}
          {[0, 1, 2, 3, 4].map((line) => {

            const y =
              chartTop +
              line * (chartHeight / 4);

            const value = Math.round(
              maxValue -
              (line / 4) * maxValue
            );

            return (
              <g key={line}>

                <line
                  x1="0"
                  x2={width}
                  y1={y}
                  y2={y}
                  stroke="#edf0f3"
                  strokeWidth="1"
                />

                <text
                  x="4"
                  y={y - 5}
                  fontSize="11"
                  fill="#8a94a6"
                >
                  {value.toLocaleString("en-IN")}
                </text>

              </g>
            );
          })}

          {/* BASELINE */}
          <line
            x1="0"
            x2={width}
            y1={chartBottom}
            y2={chartBottom}
            stroke="#dfe3e8"
            strokeWidth="1"
          />

          {/* ONLINE AREA */}
          <polygon
            points={`
              0,${chartBottom}
              ${onlinePoints}
              ${width},${chartBottom}
            `}
            fill="rgba(25,135,84,0.08)"
          />

          {/* ONLINE */}
          <polyline
            points={onlinePoints}
            fill="none"
            stroke="#198754"
            strokeWidth="3"
            strokeLinecap="round"
            strokeLinejoin="round"
          />

          {/* OFFLINE */}
          <polyline
            points={offlinePoints}
            fill="none"
            stroke="#f59f00"
            strokeWidth="2.5"
            strokeLinecap="round"
            strokeLinejoin="round"
          />

          {/* INACTIVE */}
          <polyline
            points={inactivePoints}
            fill="none"
            stroke="#dc3545"
            strokeWidth="2.5"
            strokeLinecap="round"
            strokeLinejoin="round"
          />

          {/* DISQUALIFIED */}
          <polyline
            points={disqualifiedPoints}
            fill="none"
            stroke="#6f42c1"
            strokeWidth="2.5"
            strokeDasharray="5 5"
            strokeLinecap="round"
            strokeLinejoin="round"
          />

          {/* POINTS */}
          {data.map((item, index) => {

            const x = getX(index);

            const onlineY = getY(item.online);
            const offlineY = getY(item.offline);
            const inactiveY = getY(item.inactive);
            const disqualifiedY = getY(item.disqualified);

            return (
              <g key={index}>

                {/* Invisible hover area */}
                <rect
                  x={
                    data.length === 1
                      ? x - 40
                      : Math.max(0, x - 25)
                  }
                  y="0"
                  width={
                    data.length === 1
                      ? 80
                      : 50
                  }
                  height={height}
                  fill="transparent"
                  style={{
                    cursor: "crosshair",
                  }}
                  onMouseEnter={() =>
                    setHoverIndex(index)
                  }
                  onMouseLeave={() =>
                    setHoverIndex(null)
                  }
                />

                {/* ONLINE DOT */}
                <circle
                  cx={x}
                  cy={onlineY}
                  r={hoverIndex === index ? 6 : 4}
                  fill="white"
                  stroke="#198754"
                  strokeWidth="3"
                  pointerEvents="none"
                />

                {/* OFFLINE DOT */}
                <circle
                  cx={x}
                  cy={offlineY}
                  r={hoverIndex === index ? 5 : 3}
                  fill="white"
                  stroke="#f59f00"
                  strokeWidth="2"
                  pointerEvents="none"
                />

                {/* INACTIVE DOT */}
                <circle
                  cx={x}
                  cy={inactiveY}
                  r={hoverIndex === index ? 5 : 3}
                  fill="white"
                  stroke="#dc3545"
                  strokeWidth="2"
                  pointerEvents="none"
                />

                {/* DISQUALIFIED DOT */}
                <circle
                  cx={x}
                  cy={disqualifiedY}
                  r={hoverIndex === index ? 5 : 3}
                  fill="white"
                  stroke="#6f42c1"
                  strokeWidth="2"
                  pointerEvents="none"
                />

              </g>
            );
          })}

        </svg>

        {/* =================================================
            TOOLTIP
        ================================================= */}
        {hovered && (
          <div
            className="position-absolute bg-dark text-white rounded-4 shadow-lg p-3"
            style={{
              width: "220px",
              left:
                data.length === 1
                  ? "50%"
                  : `${(hoverIndex / (data.length - 1)) * 100}%`,
              top: "10px",
              transform:
                hoverIndex === 0
                  ? "translateX(0)"
                  : hoverIndex === data.length - 1
                  ? "translateX(-100%)"
                  : "translateX(-50%)",
              zIndex: 50,
              pointerEvents: "none",
              fontSize: "12px",
            }}
          >

            <div className="fw-bold mb-2">
              {hovered.time}
            </div>

            <div className="d-flex justify-content-between mb-1">
              <span>
                <span
                  className="d-inline-block rounded-circle bg-success me-2"
                  style={{
                    width: "7px",
                    height: "7px",
                  }}
                />
                Online
              </span>

              <strong>
                {Number(hovered.online || 0).toLocaleString("en-IN")}
              </strong>
            </div>

            <div className="d-flex justify-content-between mb-1">
              <span>
                <span
                  className="d-inline-block rounded-circle bg-warning me-2"
                  style={{
                    width: "7px",
                    height: "7px",
                  }}
                />
                Offline
              </span>

              <strong>
                {Number(hovered.offline || 0).toLocaleString("en-IN")}
              </strong>
            </div>

            <div className="d-flex justify-content-between mb-1">
              <span>
                <span
                  className="d-inline-block rounded-circle bg-danger me-2"
                  style={{
                    width: "7px",
                    height: "7px",
                  }}
                />
                Inactive
              </span>

              <strong>
                {Number(hovered.inactive || 0).toLocaleString("en-IN")}
              </strong>
            </div>

            <div className="d-flex justify-content-between">
              <span>
                <span
                  className="d-inline-block rounded-circle me-2"
                  style={{
                    width: "7px",
                    height: "7px",
                    background: "#6f42c1",
                  }}
                />
                Disqualified
              </span>

              <strong>
                {Number(hovered.disqualified || 0).toLocaleString("en-IN")}
              </strong>
            </div>

          </div>
        )}

      </div>

      {/* =================================================
          FOOTER
      ================================================= */}
      <div className="d-flex justify-content-between align-items-center small text-muted border-top pt-3 mt-2">

        <span>
          Rolling window: Last 12 intervals
        </span>

        <span>
          Current Online:{" "}
          <strong className="text-success">
            {Number(
              data[data.length - 1]?.online || 0
            ).toLocaleString("en-IN")}
          </strong>
        </span>

      </div>

    </div>
  );
}

// =====================================================
// STAT CARD COMPONENT
// =====================================================
function StatCard({ title, value, icon, subtitle, tone = "success" }) {
  const tones = {
    success: { bg: "#e9f7ef", color: "#198754" },
    primary: { bg: "#eaf2ff", color: "#0d6efd" },
    warning: { bg: "#fff4df", color: "#f59f00" },
    danger: { bg: "#fdecec", color: "#dc3545" },
    dark: { bg: "#eef0f2", color: "#343a40" },
  };

  const theme = tones[tone] || tones.success;

  return (
    <div className="col-12 col-sm-6 col-xl-3">
      <div className="card border-0 shadow-sm rounded-4 h-100">
        <div className="card-body p-3 p-md-4">
          <div className="d-flex justify-content-between align-items-start">
            <div>
              <div className="text-muted small fw-semibold text-uppercase" style={{ letterSpacing: "0.05em" }}>
                {title}
              </div>
              <div className="fw-bold mt-2 fs-2">{formatNumber(value)}</div>
              <div className="text-muted small mt-1">{subtitle}</div>
            </div>
            <div
              className="rounded-4 d-flex align-items-center justify-content-center flex-shrink-0"
              style={{
                width: "48px",
                height: "48px",
                background: theme.bg,
                color: theme.color,
                fontSize: "1.25rem",
              }}
            >
              <i className={`bi ${icon}`} />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

// =====================================================
// MAIN DASHBOARD COMPONENT
// =====================================================
export default function AdminDashboard() {
  const navigate = useNavigate();

  const initialProfile = useMemo(() => {
    const saved = localStorage.getItem("admin_profile");
    if (!saved) return null;
    try {
      return JSON.parse(saved);
    } catch {
      localStorage.removeItem("admin_profile");
      return null;
    }
  }, []);

  const [profile] = useState(initialProfile);
  const [activeTab, setActiveTab] = useState("overview");

  const [stats, setStats] = useState({
    totalUsers: 0,
    currentLoginUsers: 0,
    totalUnloggedUsers: 0,
    totalDisqualifiedUsers: 0,
    totalActiveSessions: 0,
    totalActiveUsers: 0,
    totalInactiveUsers: 0,
  });

  const [metrics, setMetrics] = useState(null);
  const [diagnostics, setDiagnostics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState("");
  const [lastUpdated, setLastUpdated] = useState(null);
  const [liveHistory, setLiveHistory] = useState([]);

  const loadDashboardStats = useCallback(
    async (silent = false) => {
      try {
        if (silent) setRefreshing(true);
        else setLoading(true);

        setError("");
        const response = await api.get("/admin-dashboard/stats");
        const data = response.data;

        const nextStats = {
          totalUsers: data.totalUsers ?? 0,
          currentLoginUsers: data.currentLoginUsers ?? 0,
          totalUnloggedUsers: data.totalUnloggedUsers ?? 0,
          totalDisqualifiedUsers: data.totalDisqualifiedUsers ?? 0,
          totalActiveSessions: data.totalActiveSessions ?? 0,
          totalActiveUsers: data.totalActiveUsers ?? 0,
          totalInactiveUsers: data.totalInactiveUsers ?? 0,
        };

        setStats(nextStats);
        if (data.metrics) setMetrics(data.metrics);

        const now = new Date();
        setLastUpdated(now);

        setLiveHistory((prev) => {
  const next = [
    ...prev,
    {
      time: formatTime(now),

      // Currently logged-in users
      online: nextStats.currentLoginUsers,

      // Active accounts but currently not logged-in
      offline: nextStats.totalUnloggedUsers,

      // Inactive / deactivated accounts
      inactive: nextStats.totalInactiveUsers,

      // Security / anti-cheat disqualified users
      disqualified: nextStats.totalDisqualifiedUsers,
    },
  ];

  return next.slice(-12);
});
      } catch (err) {
        console.error("Dashboard stats error:", err);
        if (err?.response?.status === 401) {
          localStorage.removeItem("admin_jwt_token");
          localStorage.removeItem("admin_profile");
          navigate("/admin", { replace: true });
          return;
        }
        setError(err?.response?.data?.message || "Unable to load dashboard data.");
      } finally {
        setLoading(false);
        setRefreshing(false);
      }
    },
    [navigate]
  );
  
  const loadDiagnostics = useCallback(async () => {
  try {
    const response = await api.get(
      "/admin-dashboard/diagnostics"
    );

    setDiagnostics(
      response.data?.data || null
    );
  } catch (err) {
    console.error(
      "Diagnostics error:",
      err
    );
  }
}, []);

useEffect(() => {
  if (activeTab !== "diagnostics") {
    return;
  }

  loadDiagnostics();

  const interval = setInterval(
    loadDiagnostics,
    5000
  );

  return () => clearInterval(interval);
}, [
  activeTab,
  loadDiagnostics
]);
  useEffect(() => {
    let cancelled = false;
    const start = async () => {
      if (!cancelled) await loadDashboardStats(false);
    };
    start();

    const interval = setInterval(() => {
      if (!cancelled) loadDashboardStats(true);
    }, 5000);

    return () => {
      cancelled = true;
      clearInterval(interval);
    };
  }, [loadDashboardStats]);

  const handleLogout = () => {
    localStorage.removeItem("admin_jwt_token");
    localStorage.removeItem("admin_profile");
    navigate("/admin", { replace: true });
  };

  const activePercentage =
    stats.totalUsers > 0
      ? Math.round((stats.totalActiveUsers / stats.totalUsers) * 100)
      : 0;

  const loginPercentage =
    stats.totalActiveUsers > 0
      ? Math.round((stats.currentLoginUsers / stats.totalActiveUsers) * 100)
      : 0;

  if (loading && !stats.totalUsers) {
    return (
      <div className="min-vh-100 d-flex justify-content-center align-items-center bg-light">
        <div className="spinner-border text-success me-2" role="status" />
        <span className="fw-semibold text-secondary">Loading Competition Command Center...</span>
      </div>
    );
  }

  return (
    <div className="min-vh-100" style={{ background: "#f7f9fb" }}>
      {/* Header */}
      <header className="bg-white border-bottom sticky-top" style={{ zIndex: 100 }}>
        <div className="container-fluid px-3 px-lg-4">
          <div className="d-flex align-items-center justify-content-between" style={{ minHeight: "72px" }}>
            <div className="d-flex align-items-center gap-3">
              <div
                className="rounded-4 d-flex align-items-center justify-content-center"
                style={{ width: "44px", height: "44px", background: "#e9f7ef", color: "#198754" }}
              >
                <i className="bi bi-speedometer2 fs-5" />
              </div>
              <div>
                <h5 className="fw-bold mb-0">Competition</h5>
                <div className="small text-muted">
                  Updated: <strong>{formatTime(lastUpdated)}</strong>
                </div>
              </div>
            </div>

            <div className="d-flex align-items-center gap-2">

  <button
    type="button"
    className={`btn btn-sm rounded-pill px-3 fw-semibold ${
      activeTab === "summary"
        ? "btn-primary"
        : "btn-outline-primary"
    }`}
    onClick={() => setActiveTab("summary")}
  >
    <i className="bi bi-bar-chart-line-fill me-1"></i>
    <span className="d-none d-lg-inline">
      Live Summary
    </span>
  </button>

  <DeveloperReportDownloads />

  {/* LIVE */}
  <div
    className="d-none d-md-flex align-items-center gap-2 px-3 py-2 rounded-pill"
    style={{
      background: "#e9f7ef",
      color: "#198754",
    }}
  >
    <span
      style={{
        width: "8px",
        height: "8px",
        borderRadius: "50%",
        background: "#198754",
        display: "inline-block",
        boxShadow: "0 0 0 4px rgba(25,135,84,.15)",
      }}
    />

    <span className="small fw-bold">
      LIVE 5s
    </span>
  </div>

  {/* REFRESH */}
  <button
    type="button"
    className="btn btn-light rounded-circle"
    style={{
      width: "40px",
      height: "40px",
    }}
    onClick={() => loadDashboardStats(true)}
    disabled={refreshing}
    title="Refresh"
  >
    <i
      className={
        refreshing
          ? "bi bi-arrow-repeat spin"
          : "bi bi-arrow-clockwise"
      }
    />
  </button>

  {/* LOGOUT */}
  <button
    className="btn btn-outline-danger btn-sm rounded-pill px-3"
    onClick={handleLogout}
  >
    <i className="bi bi-box-arrow-right me-1"></i>
    Logout
  </button>

</div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="container-fluid px-3 px-lg-4 py-4">
        <div className="d-flex flex-wrap justify-content-between align-items-center mb-4 gap-3">
          <div>
            <h3 className="fw-bold mb-1">
              Welcome back, {profile?.displayName || profile?.username || "Administrator"}
            </h3>
            <p className="text-muted small mb-0">
              Live monitoring of competition sessions, login status, and server API logs.
            </p>
          </div>

          <div className="btn-group bg-white p-1 rounded-pill shadow-sm border">
            <button
              className={`btn btn-sm rounded-pill px-3 fw-bold ${activeTab === "overview" ? "btn-success" : "btn-light border-0"}`}
              onClick={() => setActiveTab("overview")}
            >
              <i className="bi bi-grid-fill me-1"></i> Live Competition
            </button>
            <button
              className={`btn btn-sm rounded-pill px-3 fw-bold ${activeTab === "developer" ? "btn-dark text-white" : "btn-light border-0"}`}
              onClick={() => setActiveTab("developer")}
            >
              <i className="bi bi-terminal-fill me-1"></i> Developer & API Logs
            </button>
            <button
  className={`btn btn-sm rounded-pill px-3 fw-bold ${
    activeTab === "diagnostics"
      ? "btn-danger"
      : "btn-light border-0"
  }`}
  onClick={() => setActiveTab("diagnostics")}
>
  <i className="bi bi-speedometer2 me-1"></i>
  System Diagnostics
</button>
<button
  className="btn btn-sm btn-light rounded-pill px-3 fw-bold"
  onClick={() => navigate("/admin/sessions")}
>
  <i className="bi bi-people-fill me-1"></i>
  User Sessions
</button>
          </div>
        </div>

        {error && (
          <div className="alert alert-danger border-0 rounded-4 shadow-sm mb-4">
            <i className="bi bi-exclamation-triangle-fill me-2" /> {error}
          </div>
        )}

        {/* 4 Primary Stat Cards */}
        <div className="row g-3 mb-4">
          <StatCard
            title="Online Users"
            value={stats.currentLoginUsers}
            icon="bi-broadcast-pin"
            subtitle={`${loginPercentage}% of active pool`}
            tone="success"
          />
          <StatCard
            title="Active Sessions"
            value={stats.totalActiveSessions}
            icon="bi-activity"
            subtitle="Current JWT tab sessions"
            tone="primary"
          />
          <StatCard
            title="Offline Users"
            value={stats.totalUnloggedUsers}
            icon="bi-person-dash-fill"
            subtitle="Eligible but not playing"
            tone="warning"
          />
          <StatCard
            title="Security Disqualified"
            value={stats.totalDisqualifiedUsers}
            icon="bi-shield-x"
            subtitle="Policy & tab switch breach"
            tone="danger"
          />
        </div>

        {/* Tab 1: Live Overview */}
        {activeTab === "overview" && (
          <div className="row g-4 mb-4">
            <div className="col-12 col-xl-8">
              <div className="card border-0 shadow-sm rounded-4 h-100">
                <div className="card-body p-4">
                  <div className="d-flex justify-content-between align-items-start mb-3">
                    <div>
                     <div className="fw-bold fs-5">
  Live User Activity
</div>

<div className="small text-muted">
  Real-time user login, offline, inactive and security status
</div>
                    </div>
                    <span className="badge bg-success-subtle text-success rounded-pill px-3 py-2">
                      <i className="bi bi-activity me-1" /> Real-time
                    </span>
                  </div>

                 <LiveUserStatusChart data={liveHistory} />

                 
                </div>
              </div>
            </div>

            <div className="col-12 col-xl-4">
              <div className="card border-0 shadow-sm rounded-4 h-100">
                <div className="card-body p-4">
                  <div className="fw-bold fs-5">Account Distribution</div>
                  <div className="small text-muted mb-4">Competition participant status</div>

                  <div className="mb-3">
                    <div className="d-flex justify-content-between mb-1 small fw-semibold">
                      <span>Active Accounts</span>
                      <span className="text-success">{formatNumber(stats.totalActiveUsers)}</span>
                    </div>
                    <div className="progress" style={{ height: "8px" }}>
                      <div className="progress-bar bg-success" style={{ width: `${activePercentage}%` }} />
                    </div>
                  </div>

                  <div className="mb-4">
                    <div className="d-flex justify-content-between mb-1 small fw-semibold">
                      <span>Inactive / Deactivated</span>
                      <span className="text-danger">{formatNumber(stats.totalInactiveUsers)}</span>
                    </div>
                    <div className="progress" style={{ height: "8px" }}>
                      <div className="progress-bar bg-danger" style={{ width: `${100 - activePercentage}%` }} />
                    </div>
                  </div>

                  <div className="rounded-4 p-3" style={{ background: "#fff7e6" }}>
                    <div className="d-flex align-items-center gap-3">
                      <div
                        className="rounded-circle d-flex align-items-center justify-content-center flex-shrink-0"
                        style={{ width: "42px", height: "42px", background: "#ffe8b3", color: "#f59f00" }}
                      >
                        <i className="bi bi-shield-check fs-5" />
                      </div>
                      <div>
                        <div className="fw-semibold">Anti-Cheat System</div>
                        <div className="small text-muted">
                          <strong>{formatNumber(stats.totalDisqualifiedUsers)}</strong> total users terminated due to policy breaches.
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Tab 2: Developer Metrics */}
       {activeTab === "developer" && (
  <>
    <DeveloperRealtimeTelemetry
      metrics={metrics}
    />

    <DeveloperMetricsView
      metrics={metrics}
    />
  </>
)}
{/* =====================================================
    TAB 3: LIVE SUMMARY
===================================================== */}
{activeTab === "summary" && (
  <LiveSummaryView />
)}
{/* =====================================================
    TAB 3: SYSTEM DIAGNOSTICS
===================================================== */}

{activeTab === "diagnostics" && (
  <SystemDiagnosticsView
    diagnostics={diagnostics}
  />
)}


{/* =====================================================
    TAB 4: LIVE SUMMARY
===================================================== */}

{activeTab === "summary" && (
  <LiveSummaryView />
)}
      </main>
    </div>
  );
}