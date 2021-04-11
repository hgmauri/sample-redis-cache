using System.Threading.Tasks;
using Sample.AzureRedis.Api.Services.Base;

namespace Sample.AzureRedis.Api.Services.RedisCache
{
    public interface IRedisCacheService : IBaseCacheService
    {
        Task PublishMessage(string message);
    }
}
