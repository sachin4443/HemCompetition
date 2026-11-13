import { useState } from "react";
import api from "../../services/api";

export default function DeveloperReportDownloads() {
  const [loading, setLoading] = useState("");

  const reports = [
    // =====================================================
    // EXISTING REPORTS
    // =====================================================
    {
      key: "quiz",
      label: "Quiz Report",
      icon: "bi-journal-text",
      endpoint: "/reports/quiz/download",
      fileName: "Quiz_Report.csv",
      buttonClass: "btn-outline-primary",
    },
    {
      key: "number",
      label: "Number Game",
      icon: "bi-123",
      endpoint: "/reports/number-game/download",
      fileName: "NumberGame_Report.csv",
      buttonClass: "btn-outline-success",
    },
    {
      key: "word",
      label: "Word Game",
      icon: "bi-fonts",
      endpoint: "/reports/word-game/download",
      fileName: "WordGame_Report.csv",
      buttonClass: "btn-outline-warning",
    },

    // =====================================================
    // NEW REPORTS
    // =====================================================
    {
      key: "gameSummary",
      label: "Game Summary",
      icon: "bi-controller",
      endpoint: "/reports/gamesummary/download",
      fileName: "Game_Summary_Report.csv",
      buttonClass: "btn-outline-info",
    },
    {
      key: "countReport",
      label: "Count Report",
      icon: "bi-123",
      endpoint: "/reports/countReport/download",
      fileName: "Count_Report.csv",
      buttonClass: "btn-outline-secondary",
    },
    {
      key: "detailsReport",
      label: "Details Report",
      icon: "bi-list-ul",
      endpoint: "/reports/detailsReport/download",
      fileName: "Details_Report.csv",
      buttonClass: "btn-outline-dark",
    },
  ];

  const downloadReport = async (report) => {
    if (loading) return;

    try {
      setLoading(report.key);

      const response = await api.get(report.endpoint, {
        responseType: "blob",
      });

      // =====================================================
      // CREATE DOWNLOAD FILE
      // =====================================================

      const blob = new Blob([response.data], {
        type:
          response.headers["content-type"] ||
          "text/csv;charset=utf-8",
      });

      const url = window.URL.createObjectURL(blob);

      const link = document.createElement("a");

      link.href = url;
      link.download = report.fileName;

      document.body.appendChild(link);

      link.click();

      link.remove();

      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error(
        `${report.label} download failed:`,
        error
      );

      // =====================================================
      // BLOB ERROR RESPONSE READ
      // =====================================================

      try {
        if (error?.response?.data instanceof Blob) {
          const text =
            await error.response.data.text();

          console.error(
            "Server error:",
            text
          );
        }
      } catch (blobError) {
        console.error(
          "Unable to read server error:",
          blobError
        );
      }

      alert(
        `${report.label} download failed. Please try again.`
      );
    } finally {
      setLoading("");
    }
  };

  return (
    <div className="d-flex flex-wrap gap-2">

      {reports.map((report) => {

        const isLoading =
          loading === report.key;

        return (
         <button
  key={report.key}
  type="button"
  className={`btn btn-sm ${report.buttonClass} rounded-pill`}
  onClick={() => downloadReport(report)}
  disabled={!!loading}
  title={`Download ${report.label}`}
  style={{
    fontSize: "12px",
    padding: "5px 10px",
    lineHeight: "1.2",
    whiteSpace: "nowrap",
  }}
>
  {isLoading ? (
    <>
      <span
        className="spinner-border spinner-border-sm me-1"
        role="status"
        aria-hidden="true"
        style={{
          width: "11px",
          height: "11px",
          borderWidth: "2px",
        }}
      />

      Downloading...
    </>
  ) : (
    <>
      <i
        className={`bi ${report.icon} me-1`}
        style={{
          fontSize: "11px",
        }}
      />

      {report.label}
    </>
  )}
</button>
        );
      })}

    </div>
  );
}