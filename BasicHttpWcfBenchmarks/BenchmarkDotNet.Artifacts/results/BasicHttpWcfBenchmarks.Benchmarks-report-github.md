```

BenchmarkDotNet v0.13.6, Debian GNU/Linux 11 (bullseye) (container)
Unknown processor
.NET SDK 7.0.306
  [Host]     : .NET 7.0.9 (7.0.923.32018), Arm64 RyuJIT AdvSIMD [AttachedDebugger]
  DefaultJob : .NET 7.0.9 (7.0.923.32018), Arm64 RyuJIT AdvSIMD


```
|             Method |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |    Gen0 |    Gen1 | Allocated | Alloc Ratio |
|------------------- |---------:|---------:|---------:|---------:|------:|--------:|--------:|--------:|----------:|------------:|
| UseSingletonClient | 12.05 ms | 1.104 ms | 3.220 ms | 11.09 ms |  0.64 |    0.23 |       - |       - |  42.09 KB |        0.16 |
| UseTransientClient | 19.89 ms | 1.416 ms | 3.949 ms | 19.14 ms |  1.00 |    0.00 | 62.5000 | 31.2500 | 262.62 KB |        1.00 |
|    UsePooledClient | 11.72 ms | 0.621 ms | 1.802 ms | 11.35 ms |  0.61 |    0.15 |       - |       - |  42.84 KB |        0.16 |
