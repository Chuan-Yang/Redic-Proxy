namespace RedisProxy.Service.Services.Cache.LRU;
public class LRUNode
{
	public string? Key { get; set; }
	public string? Value { get; set; }
	public LRUNode? Previous { get; set; }
	public LRUNode? Next { get; set; }

    // For dummy node
	public LRUNode()
    {

    }

	public LRUNode(string k, string v)
	{
		Key = k;
		Value = v;
	}
}
