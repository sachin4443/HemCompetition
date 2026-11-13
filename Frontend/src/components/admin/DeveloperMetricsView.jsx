import DeveloperReportDownloads from "./DeveloperReportDownloads";
export default function DeveloperMetricsView({ metrics }) {
  if (!metrics) {
    return (
      <div className="card border-0 shadow-sm rounded-4 p-4 text-center text-muted">
        Developer metrics loading...
      </div>
    );
  }

  // API JSON uses camelCase
  const totalHits = Number(metrics.totalHits || 0);
  const successfulHits = Number(metrics.successfulHits || 0);
  const failedHits = Number(metrics.failedHits || 0);
  const averageLatencyMs = Number(metrics.averageLatencyMs || 0);

  const recentFailures = metrics.recentFailures || [];

  const successRate =
    totalHits > 0
      ? Math.round((successfulHits / totalHits) * 100)
      : 100;

  const failureRate =
    totalHits > 0
      ? Math.round((failedHits / totalHits) * 100)
      : 0;

  const formatTime = (value) => {
    if (!value) return "--";

    return new Date(value).toLocaleTimeString("en-IN", {
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
    });
  };

  return (
    <div className="card border-0 shadow-sm rounded-4 mb-4">
      <div className="card-body p-4">

        {/* =====================================================
            HEADER
        ===================================================== */}

        <div className="d-flex flex-wrap justify-content-between align-items-center mb-4 gap-2">

          <div>
            <div className="fw-bold fs-5 d-flex align-items-center gap-2">
              <i className="bi bi-terminal-fill text-dark"></i>

              Developer API & System Telemetry
            </div>

            <div className="small text-muted">
              Live HTTP traffic monitoring, endpoint failure diagnosis
              & latency tracker
            </div>
          </div>

          <span className="badge bg-dark text-white rounded-pill px-3 py-2">
            <i className="bi bi-cpu-fill me-1 text-success"></i>
            Engine Active
          </span>

        </div>


        {/* =====================================================
            MINI CARDS
        ===================================================== */}

       {/* =====================================================
    REPORT DOWNLOADS
===================================================== */}

<div className="border rounded-4 p-3 mb-4 bg-light">

  <div className="d-flex flex-wrap justify-content-between align-items-center gap-3">

    <div>

      <div className="fw-bold fs-6 d-flex align-items-center gap-2">

        <i className="bi bi-download text-primary"></i>

        Developer Reports

      </div>

      <div className="small text-muted">
        Download competition reports in CSV format
      </div>

    </div>

    <DeveloperReportDownloads />

  </div>

</div>


        {/* =====================================================
            RECENT FAILED REQUESTS
        ===================================================== */}

        <div className="fw-bold mb-2 fs-6">
          Recent Failed Requests & Reasons
        </div>


        <div className="table-responsive rounded-3 border">

          <table className="table table-hover table-sm mb-0 align-middle">

            <thead className="table-light small">

              <tr>

                <th className="py-2 px-3">
                  Time
                </th>

                <th>
                  Method
                </th>

                <th>
                  Path
                </th>

                <th>
                  Status
                </th>

                <th>
                  Latency
                </th>

                <th>
                  Failure Reason
                </th>

              </tr>

            </thead>


            <tbody className="small">

              {recentFailures.length > 0 ? (

                recentFailures.map((fail) => (

                  <tr key={fail.id}>

                    <td className="px-3 text-muted">

                      {formatTime(fail.timestampUtc)}

                    </td>


                    <td>

                      <span className="badge bg-secondary">
                        {fail.method}
                      </span>

                    </td>


                    <td className="fw-semibold text-dark">

                      {fail.path}

                    </td>


                    <td>

                      <span
                        className={`badge ${
                          fail.statusCode >= 500
                            ? "bg-danger"
                            : "bg-warning text-dark"
                        }`}
                      >
                        {fail.statusCode}
                      </span>

                    </td>


                    <td>
                      {fail.durationMs}ms
                    </td>


                    <td className="text-danger fw-semibold">

                      {fail.errorReason}

                    </td>

                  </tr>

                ))

              ) : (

                <tr>

                  <td
                    colSpan="6"
                    className="text-center py-3 text-success fw-semibold"
                  >

                    <i className="bi bi-shield-check me-1"></i>

                    No recent API errors.
                    Everything running smoothly!

                  </td>

                </tr>

              )}

            </tbody>

          </table>

        </div>

      </div>
    </div>
  );
}