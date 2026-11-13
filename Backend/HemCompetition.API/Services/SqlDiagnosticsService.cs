using System.Collections.Concurrent;
using HemCompetition.API.DTOs;

namespace HemCompetition.API.Services
{
    public interface ISqlDiagnosticsService
    {
        void Record(
            long durationMs,
            string commandType,
            string sql,
            string? traceId,
            string? path,
            string? error = null);

        List<SqlDiagnosticDto> GetRecent(
            DateTime sinceUtc,
            int max = 100);

        List<SqlDiagnosticDto> GetForTrace(
            string? traceId);
    }


    public class SqlDiagnosticsService : ISqlDiagnosticsService
    {
        private readonly ConcurrentQueue<SqlDiagnosticDto> _logs = new();

        private const int MaxLogs = 5000;

        public void Record(
            long durationMs,
            string commandType,
            string sql,
            string? traceId,
            string? path,
            string? error = null)
        {
            _logs.Enqueue(new SqlDiagnosticDto
            {
                TimestampUtc = DateTime.UtcNow,
                DurationMs = durationMs,
                CommandType = commandType,
                Sql = sql,
                TraceId = traceId,
                Path = path,
                Error = error
            });

            while (_logs.Count > MaxLogs)
            {
                _logs.TryDequeue(out _);
            }
        }


        public List<SqlDiagnosticDto> GetRecent(
            DateTime sinceUtc,
            int max = 100)
        {
            return _logs
                .Where(x => x.TimestampUtc >= sinceUtc)
                .OrderByDescending(x => x.TimestampUtc)
                .Take(max)
                .ToList();
        }


        public List<SqlDiagnosticDto> GetForTrace(
            string? traceId)
        {
            if (string.IsNullOrWhiteSpace(traceId))
                return new();

            return _logs
                .Where(x => x.TraceId == traceId)
                .OrderByDescending(x => x.TimestampUtc)
                .ToList();
        }
    }
}