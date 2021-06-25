using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.AzureRedis.Api.Services.RedisCache;

namespace Sample.AzureRedis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisCacheController : ControllerBase
    {
        private readonly IRedisCacheService _redisCacheService;
        public RedisCacheController(IRedisCacheService baseCacheService)
        {
            _redisCacheService = baseCacheService ?? throw new ArgumentNullException(nameof(baseCacheService));
        }

        [HttpPost("{key}")]
        public async Task<ActionResult> AddCacheVAlue(string key, [FromBody] string value)
        {
            await _redisCacheService.StringSetAsync(key, value);
            return Ok();
        }

        [HttpGet("{key}")]
        public async Task<ActionResult<string>> GetCacheValue(string key)
        {
            return Ok(await _redisCacheService.StringGetAsync<string>(key));
        }

        [HttpDelete("{key}")]
        public async Task<ActionResult<string>> DeleteCacheValue(string key)
        {
            await _redisCacheService.KeyDeleteAsync(key);
            return Ok();
        }

        [HttpPost("publish")]
        public async Task<ActionResult<string>> PublishCacheValue([FromBody] string message)
        {
            await _redisCacheService.PublishAsync("test", message);
            return Ok();
        }
    }
}
