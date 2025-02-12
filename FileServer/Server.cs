using System.Net.Sockets;
using System.Net;
using System.Text;
using TcpLib;

namespace FileServer
{
    internal class Server
    {
        static async Task Main(string[] args) => await new Server().RunAsync();

        private TcpListener listener = null!;
        private List<TcpClient> clients = [];

        private async Task RunAsync()
        {
            listener = new TcpListener(IPAddress.Any, 6596);
            listener.Start();
            Console.WriteLine("Сервер запущен и слушает на порту 6596");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine($"Подключился клиент {client.Client.RemoteEndPoint}");
                lock (clients)
                    clients.Add(client);

                _ = ListenToClient(client);
            }
        }

        private async Task ListenToClient(TcpClient client)
        {
            while (true)
            {
                string filePath = await client.ReceiveFileToPath();
                await SendAll(filePath);
            }
        }

        private async Task SendAll(string text)
        {
            IReadOnlyList<TcpClient> clientsCopy;
            lock (clients)
                clientsCopy = clients.ToArray();

            List<Task> tasks = [];
            foreach (TcpClient client in clientsCopy)
                tasks.Add(client.SendFileFromPath(text));
            await Task.WhenAll(tasks);
        }
    }
}
