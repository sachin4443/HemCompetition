using System.Data.Common;
using System.Diagnostics;
using HemCompetition.API.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HemCompetition.API.Diagnostics
{
    public class SqlMetricsInterceptor : DbCommandInterceptor
    {
        private readonly ISqlDiagnosticsService _service;

        public SqlMetricsInterceptor(
            ISqlDiagnosticsService service)
        {
            _service = service;
        }


        public override async ValueTask<DbDataReader>
            ReaderExecutedAsync(
                DbCommand command,
                CommandExecutedEventData eventData,
                DbDataReader result,
                CancellationToken cancellationToken = default)
        {
            Record(command, eventData);

            return await base.ReaderExecutedAsync(
                command,
                eventData,
                result,
                cancellationToken);
        }


        public override async ValueTask<int>
            NonQueryExecutedAsync(
                DbCommand command,
                CommandExecutedEventData eventData,
                int result,
                CancellationToken cancellationToken = default)
        {
            Record(command, eventData);

            return await base.NonQueryExecutedAsync(
                command,
                eventData,
                result,
                cancellationToken);
        }


        public override async ValueTask<object?>
            ScalarExecutedAsync(
                DbCommand command,
                CommandExecutedEventData eventData,
                object? result,
                CancellationToken cancellationToken = default)
        {
            Record(command, eventData);

            return await base.ScalarExecutedAsync(
                command,
                eventData,
                result,
                cancellationToken);
        }


        private void Record(
            DbCommand command,
            CommandExecutedEventData eventData)
        {
            var duration =
                (long)eventData.Duration.TotalMilliseconds;


            // Don't flood diagnostics with tiny queries.
            if (duration < 100)
                return;


            var trace =
                ApiDiagnosticContext.TraceId;

            var path =
                ApiDiagnosticContext.Path;


            _service.Record(
                duration,
                command.CommandType.ToString(),
                command.CommandText,
                trace,
                path);
        }
    }
}