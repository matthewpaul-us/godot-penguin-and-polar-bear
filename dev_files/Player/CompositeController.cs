using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CompositeController : IController
{
    public List<IController> Controllers;

    public CompositeController()
    {
        Controllers = new List<IController>();
    }

    public ActionSet GetInput(Node2D parent)
    {
        var set = new ActionSet();

        var inputs = Controllers.Select(c => c.GetInput(parent));

        foreach (var input in inputs)
        {
            if (set.Direction == Vector2.Zero)
            {
                set.Direction += input.Direction;
            }

            set.IsFirePressed = set.IsFirePressed || input.IsFirePressed;
        }

        return set;
    }
}
