using Godot;
using System;

public class HomeBase : Area2D
{
	[Signal] public delegate void PlayerEnteredHomeBase();

	public override void _Ready()
	{

	}

	private void OnHomeBaseBodyEntered(object body)
	{
		if (body is Player p)
		{
			EmitSignal(nameof(PlayerEnteredHomeBase));
		}
	}

}

