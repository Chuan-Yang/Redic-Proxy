namespace RedisProxy.Service.Services.Cache.LRU;
public class LRUNode
{
	public string? Key { get; set; }
	public string? Value { get; set; }
    public DateTime Created { get; set; }
	public LRUNode? Previous { get; set; }
	public LRUNode? Next { get; set; }

    // For dummy node
	public LRUNode()
    {
        Created = DateTime.Now;
    }

	public LRUNode(string k, string v) : this()
	{
		Key = k;
		Value = v;
	}
}
