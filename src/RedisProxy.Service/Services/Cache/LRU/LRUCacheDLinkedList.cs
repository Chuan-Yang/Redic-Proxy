namespace RedisProxy.Service.Services.Cache.LRU;

public class LRUDoubleLinkedList
{
    private readonly LRUNode _head;
    private readonly LRUNode _tail;

    private int _size;
    private readonly int _capacity;

    public int GetSize => _size;
    public int GetCapacity => _capacity;
    public bool IsFull => _size == _capacity;

    public LRUDoubleLinkedList(int capacity)
    {
        _head = new LRUNode();
        _tail = new LRUNode();

        _head.Next = _tail;
        _tail.Previous = _head;

        _size = 0;
        _capacity = capacity;
    }

    public void AddNode(LRUNode node)
    {
        // Always add new node to the head
        node.Previous = _head;
        node.Next = _head.Next;

        _head.Next = node;
        _head.Next.Previous = node;

        _size++;
    }

    public void RemoveNode(LRUNode node)
    {
        node.Previous.Next = node.Next;
        node.Next.Previous = node.Previous;

        _size--;
    }

    public void MoveToHead(LRUNode node)
    {
        // Move a node to the head
        RemoveNode(node);
        AddNode(node);
    }

    public LRUNode RemoveTail()
    {
        var tail = _tail.Previous;
        RemoveNode(tail);

        return tail;
    }
}
