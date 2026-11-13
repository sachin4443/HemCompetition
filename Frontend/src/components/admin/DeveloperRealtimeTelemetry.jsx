import { useMemo, useState } from "react";

export default function DeveloperRealtimeTelemetry({ metrics }) {
  const [hoveredTraffic, setHoveredTraffic] = useState(null);
  const [hoveredLatency, setHoveredLatency] = useState(null);

  // API returns camelCase JSON
  const history = metrics?.trafficHistory || [];
  const requests = metrics?.recentRequests || [];

  const chartData = useMemo(() => {
    return [...history].slice(-30);
  }, [history]);

  const formatTime = (value) => {
    if (!value) return "--";

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return "--";
    }

    return new Intl.DateTimeFormat("en-IN", {
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
    }).format(date);
  };

  const formatDateTime = (value) => {
    if (!value) return "--";

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
      return "--";
    }

    return new Intl.DateTimeFormat("en-IN", {
      day: "2-digit",
      month: "short",
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
    }).format(date);
  };

  const statusClass = (status) => {
    if (status >= 500) return "danger";
    if (status >= 400) return "warning";
    if (status >= 300) return "info";
    return "success";
  };

  const maxRequests = Math.max(
    ...chartData.map((x) => Number(x.totalHits || 0)),
    1
  );

  const maxLatency = Math.max(
    ...chartData.map((x) => Number(x.averageLatencyMs || 0)),
    1
  );

  const totalHits = Number(metrics?.totalHits || 0);
  const successfulHits = Number(metrics?.successfulHits || 0);
  const failedHits = Number(metrics?.failedHits || 0);
  const averageLatencyMs = Number(metrics?.averageLatencyMs || 0);

  return (
    <div className="card border-0 shadow-sm rounded-4 mb-4">
      <div className="card-body p-4">

        {/* HEADER */}
        <div className="d-flex flex-wrap justify-content-between align-items-center mb-4 gap-2">
          <div>
            <div className="fw-bold fs-5">
              <i className="bi bi-activity me-2 text-success"></i>
              Live API Telemetry
            </div>

            <div className="small text-muted">
              RAM-based realtime API traffic and latency monitoring
            </div>
          </div>

          <span className="badge bg-success-subtle text-success rounded-pill px-3 py-2">
            <span
              className="d-inline-block rounded-circle bg-success me-2"
              style={{
                width: "8px",
                height: "8px",
              }}
            ></span>
            LIVE
          </span>
        </div>

        {/* =====================================================
            SUMMARY
        ===================================================== */}

        <div className="row g-3 mb-4">

          <div className="col-6 col-md-3">
            <div className="border rounded-4 p-3 bg-light h-100">
              <div className="small text-muted fw-semibold">
                TOTAL HITS
              </div>

              <div className="fs-3 fw-bold text-dark">
                {totalHits.toLocaleString("en-IN")}
              </div>

              <div className="small text-muted">
                Last RAM window
              </div>
            </div>
          </div>

          <div className="col-6 col-md-3">
            <div
              className="border rounded-4 p-3 h-100"
              style={{ background: "#e9f7ef" }}
            >
              <div className="small text-success fw-semibold">
                SUCCESS
              </div>

              <div className="fs-3 fw-bold text-success">
                {successfulHits.toLocaleString("en-IN")}
              </div>

              <div className="small text-success">
                {totalHits > 0
                  ? Math.round((successfulHits / totalHits) * 100)
                  : 100}
                % success
              </div>
            </div>
          </div>

          <div className="col-6 col-md-3">
            <div
              className="border rounded-4 p-3 h-100"
              style={{ background: "#fdecec" }}
            >
              <div className="small text-danger fw-semibold">
                FAILED
              </div>

              <div className="fs-3 fw-bold text-danger">
                {failedHits.toLocaleString("en-IN")}
              </div>

              <div className="small text-danger">
                {totalHits > 0
                  ? Math.round((failedHits / totalHits) * 100)
                  : 0}
                % error
              </div>
            </div>
          </div>

          <div className="col-6 col-md-3">
            <div
              className="border rounded-4 p-3 h-100"
              style={{ background: "#eaf2ff" }}
            >
              <div className="small text-primary fw-semibold">
                AVG LATENCY
              </div>

              <div className="fs-3 fw-bold text-primary">
                {averageLatencyMs}
                <span className="fs-6 ms-1">ms</span>
              </div>

              <div className="small text-primary">
                Average execution
              </div>
            </div>
          </div>

        </div>

        {/* =====================================================
            CHARTS
        ===================================================== */}

        <div className="row g-4">

          {/* REQUEST TRAFFIC */}

          <div className="col-12 col-xl-7">
            <div className="border rounded-4 p-3 h-100">

              <div className="d-flex justify-content-between align-items-center mb-3">
                <div>
                  <div className="fw-bold">
                    API Request Traffic
                  </div>

                  <div className="small text-muted">
                    Recent traffic activity
                  </div>
                </div>

                <span className="badge bg-primary-subtle text-primary">
                  {chartData.length} points
                </span>
              </div>

              {chartData.length === 0 ? (
                <div
                  className="d-flex justify-content-center align-items-center text-muted"
                  style={{ height: "280px" }}
                >
                  Waiting for API traffic...
                </div>
              ) : (
                <div
                  className="position-relative"
                  style={{
                    height: "280px",
                    overflowX: "auto",
                    overflowY: "hidden",
                  }}
                >

                  <div
                    className="d-flex align-items-end gap-2 px-2"
                    style={{
                      minWidth: `${Math.max(chartData.length * 42, 100)}px`,
                      height: "240px",
                      paddingTop: "20px",
                    }}
                  >

                    {chartData.map((item, index) => {
                      const value = Number(item.totalHits || 0);

                      const height =
                        value > 0
                          ? Math.max(
                              (value / maxRequests) * 190,
                              10
                            )
                          : 4;

                      const isHovered =
                        hoveredTraffic === index;

                      return (
                        <div
                          key={`${item.timestampUtc}-${index}`}
                          className="position-relative d-flex flex-column align-items-center justify-content-end"
                          style={{
                            width: "32px",
                            height: "220px",
                          }}
                          onMouseEnter={() =>
                            setHoveredTraffic(index)
                          }
                          onMouseLeave={() =>
                            setHoveredTraffic(null)
                          }
                        >

                          {/* TOOLTIP */}

                          {isHovered && (
                            <div
                              className="position-absolute bg-dark text-white rounded-3 shadow px-3 py-2"
                              style={{
                                width: "190px",
                                bottom: `${height + 8}px`,
                                left: "50%",
                                transform:
                                  "translateX(-50%)",
                                zIndex: 100,
                                fontSize: "12px",
                                pointerEvents: "none",
                              }}
                            >
                              <div className="fw-bold mb-1">
                                {formatDateTime(
                                  item.timestampUtc
                                )}
                              </div>

                              <div>
                                Requests:{" "}
                                <strong>
                                  {item.totalHits || 0}
                                </strong>
                              </div>

                              <div>
                                Success:{" "}
                                <strong>
                                  {item.successfulHits || 0}
                                </strong>
                              </div>

                              <div>
                                Failed:{" "}
                                <strong>
                                  {item.failedHits || 0}
                                </strong>
                              </div>

                              <div>
                                Avg Latency:{" "}
                                <strong>
                                  {item.averageLatencyMs || 0}ms
                                </strong>
                              </div>

                              <div>
                                Max Latency:{" "}
                                <strong>
                                  {item.maxLatencyMs || 0}ms
                                </strong>
                              </div>
                            </div>
                          )}

                          {/* BAR */}

                          <div
                            style={{
                              width: "24px",
                              height: `${height}px`,
                              background:
                                "linear-gradient(180deg, #0d6efd, #6ea8fe)",
                              borderRadius:
                                "6px 6px 2px 2px",
                              transition:
                                "height 0.3s ease, transform 0.2s ease",
                              transform: isHovered
                                ? "scaleX(1.12)"
                                : "scaleX(1)",
                              cursor: "pointer",
                            }}
                          />

                          {/* X LABEL */}

                          <div
                            className="text-muted mt-2"
                            style={{
                              fontSize: "9px",
                              whiteSpace: "nowrap",
                            }}
                          >
                            {formatTime(
                              item.timestampUtc
                            )}
                          </div>

                        </div>
                      );
                    })}

                  </div>
                </div>
              )}

              <div className="small text-muted border-top pt-2">
                Hover over a bar to inspect request details.
              </div>

            </div>
          </div>

          {/* LATENCY */}

          <div className="col-12 col-xl-5">

            <div className="border rounded-4 p-3 h-100">

              <div className="d-flex justify-content-between align-items-center mb-3">
                <div>
                  <div className="fw-bold">
                    API Latency
                  </div>

                  <div className="small text-muted">
                    Average execution time
                  </div>
                </div>

                <span className="badge bg-warning-subtle text-warning">
                  ms
                </span>
              </div>

              {chartData.length === 0 ? (
                <div
                  className="d-flex justify-content-center align-items-center text-muted"
                  style={{ height: "280px" }}
                >
                  Waiting for latency data...
                </div>
              ) : (
                <div
                  style={{
                    height: "280px",
                    overflowX: "auto",
                    overflowY: "hidden",
                  }}
                >

                  <div
                    className="d-flex align-items-end gap-2 px-2"
                    style={{
                      minWidth: `${Math.max(chartData.length * 42, 100)}px`,
                      height: "240px",
                    }}
                  >

                    {chartData.map((item, index) => {
                      const value = Number(
                        item.averageLatencyMs || 0
                      );

                      const height =
                        value > 0
                          ? Math.max(
                              (value / maxLatency) * 190,
                              10
                            )
                          : 4;

                      const isHovered =
                        hoveredLatency === index;

                      return (
                        <div
                          key={`latency-${item.timestampUtc}-${index}`}
                          className="position-relative d-flex flex-column align-items-center justify-content-end"
                          style={{
                            width: "32px",
                            height: "220px",
                          }}
                          onMouseEnter={() =>
                            setHoveredLatency(index)
                          }
                          onMouseLeave={() =>
                            setHoveredLatency(null)
                          }
                        >

                          {isHovered && (
                            <div
                              className="position-absolute bg-dark text-white rounded-3 shadow px-3 py-2"
                              style={{
                                width: "180px",
                                bottom: `${height + 8}px`,
                                left: "50%",
                                transform:
                                  "translateX(-50%)",
                                zIndex: 100,
                                fontSize: "12px",
                                pointerEvents: "none",
                              }}
                            >
                              <div className="fw-bold mb-1">
                                {formatDateTime(
                                  item.timestampUtc
                                )}
                              </div>

                              <div>
                                Requests:{" "}
                                <strong>
                                  {item.totalHits || 0}
                                </strong>
                              </div>

                              <div>
                                Average:{" "}
                                <strong>
                                  {item.averageLatencyMs || 0}ms
                                </strong>
                              </div>

                              <div>
                                Maximum:{" "}
                                <strong>
                                  {item.maxLatencyMs || 0}ms
                                </strong>
                              </div>
                            </div>
                          )}

                          <div
                            style={{
                              width: "24px",
                              height: `${height}px`,
                              background:
                                "linear-gradient(180deg, #ffc107, #ffda6a)",
                              borderRadius:
                                "6px 6px 2px 2px",
                              transition:
                                "height 0.3s ease, transform 0.2s ease",
                              transform: isHovered
                                ? "scaleX(1.12)"
                                : "scaleX(1)",
                              cursor: "pointer",
                            }}
                          />

                          <div
                            className="text-muted mt-2"
                            style={{
                              fontSize: "9px",
                              whiteSpace: "nowrap",
                            }}
                          >
                            {formatTime(
                              item.timestampUtc
                            )}
                          </div>

                        </div>
                      );
                    })}

                  </div>
                </div>
              )}

              <div className="small text-muted border-top pt-2">
                Higher bars = slower API execution.
                Hover for details.
              </div>

            </div>
          </div>
        </div>

        {/* =====================================================
            LIVE REQUEST STREAM
        ===================================================== */}

        <div className="border rounded-4 mt-4">

          <div className="p-3 border-bottom d-flex justify-content-between align-items-center">

            <div>
              <div className="fw-bold">
                <i className="bi bi-broadcast me-2 text-success"></i>
                Live API Request Stream
              </div>

              <div className="small text-muted">
                Last {requests.length} requests kept temporarily in server RAM
              </div>
            </div>

            <span className="badge bg-dark">
              {requests.length}
            </span>

          </div>

          <div
            style={{
              maxHeight: "360px",
              overflowY: "auto",
            }}
          >

            <div className="table-responsive">

              <table className="table table-hover table-sm mb-0 align-middle">

                <thead className="table-light sticky-top">

                  <tr>
                    <th className="px-3">Time</th>
                    <th>Method</th>
                    <th>Endpoint</th>
                    <th>Status</th>
                    <th>Latency</th>
                  </tr>

                </thead>

                <tbody className="small">

                  {requests.length === 0 ? (

                    <tr>
                      <td
                        colSpan="5"
                        className="text-center text-muted py-4"
                      >
                        Waiting for API requests...
                      </td>
                    </tr>

                  ) : (

                    requests.map((request) => {

                      const tone = statusClass(
                        request.statusCode
                      );

                      return (
                        <tr key={request.id}>

                          <td className="px-3 text-muted">
                            {formatTime(
                              request.timestampUtc
                            )}
                          </td>

                          <td>
                            <span className="badge bg-secondary">
                              {request.method}
                            </span>
                          </td>

                          <td
                            className="fw-semibold text-dark"
                            style={{
                              maxWidth: "420px",
                              wordBreak: "break-all",
                            }}
                          >
                            {request.path}
                          </td>

                          <td>
                            <span
                              className={`badge ${
                                tone === "danger"
                                  ? "bg-danger"
                                  : tone === "warning"
                                  ? "bg-warning text-dark"
                                  : tone === "info"
                                  ? "bg-info text-dark"
                                  : "bg-success"
                              }`}
                            >
                              {request.statusCode}
                            </span>
                          </td>

                          <td>
                            <span
                              className={
                                request.durationMs >= 500
                                  ? "text-danger fw-bold"
                                  : request.durationMs >= 200
                                  ? "text-warning fw-bold"
                                  : "text-success fw-semibold"
                              }
                            >
                              {request.durationMs}ms
                            </span>
                          </td>

                        </tr>
                      );
                    })
                  )}

                </tbody>

              </table>

            </div>

          </div>
        </div>

      </div>
    </div>
  );
}