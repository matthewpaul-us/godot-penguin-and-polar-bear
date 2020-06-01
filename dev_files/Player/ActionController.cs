using Godot;
using System;

public class ActionController : IController
{
    public ActionSet GetInput(Node2D parent)
    {
        var set = new ActionSet();
        var direction = Vector2.Zero;

        direction.x = Input.GetActionStrength("player1_right") - Input.GetActionStrength("player1_left");
        direction.y = Input.GetActionStrength("player1_down") - Input.GetActionStrength("player1_up");

        set.Direction = direction;

        set.IsFirePressed = Input.IsActionJustPressed("player1_fire");

        return set;
    }

}