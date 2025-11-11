using System.Text;
using Microsoft.EntityFrameworkCore;

public class Exporter
{
    private readonly Bookcontext _db;
    public Exporter(Bookcontext db) => _db = db;

    public async Task ExportPerInitialAsync(string outputDir)
    {
        // Proyección completa via join
        var rows = await (
            from a in _db.Authors
            join t in _db.Titles on a.AuthorId equals t.AuthorId
            join tt in _db.TitleTags on t.TitleId equals tt.TitleId
            join tag in _db.Tags on tt.TagId equals tag.TagId
            select new
            {
                AuthorName = a.AuthorName,
                TitleName = t.TitleName,
                TagName = tag.TagName
            }).ToListAsync();

        if (rows.Count == 0) return;

        // Agrupa por inicial de autor → A.tsv, B.tsv, ...
        var groups = rows.GroupBy(r => Initial(r.AuthorName))
                         .OrderBy(g => g.Key);

        foreach (var g in groups)
        {
            var fileName = Path.Combine(outputDir, $"{g.Key}.tsv");

            // Orden descendente por INICIAL de Autor, luego Título, luego Tag
            var ordered = g
                .OrderByDescending(r => Initial(r.AuthorName))
                .ThenByDescending(r => Initial(r.TitleName))
                .ThenByDescending(r => Initial(r.TagName))
                .ToList();

            await using var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            await using var writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            // Encabezado
            await writer.WriteLineAsync("AuthorName\tTitleName\tTagName");

            // Filas
            foreach (var r in ordered)
            {
                await writer.WriteLineAsync($"{Sanitize(r.AuthorName)}\t{Sanitize(r.TitleName)}\t{Sanitize(r.TagName)}");
            }
        }
    }

    private static string Sanitize(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        return s.Replace('\t', ' ')
                .Replace('\r', ' ')
                .Replace('\n', ' ')
                .Trim();
    }

    private static char Initial(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return '#';
        var ch = s.Trim()[0];
        return char.ToUpperInvariant(ch);
    }
}
