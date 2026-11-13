using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

namespace HemCompetition.API.Infrastructure;

/// <summary>Reads an uploaded .csv or .xlsx into rows of text, and writes CSV for exports.</summary>
public static class TabularFile
{
    public const long MaxBytes = 5 * 1024 * 1024;
    public const int MaxRows = 20000;

    public sealed class Table
    {
        public required string[] Headers { get; init; }
        public required List<string[]> Rows { get; init; }

        /// <summary>Index of the first header matching any of the names (ignoring case, spaces, underscores).</summary>
        public int Column(params string[] names)
        {
            var wanted = names.Select(Normalize).ToHashSet();
            return Array.FindIndex(Headers, h => wanted.Contains(Normalize(h)));
        }

        public static string Cell(string[] row, int index) => index >= 0 && index < row.Length ? row[index].Trim() : "";
        private static string Normalize(string s) => new(s.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
    }

    public static async Task<Table> ReadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) throw ApiException.BadRequest("Choose a file to import.");
        if (file.Length > MaxBytes) throw new ApiException(413, "The file is larger than 5 MB.", "FILE_TOO_LARGE");
        using var buffer = new MemoryStream();
        await file.CopyToAsync(buffer);
        var bytes = buffer.ToArray();

        // Decide by content, not by file name: .xlsx files are zip archives.
        var isZip = bytes.Length > 4 && bytes[0] == 0x50 && bytes[1] == 0x4B;
        List<string[]> rows;
        try
        {
            rows = isZip ? ReadXlsx(bytes) : ParseCsv(new UTF8Encoding(false).GetString(bytes).TrimStart('﻿'));
        }
        catch (Exception ex) when (ex is InvalidDataException or System.Xml.XmlException or FormatException)
        {
            throw ApiException.BadRequest("The file could not be read. Upload a .csv or .xlsx file.");
        }
        rows = rows.Where(r => r.Any(c => !string.IsNullOrWhiteSpace(c))).ToList();
        if (rows.Count < 2) throw ApiException.BadRequest("The file has no data rows.");
        if (rows.Count - 1 > MaxRows) throw ApiException.BadRequest($"The file has more than {MaxRows} rows.");
        return new Table { Headers = rows[0], Rows = rows.Skip(1).ToList() };
    }

    public static List<string[]> ParseCsv(string text)
    {
        var rows = new List<string[]>();
        var row = new List<string>();
        var field = new StringBuilder();
        var quoted = false;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (quoted)
            {
                if (c == '"' && i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }
                else if (c == '"') quoted = false;
                else field.Append(c);
            }
            else if (c == '"' && field.Length == 0) quoted = true;
            else if (c == ',') { row.Add(field.ToString()); field.Clear(); }
            else if (c == '\n' || c == '\r')
            {
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                row.Add(field.ToString()); field.Clear();
                rows.Add(row.ToArray()); row.Clear();
            }
            else field.Append(c);
        }
        if (field.Length > 0 || row.Count > 0) { row.Add(field.ToString()); rows.Add(row.ToArray()); }
        return rows;
    }

    /// <summary>Minimal reader for the first worksheet of an .xlsx workbook (text and numbers).</summary>
    private static List<string[]> ReadXlsx(byte[] bytes)
    {
        using var zip = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);
        XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var shared = new List<string>();
        var sharedEntry = zip.GetEntry("xl/sharedStrings.xml");
        if (sharedEntry != null)
        {
            using var s = sharedEntry.Open();
            shared = XDocument.Load(s).Root!.Elements(ns + "si")
                .Select(si => string.Concat(si.Descendants(ns + "t").Select(t => t.Value))).ToList();
        }
        var sheetEntry = zip.Entries.Where(e => e.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase))
                             .OrderBy(e => e.FullName.Length).ThenBy(e => e.FullName).FirstOrDefault()
                         ?? throw new InvalidDataException("No worksheet.");
        if (sheetEntry.Length > 50 * 1024 * 1024) throw new InvalidDataException("Worksheet too large.");
        using var sheet = sheetEntry.Open();
        var rows = new List<string[]>();
        foreach (var r in XDocument.Load(sheet).Descendants(ns + "row"))
        {
            var cells = new List<string>();
            foreach (var c in r.Elements(ns + "c"))
            {
                var reference = (string?)c.Attribute("r") ?? "";
                var col = 0;
                foreach (var ch in reference.TakeWhile(char.IsLetter)) col = col * 26 + (char.ToUpperInvariant(ch) - 'A' + 1);
                col = col == 0 ? cells.Count : col - 1;
                if (col > 200) continue;
                while (cells.Count <= col) cells.Add("");
                var type = (string?)c.Attribute("t");
                var v = c.Element(ns + "v")?.Value ?? "";
                cells[col] = type switch
                {
                    "s" => int.TryParse(v, out var i) && i >= 0 && i < shared.Count ? shared[i] : "",
                    "inlineStr" => string.Concat(c.Descendants(ns + "t").Select(t => t.Value)),
                    "b" => v == "1" ? "TRUE" : "FALSE",
                    _ => v
                };
            }
            rows.Add(cells.ToArray());
            if (rows.Count > MaxRows + 1) break;
        }
        return rows;
    }

    public static byte[] ToCsv(IEnumerable<string> headers, IEnumerable<IEnumerable<object?>> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", headers.Select(Escape)));
        foreach (var row in rows) sb.AppendLine(string.Join(",", row.Select(v => Escape(Format(v)))));
        // BOM so Excel opens UTF-8 correctly.
        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }

    private static string Format(object? v) => v switch
    {
        null => "",
        DateTime d => d.ToString("yyyy-MM-dd HH:mm:ss"),
        bool b => b ? "Yes" : "No",
        IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
        _ => v.ToString() ?? ""
    };

    private static string Escape(string value)
    {
        // Neutralise spreadsheet formula injection in exported data.
        if (value.Length > 0 && value[0] is '=' or '+' or '-' or '@' or '\t' or '\r' &&
            !decimal.TryParse(value, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out _))
            value = "'" + value;
        return value.IndexOfAny([',', '"', '\n', '\r']) >= 0 ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
    }
}
