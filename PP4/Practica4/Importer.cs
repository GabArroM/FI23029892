using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

public class Importer
{
    private readonly Bookcontext _db;

    public Importer(Bookcontext db) => _db = db;

    public async Task ImportAsync(string csvPath)
    {
        if (!File.Exists(csvPath))
            throw new FileNotFoundException($"No se encontró el archivo {csvPath}");

        // Configuración de CsvHelper: encabezado, delimitador coma, recorte de espacios, cultura invariante
        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            TrimOptions = TrimOptions.Trim,
            BadDataFound = null,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, cfg);

        // Lee encabezado: Author,Title,Tags
        await csv.ReadAsync();
        csv.ReadHeader();

        // Cachés en memoria para evitar consultas repetidas
        // (clave = nombre; valor = Id)
        var authorCache = await _db.Authors.AsNoTracking()
            .ToDictionaryAsync(a => a.AuthorName, a => a.AuthorId);

        var tagCache = await _db.Tags.AsNoTracking()
            .ToDictionaryAsync(t => t.TagName, t => t.TagId);

        // Para detectar títulos existentes (AuthorId + TitleName)
        var existingTitleKeys = await _db.Titles
            .Select(t => new { t.AuthorId, t.TitleName })
            .ToListAsync();
        var titleKeySet = new HashSet<(int AuthorId, string TitleName)>(
            existingTitleKeys.Select(e => (e.AuthorId, e.TitleName)));

        var newAuthors = new List<Author>();
        var newTags = new List<Tag>();
        var newTitles = new List<Title>();
        var newTitleTags = new List<TitleTag>();

        // Carga por lotes
        while (await csv.ReadAsync())
        {
            var authorName = (csv.GetField("Author") ?? string.Empty).Trim();
            var titleName  = (csv.GetField("Title")  ?? string.Empty).Trim();
            var tagsField  = (csv.GetField("Tags")   ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(authorName) || string.IsNullOrWhiteSpace(titleName))
                continue;

            // Autor (find or create en caché; se materializa luego)
            if (!authorCache.TryGetValue(authorName, out var authorId))
            {
                var a = new Author { AuthorName = authorName };
                newAuthors.Add(a);
                // ID real se asignará tras SaveChanges; marcamos con 0 todavía
                // (lo resolveremos más adelante releyendo de BD).
                authorCache[authorName] = 0;
            }

            // Tags (split por '|')
            var tagNames = tagsField.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var tagName in tagNames)
            {
                if (!tagCache.ContainsKey(tagName))
                {
                    var t = new Tag { TagName = tagName };
                    newTags.Add(t);
                    tagCache[tagName] = 0; // placeholder; resolvemos luego
                }
            }
        }

        // Persistimos autores y tags primero para tener IDs reales
        if (newAuthors.Count > 0)
        {
            _db.Authors.AddRange(newAuthors);
            await _db.SaveChangesAsync();

            foreach (var a in newAuthors)
                authorCache[a.AuthorName] = a.AuthorId;
        }

        if (newTags.Count > 0)
        {
            _db.Tags.AddRange(newTags);
            await _db.SaveChangesAsync();

            foreach (var t in newTags)
                tagCache[t.TagName] = t.TagId;
        }

        // Segunda pasada: títulos y titletags (ya con IDs reales en caches)
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        reader.DiscardBufferedData();
        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            var authorName = (csv.GetField("Author") ?? string.Empty).Trim();
            var titleName  = (csv.GetField("Title")  ?? string.Empty).Trim();
            var tagsField  = (csv.GetField("Tags")   ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(authorName) || string.IsNullOrWhiteSpace(titleName))
                continue;

            if (!authorCache.TryGetValue(authorName, out var authorId) || authorId <= 0)
                continue; // por seguridad

            // Title único por (AuthorId, TitleName)
            if (!titleKeySet.Contains((authorId, titleName)))
            {
                var title = new Title
                {
                    AuthorId = authorId,
                    TitleName = titleName
                };
                newTitles.Add(title);
                titleKeySet.Add((authorId, titleName));
            }
        }

        if (newTitles.Count > 0)
        {
            _db.Titles.AddRange(newTitles);
            await _db.SaveChangesAsync();
        }

        // Mapa para TitleId (ya con títulos insertados)
        var titleIdMap = await _db.Titles
            .ToDictionaryAsync(t => (t.AuthorId, t.TitleName), t => t.TitleId);

        // Tercera pasada: relaciones TitleTag
        reader.BaseStream.Seek(0, SeekOrigin.Begin);
        reader.DiscardBufferedData();
        await csv.ReadAsync();
        csv.ReadHeader();

        // Cargar relaciones existentes para no duplicar
        var existingTT = await _db.TitleTags
            .Select(tt => new { tt.TitleId, tt.TagId })
            .ToListAsync();
        var ttSet = new HashSet<(int TitleId, int TagId)>(
            existingTT.Select(x => (x.TitleId, x.TagId)));

        while (await csv.ReadAsync())
        {
            var authorName = (csv.GetField("Author") ?? string.Empty).Trim();
            var titleName  = (csv.GetField("Title")  ?? string.Empty).Trim();
            var tagsField  = (csv.GetField("Tags")   ?? string.Empty).Trim();

            if (!authorCache.TryGetValue(authorName, out var authorId)) continue;
            if (!titleIdMap.TryGetValue((authorId, titleName), out var titleId)) continue;

            var tagNames = tagsField.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var tagName in tagNames)
            {
                if (!tagCache.TryGetValue(tagName, out var tagId) || tagId <= 0)
                    continue;

                var pair = (titleId, tagId);
                if (!ttSet.Contains(pair))
                {
                    ttSet.Add(pair);
                    newTitleTags.Add(new TitleTag { TitleId = titleId, TagId = tagId });
                }
            }
        }

        if (newTitleTags.Count > 0)
        {
            _db.TitleTags.AddRange(newTitleTags);
            await _db.SaveChangesAsync();
        }
    }
}
