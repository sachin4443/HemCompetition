using HemCompetition.API.DTOs;

namespace HemCompetition.API.Services
{
    public interface IApiMetricsService
    {
        void RecordRequest(string path, string method, int statusCode, long durationMs, string? errorReason = null);
        ApiMetricsSummaryDto GetSummary();
    }
}