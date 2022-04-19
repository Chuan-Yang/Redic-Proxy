namespace RedisProxy.Service.IntegrationTest.Configs;

public class TestConfig
{
    public CacheOptions Cache { get; set; }
    public RedisOptions Redis { get; set; }
    public string ServiceUri { get; set; }
}
