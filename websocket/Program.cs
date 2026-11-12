using System.Net.WebSockets;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "C# WebSocket",
        Version = "v1",
        Description = "Health and index endpoints. The echo socket is at /ws and is omitted from this document."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "C# WebSocket v1");
    options.DocumentTitle = "C# WebSocket";
});

app.UseWebSockets();

app.MapGet("/", () => Results.Ok(new
{
    service = "csharp-websocket",
    health = "/health",
    websocket = "/ws",
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

app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Expected a WebSocket request");
        return;
    }

    using var socket = await context.WebSockets.AcceptWebSocketAsync();
    await SendAsync(socket, "connected");

    var buffer = new byte[4096];
    try
    {
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, context.RequestAborted);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", context.RequestAborted);
                break;
            }

            var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await SendAsync(socket, $"echo: {text}");
        }
    }
    catch (WebSocketException)
    {
        // The client disconnected.
    }
}).ExcludeFromDescription();

app.Run();

static Task SendAsync(WebSocket socket, string message)
{
    var bytes = Encoding.UTF8.GetBytes(message);
    return socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
}
