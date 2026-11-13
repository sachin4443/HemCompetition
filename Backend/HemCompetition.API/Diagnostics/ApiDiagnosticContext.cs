namespace HemCompetition.API.Diagnostics
{
    public static class ApiDiagnosticContext
    {
        private static readonly AsyncLocal<
            DiagnosticContext?> Current =
            new();

        public static string? TraceId =>
            Current.Value?.TraceId;

        public static string? Path =>
            Current.Value?.Path;


        public static void Set(
            string? traceId,
            string? path)
        {
            Current.Value =
                new DiagnosticContext
                {
                    TraceId = traceId,
                    Path = path
                };
        }


        public static void Clear()
        {
            Current.Value = null;
        }


        private sealed class DiagnosticContext
        {
            public string? TraceId { get; set; }

            public string? Path { get; set; }
        }
    }
}