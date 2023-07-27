namespace PingService;

public class PingService : IPingService
{
    public string GetData(int value)
    {
        Thread.Sleep(500);
        return $"Replying to your Ping - {value} with Pong.";
    }
}