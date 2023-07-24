```

BenchmarkDotNet v0.13.6, Debian GNU/Linux 11 (bullseye) (container)
12th Gen Intel Core i7-1265U, 1 CPU, 12 logical and 6 physical cores
.NET SDK 7.0.304
  [Host]     : .NET 7.0.7 (7.0.723.27404), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.7 (7.0.723.27404), X64 RyuJIT AVX2


```
|    Method |     Mean |     Error |    StdDev |    StdErr |      Min |       Q1 |   Median |       Q3 |      Max |  Op/s | Completed Work Items | Lock Contentions | Allocated |
|---------- |---------:|----------:|----------:|----------:|---------:|---------:|---------:|---------:|---------:|------:|---------------------:|-----------------:|----------:|
| Singleton | 1.154 ms | 0.0103 ms | 0.0096 ms | 0.0025 ms | 1.137 ms | 1.148 ms | 1.154 ms | 1.159 ms | 1.171 ms | 866.6 |               0.2000 |                - |     274 B |
|    Pooled | 1.147 ms | 0.0045 ms | 0.0040 ms | 0.0011 ms | 1.140 ms | 1.143 ms | 1.147 ms | 1.151 ms | 1.152 ms | 871.9 |               0.2000 |                - |     273 B |
