using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Globals : Node
{
    public static AugmentedRandom Random { get; private set; }
    public override void _Ready()
    {
        Random = new AugmentedRandom();
    }
}
