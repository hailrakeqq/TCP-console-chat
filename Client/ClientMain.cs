using System.Net.Sockets;
class ClientMain
{
    public static string? userName;
    public static async Task Main(string[] args)
    {
        using TcpClient client = new TcpClient();

        Console.Write("Enter ip address for connect: ");
        string? host = Console.ReadLine();
        Console.Write("Enter port for connect: ");
        int port = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter your name: ");
        userName = Console.ReadLine();
        Console.WriteLine($"Welcome, {userName}");

        StreamReader? Reader = null;
        StreamWriter? Writer = null;

        try
        {
            client.Connect(host, port);

            Reader = new StreamReader(client.GetStream());
            Writer = new StreamWriter(client.GetStream());

            //run new thread for recieve messages
            Task.Run(() => RecieveMessageAsync(Reader));
            await SendMessageAsync(Writer);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Reader?.Close();
            Writer?.Close();
        }
    }

    public static async Task RecieveMessageAsync(StreamReader reader)
    {
        while (true)
        {
            try
            {
                string? message = await reader.ReadLineAsync();

                if (message == null || message == "")
                    continue;

                Console.WriteLine(message);
            }
            catch
            {
                break;
            }
        }
    }

    public static async Task SendMessageAsync(StreamWriter writer)
    {

        try
        {
            await writer.WriteLineAsync(userName);
            await writer.FlushAsync();
            Console.WriteLine("Type message and press Enter for send\n");

            while (true)
            {
                await writer.WriteLineAsync(Console.ReadLine());
                await writer.FlushAsync();
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}