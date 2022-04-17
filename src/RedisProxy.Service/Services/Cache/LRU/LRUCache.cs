namespace RedisProxy.Service.Services.Cache.LRU;

public class LRUCache : ICache
{
    private readonly Dictionary<string, LRUNode> _map;
    private readonly LRUDoubleLinkedList _cache;
    public LRUCache()
    {
        _map = new Dictionary<string, LRUNode>();
        _cache = new LRUDoubleLinkedList(2);
    }


    public string? Get(string key)
    {
        if (!_map.ContainsKey(key))
            return null;

        LRUNode node = _map[key];
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
