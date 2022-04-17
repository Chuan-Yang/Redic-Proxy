namespace RedisProxy.Service.Services.Cache;

public interface ICache
{
    // The value and key are made to be strings for simplication, but they can be definitely defined as other types
    string? Get(string key);

    void Set(string key, string value);
}
