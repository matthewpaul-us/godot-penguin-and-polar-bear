using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MouseController : IController
{
    public ActionSet GetInput(Node2D parent)
    {
        var set = new ActionSet();

        set.Direction = parent.GetViewport().GetMousePosition() - parent.GlobalPosition;
        set.IsFirePressed = Input.IsActionJustPressed("player2_fire");
        return set;
    }
}
