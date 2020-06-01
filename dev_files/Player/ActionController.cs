using Godot;
using System;

public class ActionController : IController
{
    public string Left { get; internal set; }
    public string Right { get; internal set; }
    public string Up { get; internal set; }
    public string Down { get; internal set; }
    public string Fire { get; internal set; }

    public ActionSet GetInput(Node2D parent)
    {
        var set = new ActionSet();
        var direction = Vector2.Zero;

        direction.x = Input.GetActionStrength(Right) - Input.GetActionStrength(Left);
        direction.y = Input.GetActionStrength(Down) - Input.GetActionStrength(Up);

        set.Direction = direction;

        set.IsFirePressed = Input.IsActionJustPressed(Fire);

        return set;
    }

}