using System.Linq;
using System.Threading.Tasks;
using BasicHttpWcfBenchmarks.PingService;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.ObjectPool;

namespace BasicHttpWcfBenchmarks
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        static readonly PingServiceClient StaticPingServiceClient = new();

        [GlobalSetup]
        public void GlobalSetup()
        {
            
        }
        [Benchmark]
        public void UseSingletonClient()
        {
            var tasks = Enumerable.Range(0, 3)
                .Select(_ => Task.Run(() =>
                {
                    StaticPingServiceClient.GetData(new GetDataRequest(2));
                }));
            Task.WaitAll(tasks.ToArray());
        }
        
        [Benchmark(Baseline = true)]
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
            new DefaultObjectPool<PingServiceClient>(new DefaultPooledObjectPolicy<PingServiceClient>(),32);
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
        
        

        
    }
}