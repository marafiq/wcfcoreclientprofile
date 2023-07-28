using Nito.AsyncEx;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
    options.ListenAnyIP(80);
});


builder.Services.AddSingleton<ObjectPool<PingServiceClient>>(_ =>
    new DefaultObjectPool<PingServiceClient>(new PoolPolicy()));

builder.Services.AddSingleton<PingServiceClient>(_ =>
    new PingServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.None)
        {
            SendTimeout = TimeSpan.FromMilliseconds(2000),
            ReceiveTimeout = TimeSpan.FromMilliseconds(2000)
        },
        new EndpointAddress("http://pingservice/ping")));

var app = builder.Build();

ClientBase<IPingService>.CacheSetting = CacheSetting.AlwaysOn;

app.MapGet("/", () => "Hello World!");


app.MapGet("/threadpoolconfig", (ILogger<Program> logger) =>
{
    ThreadPool.GetMinThreads(out var minWorkerThreads, out var minIoThreads);
    ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxIoThreads);
    ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);
    logger.LogInformation("Min Workers {MinWorkers} {MinIoThreads} {MaxWorkers} {MaxIoThreads} {AvailableWorkers} {AvailableIoThreads}", minWorkerThreads,
        minIoThreads, maxWorkerThreads, maxIoThreads, workerThreads, completionPortThreads);
    return Results.Json(new ThreadPoolConfig(minIoThreads, maxWorkerThreads, maxIoThreads, workerThreads, completionPortThreads));
});

app.MapGet("/singleton",
    (PingServiceClient pingServiceClient) =>
    {
        return pingServiceClient.GetData(new GetDataRequest(Random.Shared.Next())).GetDataResult;
    });
app.MapGet("/asyncsingleton",
    (PingServiceClient pingServiceClient) =>
        AsyncContext.Run(() => pingServiceClient.GetData(new GetDataRequest(Random.Shared.Next()))).GetDataResult);
app.MapGet("/pooled", (ObjectPool<PingServiceClient> pool) =>
{
    var client = pool.Get();
    var data = client.GetData(new GetDataRequest(Random.Shared.Next()));
    pool.Return(client);
    return data.GetDataResult;
});

app.MapGet("/asyncpooled", (ObjectPool<PingServiceClient> pool) =>
{
    var client = pool.Get();
    var data = AsyncContext.Run(() => client.GetData(new GetDataRequest(Random.Shared.Next())));
    pool.Return(client);
    return data.GetDataResult;
});
app.MapGet("/pureasync", async (ObjectPool<PingServiceClient> pool) =>
{
    var client = pool.Get();
    var data = await client.GetDataAsync(new GetDataRequest(Random.Shared.Next()));
    pool.Return(client);
    return data.GetDataResult;
});

app.MapGet("/healthy", () => "Healthy");

app.Run();

public class PoolPolicy : IPooledObjectPolicy<PingServiceClient>
{
    public PingServiceClient Create()
    {
        return new PingServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.None)
        {
            SendTimeout = TimeSpan.FromMinutes(5),
            ReceiveTimeout = TimeSpan.FromMinutes(5)
        }, new EndpointAddress("http://pingservice/ping"));
    }

    public bool Return(PingServiceClient obj)
    {
        if (obj.State == CommunicationState.Faulted)
        {
            return false;
        }

        return true;
    }
}
public record ThreadPoolConfig(int MinIoThreads,int MaxWorkerThreads,int MaxIoThreads,int WorkerThreads,int CompletionPortThreads);
//dotnet-counters monitor -n dotnet --counters "Microsoft.AspNetCore.Hosting,System.Net.Http,System.Runtime,Microsoft-AspNetCore-Server-Kestrel,System.Net.Http,System.Net.Sockets,System.Net.Security"

//dotnet-counters monitor -n dotnet --counters "Microsoft.AspNetCore.Hosting,System.Net.Http,System.Runtime" 