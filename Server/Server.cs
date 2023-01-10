using System.Net;
using System.Net.Sockets;
using ClientSide;

namespace ServerSide;
class Server
{
    int port;
    // TcpListener tcpListener = new TcpListener(IPAddress.Any, 8888);
    TcpListener tcpListener;
    List<Client> clients;

    public Server(int port)
    {
        this.port = port;
        tcpListener = new TcpListener(IPAddress.Any, port);
        clients = new List<Client>();
    }

    public async Task AcceptClientAsync()
    {
        tcpListener.Start();
        Console.WriteLine($"Server is running. Waiting for connections...");
        try
        {
            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                Client clientObject = new Client(tcpClient, this);
                clients.Add(clientObject);
                await Task.Run(clientObject.MainActivityAsync);
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            DisconnectAllClients();
        }
    }

    public async Task BroadcastMessageAsync(string message, string id)
    {
        foreach (var client in clients)
        {
            if (client.Id != id)
            {
                await client.Writer.WriteLineAsync(message);
                await client.Writer.FlushAsync();
            }
        }
    }

    public void DisconnectClient(string id)
    {
        Client? client = clients.FirstOrDefault(c => c.Id == id);

        if (client != null)
        {
            clients.Remove(client);
            client.Close();
        }
    }

    public void DisconnectAllClients()
    {
        foreach (var client in clients)
            client.Close();
        tcpListener.Stop();
    }
}

