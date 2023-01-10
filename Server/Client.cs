using System.Net.Sockets;
using ServerSide;
namespace ClientSide;

class Client
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public StreamReader Reader { get; }
    public StreamWriter Writer { get; }
    TcpClient client;
    Server server;

    public Client(TcpClient client, Server server)
    {
        this.client = client;
        this.server = server;
        var stream = client.GetStream();

        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream);
    }

    public async Task MainActivityAsync()
    {
        try
        {
            string? clientName = await Reader.ReadLineAsync();
            string message = $"{clientName} - join to chat.";
            await server.BroadcastMessageAsync(message, Id);
            Console.WriteLine(message);
            try
            {
                while (true)
                {
                    message = await Reader.ReadLineAsync();

                    if (message == null)
                        continue;

                    if (message == "!left")
                    {
                        server.DisconnectClient(Id);
                        break;
                    }

                    message = $"{clientName}: {message}";
                    await server.BroadcastMessageAsync(message, Id);
                    Console.WriteLine(message);
                }
            }
            catch
            {
                message = $"{clientName} - has left chat.";
                await server.BroadcastMessageAsync(message, Id);
                Console.WriteLine(message);
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            server.DisconnectClient(Id);
        }
    }

    public void Close()
    {
        Reader.Close();
        Writer.Close();
        // client.Close();
    }
}