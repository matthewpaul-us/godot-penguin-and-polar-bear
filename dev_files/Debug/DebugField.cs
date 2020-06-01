using System;

public interface IDebugField
{
    string GetField();
}

public class DebugField<T> : IDebugField
{
    public string Name { get; set; }
    public T Node { get; set; }
    private Func<T, string> _getter;

    public DebugField(string name, T node, Func<T, string> getter)
    {
        Name = name;
        Node = node;
        _getter = getter;
    }

    public string GetField()
    {
        return $"{Name}: {_getter(Node)}";
    }
}