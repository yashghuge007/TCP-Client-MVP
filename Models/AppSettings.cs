namespace ABXClient.Models;

public class AppSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 3000;
    public string OutputPath { get; set; } = "AllTradesReceivedByClient.json";
}
