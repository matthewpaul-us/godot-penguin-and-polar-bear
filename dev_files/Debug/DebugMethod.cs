using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DebugMethod : IDebugField
{
    public string Name { get; set; }
    private Func<string> _getter;
    public DebugMethod(string name, Func<string> getter)
    {
        Name = name;
        _getter = getter;
    }
    public string GetField()
    {
        return $"{Name}: {_getter()}";
    }
}
