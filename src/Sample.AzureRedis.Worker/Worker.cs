using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sample.AzureRedis.Api.Services.RedisCache;

namespace Sample.AzureRedis.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRedisCacheService _redisCacheService;

        public Worker(ILogger<Worker> logger, IRedisCacheService redisCacheService)
        {
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _redisCacheService.SubscribeAsync("test", (redisChannel, redisValue) =>
            {
                _logger.LogInformation($"Message received at {DateTime.UtcNow} - {redisChannel}");
                _logger.LogInformation($"\tMessage content: {redisValue}");
            });
        }
    }
}
