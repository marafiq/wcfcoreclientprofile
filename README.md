Observe the Thread Pool Starvation Issue

1. You must have docker and docker-compose installed.
2. Install bombardier using `choco install bombardier`
3. Run `docker-compose up`

## What kind of application is running

- Ping Service is simple Core WCF service with `basicHttpBinding`. It can be written in any language as long as it supports SOAP. It has a constant 10 ms thread sleep to introduce latency.
- WebApiConsumingBasicHttp is a simple minimal API exposing bunch of endpoints outlining different techniques to increase the throughput of the consumer of PingService, which is generated using Core WCF client proxy with both async and sync operations.

## Put some load
Run the following command in your shell.
`bombardier.exe -c 100 -d 30s  -r 6000  -l -H "content-type: application/json"  http://localhost:5003/singleton`

Now in `WebApiConsumingBasicHttp` project, delete the `runtimeconfig.template.json` the file, and run `docker-compose down` and then `docker-compose up`

And finally run the same command `bombardier.exe -c 100 -d 30s  -r 6000  -l -H "content-type: application/json"  http://localhost:5003/singleton`

You will notice that app has crashed. Why, because it is starved of threads.

## How we prove that

- Open a terminal into the container of `WebApiConsumingBasicHttp` app
- Type `ECHO $PATH` and hit enter
- And the run this command `dotnet-counters monitor -n dotnet`
- Once it is active, maximize your window, so you can see all counters
- New shell, and run this `bombardier.exe -c 100 -d 30s  -r 6000  -l -H "content-type: application/json"  http://localhost:5003/singleton`
- Now closely watch the bottom 3 counters starting with 'Thread'
- Thread pool queue length will be greater than zero meaning app is waiting for thread to become available to perform the pending work.



Note: If you would like to run the benchmarks project, add the below in docker compose

```docker
  basichttpwcfbenchmarks:
    user: root
    image: basichttpwcfbenchmarks
    depends_on: 
      - pingservice
      - webapiconsumingbasichttp
    build:
      context: .
      dockerfile: BasicHttpWcfBenchmarks/Dockerfile
```