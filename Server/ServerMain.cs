namespace ServerSide
{
    public class ServerMain
    {
        public static async Task Main(string[] args)
        {
            if (args.Length != 1)
                throw new ArgumentException("You must enter number to set server port.\nFor run application type: dotnet run {port}");
         
            Server server = new Server(Convert.ToInt32(args[0]));
            await server.AcceptClientAsync();
        }
    }
}
