using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Sample.AzureRedis.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public Worker(ILogger<Worker> logger, IConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _connectionMultiplexer.GetSubscriber();
            return subscriber.SubscribeAsync("channelName", (redisChannel, redisValue) =>
            {
                _logger.LogInformation($"Message received at {DateTime.UtcNow}");
                _logger.LogInformation($"\tMessage content: {redisValue}");
            });
        }
    }
}
