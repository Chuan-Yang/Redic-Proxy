using Microsoft.AspNetCore.Mvc;
using RedisProxy.Service.Services.Cache;
using RedisProxy.Service.Services.Cache.LRU;
using StackExchange.Redis;

namespace RedisProxy.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    private readonly IConnectionMultiplexer  _redis;
    private readonly ICache _cache;

    public DataController(ILogger<DataController> logger, IConnectionMultiplexer redis, ICache cache)
    {
        _logger = logger;
        _redis = redis;
        _cache = cache;
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<string>> Get(string key)
    {
        var db = _redis.GetDatabase();

        string value;
        string? cachedValue = _cache.Get(key);
        if (cachedValue != null)
        {
            value = cachedValue;
        }
        else {
            value = await db.StringGetAsync(key);
            if (value == null)
            {
                return NotFound("The key doesn't have a corresponding value in the database");
            }

            _cache.Set(key, value);
        }

        return value;
    }
}
