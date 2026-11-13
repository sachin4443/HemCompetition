import { useCallback, useEffect, useMemo, useState } from "react";
import api from "../../services/api";

export default function AdminSessionsPage() {
  // =====================================================
  // DATA & STATE
  // =====================================================

  const [sessions, setSessions] = useState([]);
  const [stats, setStats] = useState(null);

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [exporting, setExporting] = useState(false);

  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(50);

  const [total, setTotal] = useState(0);
  const [totalPages, setTotalPages] = useState(1);

  const [lastRefresh, setLastRefresh] = useState(null);

  const [selectedSession, setSelectedSession] = useState(null);
  const [detailsLoading, setDetailsLoading] = useState(false);

  // =====================================================
  // LOAD SESSIONS
  // =====================================================

  const loadSessions = useCallback(
    async ({ initial = false, manual = false } = {}) => {
      try {
        if (initial) {
          setLoading(true);
        }

        if (manual) {
          setRefreshing(true);
        }

        const response = await api.get("/admin-sessions", {
          params: {
            search: search || undefined,
            status: status || undefined,
            from: from || undefined,
            to: to || undefined,
            page,
            pageSize,
          },
        });

        const result = response.data;

        setSessions(
          Array.isArray(result?.data)
            ? result.data
            : []
        );

        setTotal(
          Number(result?.total || 0)
        );

        setTotalPages(
          Math.max(
            Number(result?.totalPages || 1),
            1
          )
        );

        setLastRefresh(new Date());
      } catch (error) {
        console.error(
          "Failed to load sessions:",
          error
        );
      } finally {
        if (initial) {
          setLoading(false);
        }

        if (manual) {
          setRefreshing(false);
        }
      }
    },
    [
      search,
      status,
      from,
      to,
      page,
      pageSize,
    ]
  );

  // =====================================================
  // LOAD STATS
  // =====================================================

  const loadStats = useCallback(async () => {
    try {
      const response = await api.get(
        "/admin-sessions/stats"
      );

      setStats(response.data);
    } catch (error) {
      console.error(
        "Failed to load session stats:",
        error
      );
    }
  }, []);

  // =====================================================
  // INITIAL LOAD + FILTER/PAGE LOAD
  // =====================================================

  useEffect(() => {
    loadSessions({
      initial: true,
    });

    loadStats();
  }, []);

  useEffect(() => {
    if (loading) {
      return;
    }

    loadSessions();
  }, [
    search,
    status,
    from,
    to,
    page,
    pageSize,
  ]);

  // =====================================================
  // AUTO REFRESH - EVERY 60 SECONDS
  // =====================================================

  useEffect(() => {
    const interval = setInterval(() => {
      loadSessions();
      loadStats();
    }, 60000);

    return () => {
      clearInterval(interval);
    };
  }, [
    loadSessions,
    loadStats,
  ]);

  // =====================================================
  // MANUAL REFRESH
  // =====================================================

  const manualRefresh = async () => {
    try {
      setRefreshing(true);

      await Promise.all([
        loadSessions(),
        loadStats(),
      ]);
    } finally {
      setRefreshing(false);
    }
  };

  // =====================================================
  // VIEW SESSION DETAILS
  // =====================================================

  const viewDetails = async (
    sessionId
  ) => {
    try {
      setDetailsLoading(true);

      const response = await api.get(
        `/admin-sessions/${sessionId}`
      );

      setSelectedSession(
        response.data?.data || null
      );
    } catch (error) {
      console.error(
        "Failed to load session:",
        error
      );
    } finally {
      setDetailsLoading(false);
    }
  };

  // =====================================================
  // EXPORT CSV
  // =====================================================

  const exportCsv = async () => {
    try {
      setExporting(true);

      const response = await api.get(
        "/admin-sessions/export",
        {
          params: {
            search:
              search || undefined,

            status:
              status || undefined,

            from:
              from || undefined,

            to:
              to || undefined,
          },

          responseType: "blob",
        }
      );

      const blob = new Blob(
        [response.data],
        {
          type: "text/csv;charset=utf-8;",
        }
      );

      const url =
        window.URL.createObjectURL(
          blob
        );

      const link =
        document.createElement("a");

      link.href = url;

      link.download =
        `user-sessions-${new Date()
          .toISOString()
          .slice(0, 10)}.csv`;

      document.body.appendChild(link);

      link.click();

      link.remove();

      window.URL.revokeObjectURL(
        url
      );
    } catch (error) {
      console.error(
        "Export failed:",
        error
      );
    } finally {
      setExporting(false);
    }
  };

  // =====================================================
  // RESET FILTERS
  // =====================================================

  const resetFilters = () => {
    setSearch("");
    setStatus("");
    setFrom("");
    setTo("");
    setPage(1);
  };

  // =====================================================
  // HELPERS
  // =====================================================

  const formatDate = (value) => {
    if (!value) {
      return "--";
    }

    return new Date(value).toLocaleString(
      "en-IN",
      {
        dateStyle: "short",
        timeStyle: "medium",
      }
    );
  };

  const getFullName = (session) => {
    if (session?.fullName) {
      return session.fullName;
    }

    const name =
      `${session?.firstName || ""} ${
        session?.lastName || ""
      }`.trim();

    return (
      name ||
      session?.username ||
      "Unknown User"
    );
  };

  const lastRefreshText = lastRefresh
    ? lastRefresh.toLocaleTimeString(
        "en-IN",
        {
          hour: "2-digit",
          minute: "2-digit",
          second: "2-digit",
        }
      )
    : "--";

  const showingFrom =
    total === 0
      ? 0
      : (page - 1) * pageSize + 1;

  const showingTo = Math.min(
    page * pageSize,
    total
  );

  // =====================================================
  // PAGINATION
  // =====================================================

  const paginationItems = useMemo(() => {
    const items = [];

    if (totalPages <= 7) {
      for (
        let i = 1;
        i <= totalPages;
        i++
      ) {
        items.push(i);
      }

      return items;
    }

    items.push(1);

    if (page > 4) {
      items.push("...");
    }

    const start = Math.max(
      2,
      page - 1
    );

    const end = Math.min(
      totalPages - 1,
      page + 1
    );

    for (
      let i = start;
      i <= end;
      i++
    ) {
      items.push(i);
    }

    if (
      page <
      totalPages - 3
    ) {
      items.push("...");
    }

    items.push(totalPages);

    return items;
  }, [
    page,
    totalPages,
  ]);

  // =====================================================
  // RENDER
  // =====================================================

  return (
    <div className="d-flex flex-column min-vh-100 mesh-bg">

      {/* =================================================
          GLOBAL CSS
      ================================================= */}

      <style>
        {`
          .mesh-bg {
            background-color: #f8fafc;
            background-image:
              radial-gradient(
                at 0% 0%,
                hsla(213,100%,94%,1) 0px,
                transparent 50%
              ),
              radial-gradient(
                at 98% 10%,
                hsla(258,100%,95%,1) 0px,
                transparent 50%
              ),
              radial-gradient(
                at 5% 95%,
                hsla(190,100%,94%,1) 0px,
                transparent 50%
              ),
              radial-gradient(
                at 93% 90%,
                hsla(286,100%,95%,1) 0px,
                transparent 50%
              );
          }

          .glass-panel {
            background: rgba(255, 255, 255, 0.6);
            backdrop-filter: blur(16px);
            -webkit-backdrop-filter: blur(16px);
            border: 1px solid rgba(255, 255, 255, 0.9);
            box-shadow:
              0 4px 24px rgba(0, 0, 0, 0.03);
          }

          .table-glass-header th {
            background: rgba(
              255,
              255,
              255,
              0.5
            ) !important;

            backdrop-filter: blur(12px);

            border-bottom:
              1px solid
              rgba(
                200,
                200,
                200,
                0.3
              ) !important;

            font-size: 0.75rem;

            text-transform: uppercase;

            letter-spacing: 0.5px;

            color: #64748b;
          }

          .table-transparent {
            --bs-table-bg: transparent;
            --bs-table-hover-bg:
              rgba(255,255,255,0.5);
          }

          .table-transparent td {
            border-bottom:
              1px solid
              rgba(0,0,0,0.04);

            padding:
              0.65rem 0.75rem;

            font-size: 0.85rem;

            vertical-align: middle;

            white-space: nowrap;
          }

          .glass-input {
            background:
              rgba(
                255,
                255,
                255,
                0.7
              ) !important;

            border:
              1px solid
              rgba(
                255,
                255,
                255,
                0.9
              ) !important;

            box-shadow:
              inset
              0 2px 4px
              rgba(
                0,
                0,
                0,
                0.01
              ) !important;

            backdrop-filter: blur(4px);

            font-size: 0.85rem;
          }

          .glass-input:focus {
            background:
              #ffffff !important;

            box-shadow:
              0 0 0 3px
              rgba(
                99,
                102,
                241,
                0.15
              ) !important;

            border-color:
              #818cf8 !important;
          }

          .premium-scrollbar::-webkit-scrollbar {
            width: 6px;
            height: 6px;
          }

          .premium-scrollbar::-webkit-scrollbar-track {
            background: transparent;
          }

          .premium-scrollbar::-webkit-scrollbar-thumb {
            background:
              rgba(
                148,
                163,
                184,
                0.4
              );

            border-radius: 10px;
          }

          .premium-scrollbar::-webkit-scrollbar-thumb:hover {
            background:
              rgba(
                148,
                163,
                184,
                0.7
              );
          }

          .spin-refresh {
            animation:
              refreshSpin
              0.8s
              cubic-bezier(
                0.4,
                0,
                0.2,
                1
              )
              infinite;
          }

          @keyframes refreshSpin {
            from {
              transform: rotate(0deg);
            }

            to {
              transform: rotate(360deg);
            }
          }

          .grad-icon-box {
            background:
              linear-gradient(
                135deg,
                rgba(
                  255,
                  255,
                  255,
                  0.9
                ),
                rgba(
                  255,
                  255,
                  255,
                  0.4
                )
              );

            box-shadow:
              0 4px 10px
              rgba(
                0,
                0,
                0,
                0.03
              );

            border:
              1px solid
              rgba(
                255,
                255,
                255,
                1
              );
          }

          .btn-premium-primary {
            background:
              linear-gradient(
                135deg,
                #6366f1 0%,
                #4f46e5 100%
              );

            border: none;

            color: white;

            box-shadow:
              0 4px 12px
              rgba(
                79,
                70,
                229,
                0.2
              );
          }

          .btn-premium-primary:hover {
            background:
              linear-gradient(
                135deg,
                #4f46e5 0%,
                #4338ca 100%
              );

            color: white;
          }

          .session-user-name {
            max-width: 230px;
            overflow: hidden;
            text-overflow: ellipsis;
          }

          .session-email {
            max-width: 230px;
            overflow: hidden;
            text-overflow: ellipsis;
          }
        `}
      </style>

      {/* =================================================
          HEADER
      ================================================= */}

      <div
        className="glass-panel z-3 sticky-top shadow-sm"
        style={{
          borderBottom:
            "1px solid rgba(0,0,0,0.05)",
        }}
      >
        <div className="container-fluid px-3 py-3">

          {/* TITLE + ACTIONS */}

          <div className="d-flex flex-wrap justify-content-between align-items-center mb-3 gap-2">

            <div className="d-flex align-items-center gap-3">

              <div
                className="grad-icon-box rounded-circle d-flex align-items-center justify-content-center"
                style={{
                  width: 42,
                  height: 42,
                }}
              >
                <i
                  className="bi bi-shield-lock-fill fs-5"
                  style={{
                    background:
                      "-webkit-linear-gradient(#6366f1, #8b5cf6)",
                    WebkitBackgroundClip:
                      "text",
                    WebkitTextFillColor:
                      "transparent",
                  }}
                />
              </div>

              <div>
                <h5
                  className="fw-bolder mb-0 text-dark"
                  style={{
                    letterSpacing:
                      "-0.5px",
                  }}
                >
                  User Sessions
                </h5>

                <div
                  className="text-muted fw-medium"
                  style={{
                    fontSize:
                      "0.75rem",
                  }}
                >
                  Auto-sync:{" "}
                  <strong>60s</strong>

                  {" • "}

                  Last updated:{" "}
                  <strong>
                    {lastRefreshText}
                  </strong>
                </div>
              </div>
            </div>

            <div className="d-flex gap-2">

              {/* MANUAL REFRESH */}

              <button
                type="button"
                className="btn btn-sm btn-light rounded-pill px-3 fw-semibold border shadow-sm"
                onClick={manualRefresh}
                disabled={refreshing}
              >
                <i
                  className={`bi ${
                    refreshing
                      ? "bi-arrow-repeat spin-refresh"
                      : "bi-arrow-clockwise"
                  } me-1 text-primary`}
                />

                {refreshing
                  ? "Refreshing..."
                  : "Refresh"}
              </button>

              {/* EXPORT */}

              <button
                type="button"
                className="btn btn-sm btn-premium-primary rounded-pill px-3 fw-semibold"
                onClick={exportCsv}
                disabled={exporting}
              >
                <i className="bi bi-cloud-download me-1" />

                {exporting
                  ? "Exporting..."
                  : "Export Data"}
              </button>
            </div>
          </div>

          {/* =================================================
              STATS
          ================================================= */}

          <div className="row g-2 mb-3">

            <StatCard
              title="Total"
              value={
                stats?.totalSessions ??
                0
              }
              icon="bi-grid-fill"
              color="#64748b"
            />

            <StatCard
              title="Active Now"
              value={
                stats?.activeSessions ??
                0
              }
              icon="bi-record-circle-fill"
              color="#10b981"
            />

            <StatCard
              title="Logged Out"
              value={
                stats?.loggedOutSessions ??
                0
              }
              icon="bi-door-closed-fill"
              color="#f59e0b"
            />

            <StatCard
              title="Today's Logins"
              value={
                stats?.todaySessions ??
                0
              }
              icon="bi-calendar2-check-fill"
              color="#3b82f6"
            />

            <StatCard
              title="Avg Duration"
              value={formatSeconds(
                stats?.averageDurationSeconds ??
                  0
              )}
              icon="bi-stopwatch-fill"
              color="#8b5cf6"
            />
          </div>

          {/* =================================================
              FILTERS
          ================================================= */}

          <div className="row g-2 align-items-center">

            {/* SEARCH */}

            <div className="col-12 col-md-3">

              <div className="input-group input-group-sm shadow-sm rounded-pill overflow-hidden">

                <span className="input-group-text bg-white border-0 ps-3">
                  <i className="bi bi-search text-muted" />
                </span>

                <input
                  type="text"
                  className="form-control border-0 glass-input px-2"
                  placeholder="Search name, username, email, ID, IP..."
                  value={search}
                  onChange={(e) => {
                    setSearch(
                      e.target.value
                    );

                    setPage(1);
                  }}
                />
              </div>
            </div>

            {/* STATUS */}

            <div className="col-6 col-md-2">

              <select
                className="form-select form-select-sm glass-input rounded-pill shadow-sm px-3"
                value={status}
                onChange={(e) => {
                  setStatus(
                    e.target.value
                  );

                  setPage(1);
                }}
              >
                <option value="">
                  All Status
                </option>

                <option value="active">
                  🟢 Active
                </option>

                <option value="loggedout">
                  ⚪ Logged Out
                </option>
              </select>
            </div>

            {/* FROM */}

            <div className="col-6 col-md-2">

              <input
                type="date"
                className="form-control form-control-sm glass-input rounded-pill shadow-sm px-3 text-muted"
                value={from}
                onChange={(e) => {
                  setFrom(
                    e.target.value
                  );

                  setPage(1);
                }}
              />
            </div>

            {/* TO */}

            <div className="col-6 col-md-2">

              <input
                type="date"
                className="form-control form-control-sm glass-input rounded-pill shadow-sm px-3 text-muted"
                value={to}
                onChange={(e) => {
                  setTo(
                    e.target.value
                  );

                  setPage(1);
                }}
              />
            </div>

            {/* CLEAR */}

            <div className="col-6 col-md-1">

              <button
                type="button"
                className="btn btn-sm btn-white border rounded-pill shadow-sm w-100 fw-medium text-muted"
                onClick={resetFilters}
              >
                Clear
              </button>
            </div>

            {/* PAGE SIZE */}

            <div className="col-12 col-md-2 text-end ms-auto">

              <div className="d-flex align-items-center justify-content-end gap-2 bg-white rounded-pill shadow-sm border px-2 py-1">

                <span
                  className="small text-muted fw-medium ms-1"
                  style={{
                    fontSize:
                      "0.75rem",
                  }}
                >
                  Rows:
                </span>

                <select
                  className="form-select form-select-sm border-0 bg-transparent fw-bold"
                  style={{
                    width: "60px",
                    boxShadow: "none",
                    cursor: "pointer",
                  }}
                  value={pageSize}
                  onChange={(e) => {
                    setPageSize(
                      Number(
                        e.target.value
                      )
                    );

                    setPage(1);
                  }}
                >
                  <option value={25}>
                    25
                  </option>

                  <option value={50}>
                    50
                  </option>

                  <option value={100}>
                    100
                  </option>
                </select>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* =================================================
          TABLE
      ================================================= */}

      <div className="container-fluid px-3 py-4">

        <div className="glass-panel rounded-4 overflow-hidden mb-4">

          <div className="table-responsive premium-scrollbar">

            <table className="table table-hover table-transparent mb-0 w-100">

              <thead className="table-glass-header">

                <tr>

                  <th className="ps-4 py-3 border-0">
                    User Details
                  </th>

                  <th className="py-3 border-0">
                    Login Time
                  </th>

                  <th className="py-3 border-0">
                    Last Activity
                  </th>

                  <th className="py-3 border-0">
                    Logout Time
                  </th>

                  <th className="py-3 border-0">
                    Duration
                  </th>

                  <th className="py-3 border-0">
                    Status
                  </th>

                  <th className="py-3 border-0">
                    IP Address
                  </th>

                  <th className="text-center pe-4 py-3 border-0">
                    Action
                  </th>

                </tr>

              </thead>

              <tbody>

                {/* LOADING */}

                {loading &&
                sessions.length === 0 ? (
                  <tr>
                    <td
                      colSpan="8"
                      className="text-center py-5"
                    >
                      <div className="spinner-border spinner-border-sm text-primary mb-2" />

                      <div className="text-muted fw-medium small">
                        Loading session data...
                      </div>
                    </td>
                  </tr>
                ) : sessions.length === 0 ? (

                  /* EMPTY */

                  <tr>
                    <td
                      colSpan="8"
                      className="text-center py-5"
                    >

                      <div
                        className="grad-icon-box rounded-circle mx-auto d-flex align-items-center justify-content-center mb-3"
                        style={{
                          width: 60,
                          height: 60,
                        }}
                      >
                        <i className="bi bi-inbox fs-3 text-muted" />
                      </div>

                      <div className="text-muted fw-medium">
                        No sessions found in the selected range.
                      </div>

                    </td>
                  </tr>

                ) : (

                  /* DATA */

                  sessions.map(
                    (session) => (
                      <tr
                        key={
                          session.sessionId
                        }
                        style={{
                          cursor:
                            "pointer",
                        }}
                        onClick={() =>
                          viewDetails(
                            session.sessionId
                          )
                        }
                      >

                        {/* USER DETAILS */}

                        <td className="ps-4">

                          <div
                            className="fw-bold text-dark lh-sm session-user-name"
                            title={getFullName(
                              session
                            )}
                          >
                            {getFullName(
                              session
                            )}
                          </div>

                          <div
                            className="text-muted fw-medium"
                            style={{
                              fontSize:
                                "0.7rem",
                            }}
                          >
                            @
                            {session.username ||
                              "--"}
                          </div>

                          <div
                            className="text-muted session-email"
                            style={{
                              fontSize:
                                "0.68rem",
                            }}
                            title={
                              session.email ||
                              ""
                            }
                          >
                            {session.email ||
                              `ID: ${session.userId}`}
                          </div>

                        </td>

                        {/* LOGIN */}

                        <td className="text-muted fw-medium">
                          {formatDate(
                            session.loginAtUtc
                          )}
                        </td>

                        {/* LAST ACTIVITY */}

                        <td className="text-muted fw-medium">
                          {formatDate(
                            session.lastActivityUtc
                          )}
                        </td>

                        {/* LOGOUT */}

                        <td className="text-muted fw-medium">
                          {formatDate(
                            session.logoutAtUtc
                          )}
                        </td>

                        {/* DURATION */}

                        <td>
                          <span className="badge bg-white text-dark border shadow-sm rounded-pill px-2 py-1">
                            {session.duration ||
                              formatSeconds(
                                session.durationSeconds
                              )}
                          </span>
                        </td>

                        {/* STATUS */}

                        <td>

                          {session.isActive ? (

                            <span
                              className="badge rounded-pill px-2 py-1"
                              style={{
                                background:
                                  "rgba(16, 185, 129, 0.1)",
                                color:
                                  "#059669",
                                border:
                                  "1px solid rgba(16, 185, 129, 0.2)",
                              }}
                            >

                              <i
                                className="bi bi-circle-fill me-1"
                                style={{
                                  fontSize:
                                    "5px",
                                  verticalAlign:
                                    "middle",
                                }}
                              />

                              Active

                            </span>

                          ) : (

                            <span
                              className="badge rounded-pill px-2 py-1"
                              style={{
                                background:
                                  "rgba(100, 116, 139, 0.1)",
                                color:
                                  "#475569",
                                border:
                                  "1px solid rgba(100, 116, 139, 0.2)",
                              }}
                            >
                              Logged Out
                            </span>

                          )}

                        </td>

                        {/* IP */}

                        <td>

                          <code className="text-muted bg-white border px-2 py-1 rounded-3 shadow-sm">
                            {session.ipAddress ||
                              "--"}
                          </code>

                        </td>

                        {/* ACTION */}

                        <td className="text-center pe-4">

                          <button
                            type="button"
                            className="btn btn-sm btn-white border rounded-circle shadow-sm text-primary"
                            style={{
                              width: 32,
                              height: 32,
                            }}
                            onClick={(e) => {
                              e.stopPropagation();

                              viewDetails(
                                session.sessionId
                              );
                            }}
                            title="View Details"
                          >
                            <i className="bi bi-arrow-right-short fs-5 lh-1" />
                          </button>

                        </td>

                      </tr>
                    )
                  )
                )}

              </tbody>

            </table>

          </div>

          {/* =================================================
              PAGINATION
          ================================================= */}

          <div className="bg-white bg-opacity-50 border-top px-4 py-3">

            <div className="d-flex flex-wrap justify-content-between align-items-center">

              <div
                className="small text-muted fw-medium"
                style={{
                  fontSize:
                    "0.8rem",
                }}
              >
                Showing{" "}
                <strong className="text-dark">
                  {showingFrom}
                </strong>

                {" - "}

                <strong className="text-dark">
                  {showingTo}
                </strong>

                {" of "}

                <strong className="text-dark">
                  {total}
                </strong>

                {" records"}
              </div>

              <div className="d-flex align-items-center gap-1 mt-2 mt-sm-0">

                {/* PREVIOUS */}

                <button
                  type="button"
                  className="btn btn-sm btn-white border rounded shadow-sm py-1 px-2"
                  disabled={page <= 1}
                  onClick={() =>
                    setPage(
                      (p) =>
                        Math.max(
                          p - 1,
                          1
                        )
                    )
                  }
                >
                  <i className="bi bi-chevron-left" />
                </button>

                {/* PAGE NUMBERS */}

                {paginationItems.map(
                  (item, index) => {

                    if (
                      item === "..."
                    ) {
                      return (
                        <span
                          key={`dots-${index}`}
                          className="px-1 text-muted small fw-bold"
                        >
                          ...
                        </span>
                      );
                    }

                    return (
                      <button
                        type="button"
                        key={item}
                        className={`btn btn-sm py-1 px-2 shadow-sm rounded fw-bold ${
                          page === item
                            ? "btn-premium-primary"
                            : "btn-white border text-muted"
                        }`}
                        style={{
                          minWidth:
                            "32px",
                        }}
                        onClick={() =>
                          setPage(item)
                        }
                      >
                        {item}
                      </button>
                    );
                  }
                )}

                {/* NEXT */}

                <button
                  type="button"
                  className="btn btn-sm btn-white border rounded shadow-sm py-1 px-2"
                  disabled={
                    page >=
                    totalPages
                  }
                  onClick={() =>
                    setPage(
                      (p) =>
                        Math.min(
                          p + 1,
                          totalPages
                        )
                    )
                  }
                >
                  <i className="bi bi-chevron-right" />
                </button>

              </div>
            </div>
          </div>

        </div>
      </div>

      {/* =================================================
          SESSION DETAILS MODAL
      ================================================= */}

      {selectedSession && (

        <div
          className="modal fade show d-block"
          style={{
            backgroundColor:
              "rgba(15, 23, 42, 0.4)",
            backdropFilter:
              "blur(8px)",
            zIndex: 1050,
          }}
        >

          <div className="modal-dialog modal-lg modal-dialog-centered">

            <div className="modal-content glass-panel border-0 rounded-4 overflow-hidden shadow-lg">

              {/* MODAL HEADER */}

              <div className="modal-header bg-white bg-opacity-75 py-3 px-4 border-bottom border-light">

                <div className="d-flex align-items-center gap-3">

                  <div
                    className="grad-icon-box rounded-circle d-flex align-items-center justify-content-center"
                    style={{
                      width: 40,
                      height: 40,
                    }}
                  >
                    <i className="bi bi-person-bounding-box text-primary fs-5" />
                  </div>

                  <div>

                    <h5 className="modal-title fw-bold lh-1 mb-1 text-dark">
                      Session Inspector
                    </h5>

                    <small
                      className="text-muted fw-medium"
                      style={{
                        fontSize:
                          "0.8rem",
                      }}
                    >

                      {getFullName(
                        selectedSession
                      )}

                      {" • @"}

                      {selectedSession.username ||
                        "--"}

                      {" • ID: "}

                      {selectedSession.userId ||
                        "--"}

                    </small>

                  </div>
                </div>

                <button
                  type="button"
                  className="btn-close rounded-circle bg-light p-2 shadow-sm"
                  onClick={() =>
                    setSelectedSession(
                      null
                    )
                  }
                />

              </div>

              {/* MODAL BODY */}

              <div className="modal-body p-4 bg-white bg-opacity-50">

                {detailsLoading ? (

                  <div className="text-center py-5">

                    <div className="spinner-border text-primary" />

                    <div className="small text-muted mt-2">
                      Loading session details...
                    </div>

                  </div>

                ) : (

                  <div className="row g-3">

                    {/* FULL NAME */}

                    <Detail
                      label="Full Name"
                      value={getFullName(
                        selectedSession
                      )}
                    />

                    {/* USERNAME */}

                    <Detail
                      label="Username"
                      value={
                        selectedSession.username
                      }
                    />

                    {/* EMAIL */}

                    <Detail
                      label="Email"
                      value={
                        selectedSession.email
                      }
                    />

                    {/* USER ID */}

                    <Detail
                      label="User ID"
                      value={
                        selectedSession.userId
                      }
                    />

                    {/* SESSION ID */}

                    <Detail
                      label="Session ID"
                      value={
                        <code className="text-primary bg-white px-2 py-1 rounded border d-inline-block text-break">
                          {
                            selectedSession.sessionId
                          }
                        </code>
                      }
                    />

                    {/* IP */}

                    <Detail
                      label="IP Address"
                      value={
                        <code className="text-dark bg-white px-2 py-1 rounded border">
                          {
                            selectedSession.ipAddress ||
                            "--"
                          }
                        </code>
                      }
                    />

                    {/* LOGIN */}

                    <Detail
                      label="Login Time"
                      value={formatDate(
                        selectedSession.loginAtUtc
                      )}
                    />

                    {/* LAST ACTIVITY */}

                    <Detail
                      label="Last Activity"
                      value={formatDate(
                        selectedSession.lastActivityUtc
                      )}
                    />

                    {/* HEARTBEAT */}

                    <Detail
                      label="Last Heartbeat"
                      value={formatDate(
                        selectedSession.lastHeartbeatUtc
                      )}
                    />

                    {/* LOGOUT */}

                    <Detail
                      label="Logout Time"
                      value={formatDate(
                        selectedSession.logoutAtUtc
                      )}
                    />

                    {/* DURATION */}

                    <Detail
                      label="Session Duration"
                      value={
                        selectedSession.duration ||
                        formatSeconds(
                          selectedSession.durationSeconds
                        )
                      }
                    />

                    {/* STATUS */}

                    <Detail
                      label="Session Status"
                      value={
                        selectedSession.isActive
                          ? "🟢 Active Session"
                          : "⚪ Logged Out"
                      }
                    />

                    {/* USER AGENT */}

                    <div className="col-12">

                      <div className="bg-white bg-opacity-75 border rounded-4 p-3 shadow-sm">

                        <div
                          className="text-muted fw-bold mb-1"
                          style={{
                            fontSize:
                              "0.7rem",
                            textTransform:
                              "uppercase",
                            letterSpacing:
                              "0.5px",
                          }}
                        >
                          Client Device / User Agent
                        </div>

                        <div className="small text-break text-dark fw-medium font-monospace">
                          {selectedSession.userAgent ||
                            "Unknown Device"}
                        </div>

                      </div>

                    </div>

                  </div>
                )}

              </div>

            </div>

          </div>

        </div>
      )}

    </div>
  );
}

