using Grpc.Core;
using ConsoleCommunication.Shared; // Namespace definito nel .proto

namespace ConsoleCommunicationServer
{
    public class MyGreeterService : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Ricevuta chiamata da: {request.Name}");
            return Task.FromResult(new HelloReply { Message = $"Ciao {request.Name}, test gRPC riuscito!" });
        }

        public override async Task Countdown(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            for (int i = 5; i > 0; i--)
            {
                await responseStream.WriteAsync(new HelloReply
                {
                    Message = $"Messaggio {i} per {request.Name}..."
                });
                await Task.Delay(1000); // Simuliamo un'attesa di 1 secondo tra un invio e l'altro
            }
        }

        public override async Task MonitoraSistema(HelloRequest request, IServerStreamWriter<MonitorResponse> responseStream, ServerCallContext context)
        {
            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                var usage = random.Next(0, 100);
                var response = new MonitorResponse
                {
                    CpuUsage = usage,
                    Stato = usage > 80 ? StatoServer.Sovraccarico : StatoServer.Ok,
                    LogRecenti = { "Connessione stabile", $"Check n. {i}", "Nessuna anomalia" }
                };

                await responseStream.WriteAsync(response);
                await Task.Delay(800);
            }
        }

        public override async Task ConversazioneSpeciale(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            // Leggiamo finché il client invia messaggi
            await foreach (var request in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"Il client dice: {request.Name}");

                // Rispondiamo immediatamente nello stesso canale
                await responseStream.WriteAsync(new HelloReply
                {
                    Message = $"Ricevuto '{request.Name}' alle {DateTime.Now:T}"
                });
            }
        }
    }
}
