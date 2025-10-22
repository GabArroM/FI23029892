using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var list = new List<object>();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapPost("/", ([FromHeader(Name = "xml")] bool xml = false) =>

{
    if (xml)
    {
        var xmlItems = string.Join("", list.Select(x => $"<item>{x}</item>"));//Este listado fue con ayuda de ChatGPT.

        var xmlResponse =
            $"<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<Result xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
            $"<list>[{xmlItems}]</list>" +
            "</Result>";

        return Results.Content(xmlResponse, "application/xml");
    }


    return Results.Ok(list);
});

app.MapPut("/", ([FromForm] int quantity, [FromForm] string type) =>
{
    if (quantity < 1)
    {
        return Results.BadRequest(new { error = "'quantity' must be higher than zero" });
    }
    if (type != "int" && type != "float")
    {
        return Results.BadRequest(new { error = "'type' must be 'int' or 'float'" });//Esta validacion se autocompleto en base a Copilot.
    }

    var random = new Random();
    if (type == "int")
    {
        for (; quantity > 0; quantity--)
        {
            list.Add(random.Next());
        }
    }
    else if (type == "float")
    {
        for (; quantity > 0; quantity--)
        {
            list.Add(random.NextSingle());
        }
    }
    return Results.Ok();

}).DisableAntiforgery();

app.MapDelete("/", ([FromForm] int quantity) =>
{
    if (quantity < 1)
    {
        return Results.BadRequest(new { error = "'quantity' must be higher than zero" });
    }
    if (quantity > list.Count)
    {
        return Results.BadRequest(new { error = "'quantity' is greater than the size of the list" });
    }

    for (; quantity > 0; quantity--)
    {
        list.RemoveAt(0);
    }
    return Results.Ok();

}).DisableAntiforgery();

app.MapPatch("/", () =>
{
    for (; list.Count > 0;)
    {
        list.RemoveAt(0);
    }
    return Results.Ok();
});

app.Run();
