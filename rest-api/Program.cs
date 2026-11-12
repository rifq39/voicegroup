using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "C# REST API",
        Version = "v1",
        Description = "In-memory item API. List, fetch, and create items."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "C# REST API v1");
    options.DocumentTitle = "C# REST API";
});

var items = new List<Item>
{
    new(1, "alpha"),
    new(2, "beta")
};

app.MapGet("/", () => Results.Ok(new
{
    service = "csharp-rest-api",
    health = "/health",
    items = "/api/items",
    swagger = "/swagger"
}))
.WithName("GetServiceInfo")
.WithTags("Service")
.WithSummary("Service index");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("GetHealth")
    .WithTags("Service")
    .WithSummary("Health check")
    .Produces(StatusCodes.Status200OK);

app.MapGet("/api/items", () => items)
    .WithName("ListItems")
    .WithTags("Items")
    .WithSummary("List items")
    .WithDescription("Returns every item currently held in memory.")
    .Produces<List<Item>>(StatusCodes.Status200OK);

app.MapGet("/api/items/{id:int}", (int id) =>
{
    var item = items.FirstOrDefault(i => i.Id == id);
    return item is null ? Results.NotFound() : Results.Ok(item);
})
.WithName("GetItem")
.WithTags("Items")
.WithSummary("Get an item by id")
.Produces<Item>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

app.MapPost("/api/items", (CreateItem request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest(new { message = "name is required" });
    }

    var item = new Item(items.Count == 0 ? 1 : items.Max(i => i.Id) + 1, request.Name.Trim());
    items.Add(item);
    return Results.Created($"/api/items/{item.Id}", item);
})
.WithName("CreateItem")
.WithTags("Items")
.WithSummary("Create an item")
.WithDescription("Name is required. The response includes the new id.")
.Produces<Item>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.Run();

record Item(int Id, string Name);
record CreateItem(string Name);
