using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.AzureRedis.Api.Extensions;
using Sample.AzureRedis.Api.Services.RedisCache;
using Sample.AzureRedis.Worker;
using Serilog;
using StackExchange.Redis;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

SerilogExtensions.AddSerilog(configuration);

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog(Log.Logger)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(hostContext.Configuration.GetConnectionString("RedisConnection")));
        services.AddSingleton<IRedisCacheService, RedisCacheService>();
    })
    .Build();

await host.StartAsync();

Console.WriteLine("Waiting for new messages.");

while (true) ;