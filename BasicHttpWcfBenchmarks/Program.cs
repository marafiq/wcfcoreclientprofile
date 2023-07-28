using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using BasicHttpWcfBenchmarks.PingService;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.ObjectPool;
using Nito.AsyncEx;

namespace BasicHttpWcfBenchmarks
{
    public class Program
    {
        public class PoolPolicy : IPooledObjectPolicy<PingServiceClient>
        {
            public PingServiceClient Create()
            {
                return new PingServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.None), new EndpointAddress("http://pingservice/ping"));
            }

            public bool Return(PingServiceClient obj)
            {
                return true;
            }
        }
        public static async Task Main(string[] args)
        {
            await Task.Delay(100);
            ClientBase<IPingService>.CacheSetting = CacheSetting.AlwaysOn;
            var config = DefaultConfig.Instance;
            config.WithOption(ConfigOptions.Default, true);
            
            var summary = BenchmarkRunner.Run<Benchmarks>(config, args);
          
            
            ObjectPool<PingServiceClient> pool =
            new DefaultObjectPool<PingServiceClient>(new PoolPolicy());
          /*var poolFillTasks = Enumerable.Range(0, 1000)
              .Select(val => Task.Run(() =>
              {
                  var pingServiceClient = pool.Get();
                  

                  pool.Return(pingServiceClient);
              }));
          Task.WaitAll(poolFillTasks.ToArray());*/
          Stopwatch stopwatch = new();
          stopwatch.Start();
          
              var tasks = Enumerable.Range(0, 64)
                  .Select(val => Task.Run(() =>
                  {
                      var pingServiceClient = pool.Get();
                      var r=pingServiceClient.GetData(new GetDataRequest(val));
                      //Console.WriteLine(r.GetDataResult);
                      pool.Return(pingServiceClient);
                  }));
              AsyncContext.Run(() => Task.WhenAll(tasks.ToArray()));
          

          Console.WriteLine(stopwatch.ElapsedMilliseconds);
          var pooled = stopwatch.ElapsedMilliseconds;
          stopwatch.Stop();
          stopwatch.Start();
          var pingServiceClient=new PingServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.None), new EndpointAddress("http://pingservice/ping"));
          
              var t2 = Enumerable.Range(0, 64)
                  .Select(val => Task.Run(() =>
                  {

                      var r=pingServiceClient.GetData(new GetDataRequest(val));
                      //Console.WriteLine(r.GetDataResult);

                  }));
              AsyncContext.Run(() => Task.WhenAll(t2.ToArray()));
          
          Console.WriteLine(stopwatch.ElapsedMilliseconds);
          Console.WriteLine(pooled - stopwatch.ElapsedMilliseconds);
          Console.WriteLine("Static is fast.");
            //ClientBase<IPingService>.CacheSetting = CacheSetting.AlwaysOn;
            //ClientBase<IPingService>.CacheSetting = CacheSetting.AlwaysOn;
            /*PingServiceClient StaticPingServiceClient = new(new BasicHttpBinding(BasicHttpSecurityMode.None), new EndpointAddress("http://localhost:5002/ping"));
            
            var tasks = Enumerable.Range(0, 40)
                .Select(_ => Task.Run(() => { StaticPingServiceClient.GetData(new GetDataRequest(2)); }));
            Task.WaitAll(tasks.ToArray());
            StaticPingServiceClient.GetData(new GetDataRequest(2));
            */
            
            //var config = DefaultConfig.Instance;
            //var summary = BenchmarkRunner.Run<Benchmarks>(config, args);
            /*Benchmarks benchmarks = new();
            benchmarks.UseSingletonClient();*/

            /*ChannelFactory<IPingService> StaticChannelFactory = new ChannelFactory<IPingService>(
                new BasicHttpBinding(BasicHttpSecurityMode.None)
                , new EndpointAddress("http://localhost:5002/ping"));
            for (int i = 0; i < 40; i++)
            {
                var channel = StaticChannelFactory.CreateChannel(new EndpointAddress("http://localhost:5002/ping"));
                
                var val=channel.GetData(new GetDataRequest(i));
                Console.WriteLine(val.GetDataResult);
            }
            var channel1 = StaticChannelFactory.CreateChannel(new EndpointAddress("http://localhost:5002/ping"));
            var val1=channel1.GetData(new GetDataRequest(2));*/



        }
    }
}