// =====================================================
// PREMIUM STAT CARD
// =====================================================

function StatCard({
  title,
  value,
  icon,
  color,
}) {
  return (
    <div className="col">

      <div className="glass-panel rounded-pill px-3 py-2 d-flex align-items-center justify-content-between h-100">

        <div>

          <div
            className="text-muted fw-bold lh-1 mb-1"
            style={{
              fontSize:
                "0.65rem",
              textTransform:
                "uppercase",
              letterSpacing:
                "0.5px",
            }}
          >
            {title}
          </div>

          <div
            className="fw-bolder text-dark lh-1"
            style={{
              fontSize:
                "1.1rem",
            }}
          >
            {value}
          </div>

        </div>

        <div
          className="grad-icon-box rounded-circle d-flex align-items-center justify-content-center"
          style={{
            width: 32,
            height: 32,
          }}
        >
          <i
            className={`bi ${icon}`}
            style={{
              color,
              fontSize:
                "1rem",
            }}
          />
        </div>

      </div>

    </div>
  );
}

// =====================================================
// DETAIL COMPONENT
// =====================================================

function Detail({
  label,
  value,
}) {
  return (
    <div className="col-md-6">

      <div className="bg-white bg-opacity-75 border rounded-4 p-3 h-100 shadow-sm">

        <div
          className="text-muted fw-bold lh-1 mb-2"
          style={{
            fontSize:
              "0.7rem",
            textTransform:
              "uppercase",
            letterSpacing:
              "0.5px",
          }}
        >
          {label}
        </div>

        <div
          className="text-dark fw-bold text-break lh-sm"
          style={{
            fontSize:
              "0.95rem",
          }}
        >
          {value || "--"}
        </div>

      </div>

    </div>
  );
}

// =====================================================
// FORMAT SECONDS
// =====================================================

function formatSeconds(
  seconds
) {
  if (!seconds) {
    return "0s";
  }

  const s = Number(seconds);

  if (s >= 3600) {
    return `${Math.floor(
      s / 3600
    )}h ${Math.floor(
      (s % 3600) / 60
    )}m`;
  }

  if (s >= 60) {
    return `${Math.floor(
      s / 60
    )}m ${Math.floor(
      s % 60
    )}s`;
  }

  return `${Math.floor(s)}s`;
}