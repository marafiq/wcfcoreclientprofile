using System;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using BasicHttpWcfBenchmarks.PingService;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.ObjectPool;

/*using BenchmarkDotNet.Diagnostics.dotTrace;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using Microsoft.Extensions.ObjectPool;
using Nito.AsyncEx;*/

namespace BasicHttpWcfBenchmarks
{
    [MemoryDiagnoser]
    //[DotTraceDiagnoser]
    [SimpleJob]
    //[EtwProfiler]
    [AllStatisticsColumn]
    [ThreadingDiagnoser]
    public class Benchmarks
    {
        private static readonly HttpClient StaticHttpClient=new HttpClient();

        [Benchmark(OperationsPerInvoke = 10)]
        public async Task<string> Singleton()
        {
            var stringAsync=await StaticHttpClient.GetStringAsync("http://webapiconsumingbasichttp/singleton");
            return stringAsync;
        }
        [Benchmark(OperationsPerInvoke = 10)]
        public async Task<string> Pooled()
        {
            var stringAsync=await StaticHttpClient.GetStringAsync("http://webapiconsumingbasichttp/pooled");
            return stringAsync;
        }
        /*public static readonly PingServiceClient StaticPingServiceClient =
            new(new BasicHttpBinding(BasicHttpSecurityMode.None), new EndpointAddress("http://pingservice/ping"));

        /*static ChannelFactory<IPingService> StaticChannelFactory = new ChannelFactory<IPingService>(
            new BasicHttpBinding(BasicHttpSecurityMode.None), new EndpointAddress("http://localhost:5002/ping"));#1#


        [GlobalSetup]
        public void GlobalSetup()
        {
            ClientBase<IPingService>.CacheSetting = CacheSetting.AlwaysOn;
            _pool =new DefaultObjectPool<PingServiceClient>(new Program.PoolPolicy(),8);
        }

        [Benchmark(OperationsPerInvoke = 100)]
        public async Task UseSingletonClient()
        {
            var tasks = Enumerable.Range(0, 64)
                .Select(i => Task.Run(() =>
                {
                    try
                    {
                        var x= StaticPingServiceClient.GetData(new GetDataRequest(i));
                        if (x.GetDataResult.Length < 0)
                        {
                            Console.WriteLine("Hello");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }));
            await Task.WhenAll(tasks.ToArray());
        }

        private  ObjectPool<PingServiceClient> _pool;

        [Benchmark(OperationsPerInvoke = 100, Baseline = true)]
        public async Task UsePooledClient()
        {
            var tasks = Enumerable.Range(0, 64)
                .Select(i => Task.Run(() =>
                {
                    var pingServiceClient = _pool.Get();
                    try
                    {
                        var x=pingServiceClient.GetData(new GetDataRequest(i));
                        if (x.GetDataResult.Length < 0)
                        {
                            Console.WriteLine("Hello");
                        }
                        _pool.Return(pingServiceClient);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        pingServiceClient.Close();
                        pingServiceClient = null;
                    }
                    
                    
                }));
          await  Task.WhenAll(tasks.ToArray());
        }*/
        /*[Benchmark]
        public void UseSingletonChannelFactory()
        {var channel = StaticChannelFactory.CreateChannel(new EndpointAddress("http://localhost:5002/ping"));
            var tasks = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(() =>
                {
                    
                    channel.GetData(new GetDataRequest(2));
                    
                }));
            Task.WaitAll(tasks.ToArray());
        }*/

        /*[Benchmark]
        public void UseTransientClient()
        {
            var tasks = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(() =>
                {
                    PingServiceClient transientClient = new();
                    transientClient.GetData(new GetDataRequest(2));
                }));
            Task.WaitAll(tasks.ToArray());
        }

        private readonly ObjectPool<PingServiceClient> _pool =
            new DefaultObjectPool<PingServiceClient>(new DefaultPooledObjectPolicy<PingServiceClient>());

        [Benchmark]
        public void UsePooledClient()
        {
            var tasks = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(() =>
                {
                    var pingServiceClient = _pool.Get();
                    pingServiceClient.GetData(new GetDataRequest(2));
                    _pool.Return(pingServiceClient);
                }));
            Task.WaitAll(tasks.ToArray());
        }


        [Benchmark(Baseline = true)]
        public async Task UsePooledClientAsync()
        {
            var tasks = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(async () =>
                {
                    var pingServiceClient = _pool.Get();
                    await pingServiceClient.GetDataAsync(new GetDataRequest(2));
                    _pool.Return(pingServiceClient);
                }));
            await Task.WhenAll(tasks.ToArray());
        }

        [Benchmark]
        public void UsePooledClientAsyncWithGetResults()
        {
            var tasks = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(() =>
                {
                    var pingServiceClient = _pool.Get();
                    AsyncContext.Run(() => pingServiceClient.GetDataAsync(new GetDataRequest(2)));

                    _pool.Return(pingServiceClient);
                }));
            AsyncContext.Run(() => Task.WhenAll(tasks.ToArray()));
        }
        
        [Benchmark]
        public void UseChannelFactory()
        {
            var tasks = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(() =>
                {
                    var pingServiceClient = _pool.Get();
                    
                    pingServiceClient.GetData(new GetDataRequest(3));
                    
                    AsyncContext.Run(() => pingServiceClient.GetDataAsync(new GetDataRequest(2)));

                    _pool.Return(pingServiceClient);
                }));
            AsyncContext.Run(() => Task.WhenAll(tasks.ToArray()));
        }*/
    }
}