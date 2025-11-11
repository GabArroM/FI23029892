using Microsoft.EntityFrameworkCore;
using System.Text;

// Asegura stdout flushing "bonito" para ver los mensajes mientras corre
Console.OutputEncoding = Encoding.UTF8;

await using var db = new Bookcontext();

// Asegura existencia de la BD sin recrear migraciones.
await db.Database.EnsureCreatedAsync();

// ¿La BD está vacía?
var isEmpty = !await db.Titles.AnyAsync();

if (isEmpty)
{
    Console.WriteLine("La base de datos está vacía, por lo que será llenada a partir de los datos del archivo CSV.");
    Console.Write("Procesando... ");

    var importer = new Importer(db);
    var csvPath = Path.Combine("data", "books.csv");
    await importer.ImportAsync(csvPath);

    Console.WriteLine("Listo.");
}
else
{
    Console.WriteLine("La base de datos se está leyendo para crear los archivos TSV.");
    Console.Write("Procesando... ");

    var exporter = new Exporter(db);
    var outDir = "data";
    Directory.CreateDirectory(outDir);
    await exporter.ExportPerInitialAsync(outDir);

    Console.WriteLine("Listo.");
}
