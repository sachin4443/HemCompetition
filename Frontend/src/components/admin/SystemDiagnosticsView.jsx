import { useMemo } from "react";

export default function SystemDiagnosticsView({
  diagnostics
}) {
  if (!diagnostics) {
    return (
      <div className="card border-0 shadow-sm rounded-4 p-5 text-center">
        <div className="spinner-border text-danger mb-3" />
        <div className="fw-semibold">
          Collecting system diagnostics...
        </div>
        <div className="small text-muted">
          Waiting for telemetry data
        </div>
      </div>
    );
  }


  const {
    totalRequests,
    successfulRequests,
    failedRequests,
    requestsPerSecond,
    averageLatencyMs,
    p50LatencyMs,
    p95LatencyMs,
    p99LatencyMs,
    maxLatencyMs,
    errorRate,
    endpoints = [],
    recentRequests = [],
    slowRequests = [],
    slowSqlQueries = [],
    bottlenecks = [],
    system = {}
  } = diagnostics;


  const statusClass = (status) => {
    if (status >= 500) return "bg-danger";
    if (status >= 400) return "bg-warning text-dark";
    if (status >= 300) return "bg-info text-dark";
    return "bg-success";
  };


  const latencyClass = (value) => {
    if (value >= 2000)
      return "text-danger fw-bold";

    if (value >= 1000)
      return "text-warning fw-bold";

    return "text-success fw-semibold";
  };


  return (
    <div>

      {/* HEADER */}

      <div className="d-flex justify-content-between align-items-center mb-4">

        <div>
          <h4 className="fw-bold mb-1">
            <i className="bi bi-speedometer2 me-2 text-danger" />
            System Diagnostics
          </h4>

          <div className="small text-muted">
            Last 5 minutes — API, SQL, CPU, memory
            and runtime diagnostics
          </div>
        </div>

        <span className="badge bg-danger rounded-pill px-3 py-2">
          LIVE 5s
        </span>

      </div>


      {/* TOP METRICS */}

      <div className="row g-3 mb-4">

        <MetricCard
          title="Requests"
          value={totalRequests}
          subtitle={`${requestsPerSecond.toFixed(2)} req/sec`}
          icon="bi-activity"
        />

        <MetricCard
          title="Average"
          value={`${averageLatencyMs} ms`}
          subtitle="Average latency"
          icon="bi-stopwatch"
        />

        <MetricCard
          title="P95"
          value={`${p95LatencyMs} ms`}
          subtitle="95% requests below"
          icon="bi-speedometer"
          danger={p95LatencyMs >= 1000}
        />

        <MetricCard
          title="P99"
          value={`${p99LatencyMs} ms`}
          subtitle="99% requests below"
          icon="bi-lightning-charge"
          danger={p99LatencyMs >= 2000}
        />

        <MetricCard
          title="Errors"
          value={`${errorRate}%`}
          subtitle={`${failedRequests} failed`}
          icon="bi-exclamation-triangle"
          danger={errorRate >= 5}
        />

      </div>


      {/* BOTTLENECKS */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">

        <div className="card-body p-4">

          <div className="fw-bold mb-3">
            <i className="bi bi-fire me-2 text-danger" />
            Bottleneck Detection
          </div>


          {bottlenecks.length === 0 ? (

            <div className="alert alert-success mb-0 rounded-3">
              <i className="bi bi-check-circle me-2" />
              No major bottleneck detected in the last
              5 minutes.
            </div>

          ) : (

            <div className="d-flex flex-column gap-2">

              {bottlenecks.map(
                (item, index) => (
                  <div
                    key={index}
                    className="alert alert-danger mb-0"
                  >
                    <i className="bi bi-exclamation-octagon me-2" />
                    {item}
                  </div>
                )
              )}

            </div>
          )}

        </div>
      </div>


      {/* SYSTEM */}

      <div className="row g-3 mb-4">

        <HealthCard
          title="CPU / Process"
          value={`${system.processCpuPercent ?? 0}%`}
          subtitle={system.processName}
          icon="bi-cpu"
        />

        <HealthCard
          title="Memory"
          value={`${system.workingSetMb ?? 0} MB`}
          subtitle={`GC Heap ${system.gcHeapMb ?? 0} MB`}
          icon="bi-memory"
        />

        <HealthCard
          title="Threads"
          value={system.threadCount ?? 0}
          subtitle="Process threads"
          icon="bi-diagram-3"
        />

        <HealthCard
          title="Thread Pool"
          value={
            system.threadPoolAvailableWorker ??
            0
          }
          subtitle="Available workers"
          icon="bi-layers"
        />

      </div>


      {/* ENDPOINT PERFORMANCE */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">

        <div className="card-body p-0">

          <div className="p-4 border-bottom">

            <div className="fw-bold">
              <i className="bi bi-bar-chart-fill me-2" />
              Endpoint Performance
            </div>

            <div className="small text-muted">
              Sorted by P95 latency
            </div>

          </div>


          <div className="table-responsive">

            <table className="table table-hover align-middle mb-0">

              <thead className="table-light">

                <tr>
                  <th className="px-4">Method</th>
                  <th>Endpoint</th>
                  <th>Requests</th>
                  <th>Avg</th>
                  <th>P95</th>
                  <th>P99</th>
                  <th>Max</th>
                  <th>Errors</th>
                </tr>

              </thead>


              <tbody>

                {endpoints.map(
                  (endpoint, index) => (

                    <tr key={index}>

                      <td className="px-4">
                        <span className="badge bg-secondary">
                          {endpoint.method}
                        </span>
                      </td>

                      <td className="fw-semibold">
                        {endpoint.path}
                      </td>

                      <td>
                        {endpoint.requests}
                      </td>

                      <td>
                        {endpoint.averageLatencyMs} ms
                      </td>

                      <td className={
                        latencyClass(
                          endpoint.p95LatencyMs
                        )
                      }>
                        {endpoint.p95LatencyMs} ms
                      </td>

                      <td className={
                        latencyClass(
                          endpoint.p99LatencyMs
                        )
                      }>
                        {endpoint.p99LatencyMs} ms
                      </td>

                      <td>
                        {endpoint.maxLatencyMs} ms
                      </td>

                      <td>
                        <span
                          className={
                            endpoint.errorRate > 0
                              ? "badge bg-danger"
                              : "badge bg-success"
                          }
                        >
                          {endpoint.errorRate}%
                        </span>
                      </td>

                    </tr>
                  )
                )}

              </tbody>

            </table>

          </div>

        </div>
      </div>


      {/* SLOW REQUESTS */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">

        <div className="card-body p-0">

          <div className="p-4 border-bottom">

            <div className="fw-bold">
              <i className="bi bi-hourglass-split me-2 text-warning" />
              Slow Requests
            </div>

            <div className="small text-muted">
              Requests taking 1 second or more
            </div>

          </div>


          <div
            style={{
              maxHeight: "420px",
              overflowY: "auto"
            }}
          >

            <table className="table table-hover table-sm align-middle mb-0">

              <thead className="table-light sticky-top">

                <tr>
                  <th className="px-4">Time</th>
                  <th>Method</th>
                  <th>Endpoint</th>
                  <th>Status</th>
                  <th>Duration</th>
                  <th>SQL</th>
                  <th>Trace</th>
                  <th>Reason</th>
                </tr>

              </thead>


              <tbody>

                {slowRequests.map(
                  request => (

                    <tr key={request.id}>

                      <td className="px-4 small">
                        {new Date(
                          request.timestampUtc
                        ).toLocaleTimeString(
                          "en-IN"
                        )}
                      </td>

                      <td>
                        <span className="badge bg-secondary">
                          {request.method}
                        </span>
                      </td>

                      <td className="fw-semibold">
                        {request.path}
                      </td>

                      <td>
                        <span className={
                          `badge ${statusClass(
                            request.statusCode
                          )}`
                        }>
                          {request.statusCode}
                        </span>
                      </td>

                      <td className={
                        latencyClass(
                          request.durationMs
                        )
                      }>
                        {request.durationMs} ms
                      </td>

                      <td>
                        {request.sqlQueryCount}
                        {" "}
                        queries
                        <br />
                        <small>
                          {request.sqlTotalDurationMs} ms
                        </small>
                      </td>

                      <td>
                        <code>
                          {request.traceId
                            ?.slice(0, 12)}
                        </code>
                      </td>

                      <td className="text-danger">
                        {request.errorReason ||
                          request.exceptionType ||
                          "--"}
                      </td>

                    </tr>

                  )
                )}

              </tbody>

            </table>

          </div>

        </div>
      </div>


      {/* SQL */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">

        <div className="card-body p-0">

          <div className="p-4 border-bottom">

            <div className="fw-bold">
              <i className="bi bi-database-fill me-2 text-primary" />
              Slow SQL Queries
            </div>

          </div>


          <div className="table-responsive">

            <table className="table table-hover table-sm mb-0">

              <thead className="table-light">

                <tr>
                  <th className="px-4">Time</th>
                  <th>Duration</th>
                  <th>Endpoint</th>
                  <th>SQL</th>
                  <th>Error</th>
                </tr>

              </thead>

              <tbody>

                {slowSqlQueries.map(
                  (query, index) => (

                    <tr key={index}>

                      <td className="px-4">
                        {new Date(
                          query.timestampUtc
                        ).toLocaleTimeString(
                          "en-IN"
                        )}
                      </td>

                      <td className={
                        latencyClass(
                          query.durationMs
                        )
                      }>
                        {query.durationMs} ms
                      </td>

                      <td>
                        {query.path || "--"}
                      </td>

                      <td
                        style={{
                          minWidth: "500px"
                        }}
                      >
                        <code
                          style={{
                            whiteSpace: "pre-wrap",
                            wordBreak: "break-word"
                          }}
                        >
                          {query.sql}
                        </code>
                      </td>

                      <td className="text-danger">
                        {query.error || "--"}
                      </td>

                    </tr>

                  )
                )}

              </tbody>

            </table>

          </div>

        </div>
      </div>


      {/* ALL REQUESTS */}

      <div className="card border-0 shadow-sm rounded-4">

        <div className="card-body p-0">

          <div className="p-4 border-bottom">

            <div className="fw-bold">
              <i className="bi bi-list-columns-reverse me-2" />
              All Requests — Last 5 Minutes
            </div>

            <div className="small text-muted">
              Every API request captured during the
              rolling 5-minute window
            </div>

          </div>


          <div
            style={{
              maxHeight: "600px",
              overflowY: "auto"
            }}
          >

            <table className="table table-hover table-sm mb-0">

              <thead className="table-light sticky-top">

                <tr>
                  <th className="px-4">Time</th>
                  <th>Method</th>
                  <th>Endpoint</th>
                  <th>Status</th>
                  <th>Duration</th>
                  <th>SQL</th>
                  <th>Reason</th>
                </tr>

              </thead>


              <tbody>

                {[...recentRequests]
                  .reverse()
                  .map(request => (

                    <tr key={request.id}>

                      <td className="px-4">
                        {new Date(
                          request.timestampUtc
                        ).toLocaleTimeString(
                          "en-IN"
                        )}
                      </td>

                      <td>
                        <span className="badge bg-secondary">
                          {request.method}
                        </span>
                      </td>

                      <td className="fw-semibold">
                        {request.path}
                      </td>

                      <td>
                        <span className={
                          `badge ${statusClass(
                            request.statusCode
                          )}`
                        }>
                          {request.statusCode}
                        </span>
                      </td>

                      <td className={
                        latencyClass(
                          request.durationMs
                        )
                      }>
                        {request.durationMs} ms
                      </td>

                      <td>
                        {request.sqlQueryCount}
                      </td>

                      <td className="text-danger">
                        {request.errorReason ||
                          request.exceptionType ||
                          "--"}
                      </td>

                    </tr>

                  ))}

              </tbody>

            </table>

          </div>

        </div>
      </div>

    </div>
  );
}


function MetricCard({
  title,
  value,
  subtitle,
  icon,
  danger
}) {
  return (
    <div className="col-6 col-xl">

      <div className="card border-0 shadow-sm rounded-4 h-100">

        <div className="card-body">

          <div className="small text-muted fw-semibold">
            {title}
          </div>

          <div className={
            `fs-3 fw-bold mt-2 ${
              danger
                ? "text-danger"
                : "text-dark"
            }`
          }>
            {value}
          </div>

          <div className="small text-muted">
            <i className={`bi ${icon} me-1`} />
            {subtitle}
          </div>

        </div>

      </div>

    </div>
  );
}


function HealthCard({
  title,
  value,
  subtitle,
  icon
}) {
  return (
    <div className="col-6 col-xl-3">

      <div className="card border-0 shadow-sm rounded-4">

        <div className="card-body">

          <div className="d-flex justify-content-between">

            <div>
              <div className="small text-muted">
                {title}
              </div>

              <div className="fs-4 fw-bold">
                {value}
              </div>

              <div className="small text-muted">
                {subtitle}
              </div>
            </div>

            <i className={`bi ${icon} fs-3 text-primary`} />

          </div>

        </div>

      </div>

    </div>
  );
}