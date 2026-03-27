using ConsoleCommunicationServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configura Kestrel per forzare HTTP/2 sulla porta 5005 senza TLS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5005, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

// Aggiungi il servizio gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// Mappiamo il servizio (dobbiamo ancora creare la classe GreeterService)
app.MapGrpcService<MyGreeterService>();

//app.Run("http://localhost:5005");
app.Run(); // Non serve passare l'URL qui se l'abbiamo messo sopra