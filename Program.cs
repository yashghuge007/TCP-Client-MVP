using System.Net.Sockets;
using System.Text.Json;
using ABXClient.Models;
using ABXClient.Utils;
using Microsoft.Extensions.Configuration;

public class Program()
{
    public static async Task Main(string[] args)
    {
        // Load configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var appSettings = new AppSettings();
        config.GetSection("AppSettings").Bind(appSettings);

        var outputPath = appSettings.OutputPath; //config["AppSettings:OutputPath"];

        Console.WriteLine("Please make sure the properties in appsettings.json are properly configured...");
        Console.WriteLine($"Application will start with host = {appSettings.Host}, port={appSettings.Port} and outputPath = {outputPath}");
        Console.WriteLine($"outputPath = {outputPath} is relative to ABXClient.sln");
        Console.WriteLine("Please make sure the properties in appsettings.json are properly configured and restart the application with proper settings else press any key to continue...");
        Console.ReadLine();
        outputPath = "..\\..\\..\\" + outputPath;
        Console.WriteLine("Starting the application...");

        var receivedPackets = new Dictionary<int, TickerPacket>();
        int highestSequence = 0;

        Console.WriteLine($"Connecting to ABX server {appSettings.Host}:{appSettings.Port}...");

        try
        {
            var client = new TcpClient();
            await client.ConnectAsync(appSettings.Host, appSettings.Port);
            Console.WriteLine("Connected!");
            var stream = client.GetStream();

            await stream.WriteAsync(new byte[] { 0x01, 0x00 }); // send stream-all request hence only 1st byte is 0x01, 2nd byte is 0x00 (no sequence number)
            Console.WriteLine("Sent stream-all request to server.");

            try
            {
                while (true)
                {
                    var packet = HelperFunctions.ReadPacket(stream);
                    if (packet == null) break;

                    receivedPackets[packet.Sequence] = packet;
                    highestSequence = Math.Max(highestSequence, packet.Sequence);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Server closed connection after stream-all.");
            }

            // Step 2: Detect missing sequences
            var missing = Enumerable.Range(1, highestSequence)
                .Where(seq => !receivedPackets.ContainsKey(seq))
                .ToList();

            Console.WriteLine($"Received {receivedPackets.Count} packets. Missing {missing.Count}...");

            // Step 3: Request missing sequences
            // Since the server disconnected after the stream-all request, we need to reconnect
            Console.WriteLine("Reconnecting to server...");
            client.Dispose(); // Dispose the old client to release resources
            client = new TcpClient();
            await client.ConnectAsync(appSettings.Host, appSettings.Port);
            Console.WriteLine("Reconnected!");
            stream = client.GetStream();

            //could add a recursive loop in  case any packet fails in between but not needed for such small amount of data
            foreach (var seq in missing)
            {
                await stream.WriteAsync(new byte[] { 0x02, (byte)seq }); // callType = 2, 

                var packet = HelperFunctions.ReadPacket(stream);
                if (packet != null)
                    receivedPackets[packet.Sequence] = packet;
            }

            Console.WriteLine($"Total packets after recovery: {receivedPackets.Count}");

            // Step 4: Write JSON file
            var ordered = receivedPackets.OrderBy(p => p.Key).Select(p => p.Value).ToList();
            var options = new JsonSerializerOptions { WriteIndented = true };
            await File.WriteAllTextAsync(outputPath, JsonSerializer.Serialize(ordered, options));

            Console.WriteLine($"Saved to {appSettings.OutputPath}");
            client.Close();
            stream.Close();
            stream.Dispose();
            client.Dispose();
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException: {ex.Message}");
        }
    }
}
