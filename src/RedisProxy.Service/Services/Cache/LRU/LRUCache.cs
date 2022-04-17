using Microsoft.Extensions.Options;
using RedisProxy.Service.Configs;

namespace RedisProxy.Service.Services.Cache.LRU;

public class LRUCache : ICache
{
    private readonly Dictionary<string, LRUNode> _map;
    private readonly LRUDoubleLinkedList _cache;

    // Since the capacity is not used in this example, so it is set as private
    // But if it is needed, we can have a getter for it or any other properties
    private readonly int _capacity;
    private readonly int _expiryTime;

    public LRUCache(IOptionsMonitor<CacheOptions> options)
    {
        _capacity = options.CurrentValue.Capacity;
        _expiryTime = options.CurrentValue.ExpiryTime;
        _map = new Dictionary<string, LRUNode>();
        _cache = new LRUDoubleLinkedList(_capacity);
    }


    public string? Get(string key)
    {
        if (!_map.ContainsKey(key))
            return null;

        LRUNode node = _map[key];
        var existingTime = DateTime.Now.Subtract(node.Created).Seconds;
        if (existingTime >= _expiryTime)
        {
            _cache.RemoveNode(node);
            _map.Remove(node.Key);

            return null;
        }

        _cache.MoveToHead(node);

        return node.Value;
    }

    public void Set(string key, string value)
    {
        if (_map.ContainsKey(key))
        {
            LRUNode node = _map[key];
            _cache.RemoveNode(node);
            node.Value = value;
            _cache.AddNode(node);
        }
        else
        {
            if (_cache.IsFull)
            {
                LRUNode removed = _cache.RemoveTail();
                _map.Remove(removed.Key);
            }

            LRUNode node = new(key, value);
            _cache.AddNode(node);
            _map[key] = node;
        }
    }
}
