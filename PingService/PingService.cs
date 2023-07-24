namespace PingService;

public class PingService : IPingService
{
    public string GetData(int value)
    {
        Thread.Sleep(10);
        return $"Replying to your Ping - {value} with Pong.";
    }
}