```

BenchmarkDotNet v0.13.6, Debian GNU/Linux 11 (bullseye) (container)
Unknown processor
.NET SDK 7.0.306
  [Host]     : .NET 7.0.9 (7.0.923.32018), Arm64 RyuJIT AdvSIMD [AttachedDebugger]
  DefaultJob : .NET 7.0.9 (7.0.923.32018), Arm64 RyuJIT AdvSIMD


```
|             Method |     Mean |    Error |   StdDev | Ratio | RatioSD |    Gen0 | Allocated | Alloc Ratio |
|------------------- |---------:|---------:|---------:|------:|--------:|--------:|----------:|------------:|
| UseSingletonClient | 10.06 ms | 0.322 ms | 0.928 ms |  0.51 |    0.12 |       - |  41.95 KB |        0.16 |
| UseTransientClient | 20.58 ms | 1.698 ms | 4.952 ms |  1.00 |    0.00 | 76.9231 | 263.19 KB |        1.00 |
|    UsePooledClient | 10.46 ms | 0.280 ms | 0.813 ms |  0.53 |    0.12 |       - |     43 KB |        0.16 |
