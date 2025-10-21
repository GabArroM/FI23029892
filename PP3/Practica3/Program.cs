using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
// Más información sobre Swagger/OpenAPI en https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
///////////////////////////////////////////////////
// ENDPOINT /
///////////////////////////////////////////////////
app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

///////////////////////////////////////////////////
// ENDPOINT /include
///////////////////////////////////////////////////
app.MapPost("/include/{position:int}", ([FromRoute] int position, [FromQuery] string value, [FromForm] string text, [FromHeader(Name = "xml")] bool xml = false) =>
{
    // Validaciones
    if (position < 0)
        return Results.BadRequest(new { error = "'position' must be 0 or higher" });

    if (string.IsNullOrWhiteSpace(value))
        return Results.BadRequest(new { error = "'value' cannot be empty" });

    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(new { error = "'text' cannot be empty" });

    var words = text.Split(' ').ToList(); // Se crea una lista de palabras a partir del texto, dividiéndolo por espacios.

    if (position >= words.Count) // Verifica si la posición es mayor o igual al número de palabras.
        words.Add(value);        // Si es mayor o igual, se agrega el valor al final de la lista.
    else
        words.Insert(position, value); // Inserta el valor en la posición indicada.

    var newText = string.Join(" ", words); // Une la lista de palabras en un solo string separado por espacios.

    if (xml)
    {
        var xmlResponse =
            $"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<Result xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
            $"<Ori>{text}</Ori>" +
            $"<New>{newText}</New>" +
            "</Result>";

        return Results.Content(xmlResponse, "application/xml");
    }

    return Results.Ok(new { ori = text, @new = newText }); // Retorna el texto original y el nuevo texto con estado 200 (OK).

}).DisableAntiforgery();

///////////////////////////////////////////////////
// ENDPOINT /replace
///////////////////////////////////////////////////
app.MapPut("/replace/{length:int}", ([FromRoute] int length, [FromQuery] string value, [FromForm] string text, [FromHeader(Name = "xml")] bool xml = false) =>
{
    // Validaciones
    if (length <= 0)
        return Results.BadRequest(new { error = "'length' must be greater than 0" });

    if (string.IsNullOrWhiteSpace(value))
        return Results.BadRequest(new { error = "'value' cannot be empty" });

    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(new { error = "'text' cannot be empty" });

    var words = text.Split(' ').ToList(); // Se crea una lista de palabras a partir del texto, dividiéndolo por espacios.

    var replacedWords = words.Select(word =>
    {
        // Se eliminan los caracteres que no son letras para contar la longitud real de la palabra.
        var cleanWord = new string(word.Where(char.IsLetter).ToArray());
        // Si la longitud coincide, se reemplaza por el valor dado; si no, se mantiene la palabra original.
        return cleanWord.Length == length ? value : word;

    }).ToList(); // Convierte todo en una lista nuevamente.

    var newText = string.Join(" ", replacedWords); // Une la lista de palabras en un solo string separado por espacios.

    if (xml)
    {
        var xmlResponse =
            $"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<Result xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
            $"<Ori>{text}</Ori>" +
            $"<New>{newText}</New>" +
            "</Result>";

        return Results.Content(xmlResponse, "application/xml");
    }

    return Results.Ok(new { ori = text, @new = newText }); // Retorna el texto original y el nuevo texto con estado 200 (OK).

}).DisableAntiforgery();

///////////////////////////////////////////////////
// ENDPOINT /erase
///////////////////////////////////////////////////
app.MapDelete("/erase/{length:int}", ([FromRoute] int length, [FromForm] string text, [FromHeader(Name = "xml")] bool xml = false) =>
{
    // Validaciones
    if (length <= 0)
        return Results.BadRequest(new { error = "'length' must be greater than 0" });

    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(new { error = "'text' cannot be empty" });

    var words = text.Split(' ').ToList(); // Se crea una lista de palabras a partir del texto.

    // Se eliminan todas las palabras cuya longitud real coincida con 'length'.
    var filteredWords = words.Where(word => new string(word.Where(char.IsLetter).ToArray()).Length != length).ToList();

    var newText = string.Join(" ", filteredWords); // Une la lista resultante en un solo string.

    if (xml)
    {
        var xmlResponse =
            $"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<Result xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
            $"<Ori>{text}</Ori>" +
            $"<New>{newText}</New>" +
            "</Result>";

        return Results.Content(xmlResponse, "application/xml");
    }

    return Results.Ok(new { ori = text, @new = newText }); // Retorna el texto original y el modificado con estado 200 (OK).
}).DisableAntiforgery();

app.Run();
