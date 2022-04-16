using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisProxy.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController : ControllerBase
{
    private readonly ILogger<DataController> _logger;
    private readonly IConnectionMultiplexer  _redis;

    public DataController(ILogger<DataController> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    [HttpGet("{key}")]
    public string Get(string key)
    {
        var db = _redis.GetDatabase();

        var value = db.StringGet(key);

        return value;
    }
}
