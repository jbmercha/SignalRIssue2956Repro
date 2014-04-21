using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalrTransportTimeoutRepro.Client
{
    class Program
    {
        private static HubConnection _hubConnection;
        static void Main()
        {
            var p = new Program();
            p.MainAsync().Wait();
        }

        private async Task MainAsync()
        {
            _hubConnection = CreateConnection();
            Console.WriteLine("Created connection, waiting 1 minute to start it.");
            Thread.Sleep(TimeSpan.FromMinutes(1));
            await _hubConnection.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            _hubConnection.Stop();
        }

        private static HubConnection CreateConnection()
        {
            if(_hubConnection != null) _hubConnection.Dispose();
            _hubConnection = new HubConnection("http://localhost:8080")
            {
                Credentials = CredentialCache.DefaultCredentials
            };
            _hubConnection.Error += e => Console.WriteLine(e.Message);
            _hubConnection.Closed += async () =>
            {
                Console.WriteLine("Connection closed.");
                Console.WriteLine("Reconnecting in 500 ms");
                Thread.Sleep(500);
                await _hubConnection.Start(); // Will never reconnect. Workaround: Recreate connection CreateConnection().Start();
            };

            _hubConnection.TraceLevel = TraceLevels.All;
            _hubConnection.TraceWriter = Console.Out;

            var hub = _hubConnection.CreateHubProxy("MyHub");
            hub.On<string, string>("addMessage", (name, message) => Console.WriteLine("{0}: {1}", name, message));
            return _hubConnection;
        }
    }
}
