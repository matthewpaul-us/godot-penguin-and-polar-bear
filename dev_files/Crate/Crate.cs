using Godot;
using System;

public class Crate : Area2D
{
	[Signal] public delegate void Collected(Crate crate);

	[Export] public Texture CarryTexture { get; set; }

	public override void _Ready()
	{

	}

	private void OnCrateBodyEntered(object body)
	{
		if(body is Player p)
		{
			Collect();
		}
	}

	public void Collect()
	{
		Hide();
		EmitSignal(nameof(Collected), this);
		CallDeferred("queue_free");
	}

}

