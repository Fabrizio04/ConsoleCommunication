using Grpc.Core;
using Grpc.Net.Client;
using ConsoleCommunication.Shared;

// Creiamo il canale verso il server
using var channel = GrpcChannel.ForAddress("http://localhost:5005");
var client = new Greeter.GreeterClient(channel);

Console.WriteLine("Premi un tasto per inviare il saluto al server...");
Console.ReadKey();

// Chiamata gRPC
var reply = await client.SayHelloAsync(new HelloRequest { Name = "Fabrizio" });

Console.WriteLine("Risposta dal Server: " + reply.Message);

// Chiamata Streaming

Console.WriteLine("\nPremi un tasto per avviare lo streaming...");
Console.ReadKey();

using var call = client.Countdown(new HelloRequest { Name = "Fabrizio" });

// Leggiamo finché il server non chiude il canale
await foreach (var response in call.ResponseStream.ReadAllAsync())
{
    Console.WriteLine("Ricevuto: " + response.Message);
}
Console.WriteLine("Streaming terminato!");

Console.WriteLine("\nPremi un tasto per avviare lo streaming...");
Console.ReadKey();
Console.WriteLine("--- MONITORAGGIO SISTEMA IN CORSO ---");
using var monitorCall = client.MonitoraSistema(new HelloRequest { Name = "Fabrizio" });

await foreach (var update in monitorCall.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"[Stato: {update.Stato}] CPU: {update.CpuUsage}% - Ultimo log: {update.LogRecenti.Last()}");
}
Console.WriteLine("Monitoraggio completato.");

Console.WriteLine("\nPremi un tasto per avviare la comunicazione bidirezionale...");
Console.ReadKey();

using var callB = client.ConversazioneSpeciale();

// Task per leggere le risposte (in background)
var readTask = Task.Run(async () =>
{
    await foreach (var response in callB.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"Server risponde: {response.Message}");
    }
});

// Task per inviare messaggi (es. da input utente)
foreach (var msg in new[] { "Ehi", "Come va?", "Chiudi tutto" })
{
    await callB.RequestStream.WriteAsync(new HelloRequest { Name = msg });
    await Task.Delay(500);
}

await callB.RequestStream.CompleteAsync(); // Segnala al server che abbiamo finito
await readTask; // Aspetta che anche la lettura finisca

Console.WriteLine("Comunicazione completata.");

Console.WriteLine("\nPremi un tasto per uscire.");
Console.ReadKey();