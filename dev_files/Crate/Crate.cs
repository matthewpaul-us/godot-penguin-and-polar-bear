using Godot;
using System;

public class Crate : Area2D, IDamageable
{
	[Signal] public delegate void Collected(Crate crate);

	[Export] public bool IsFloating { get; set; }
	[Export] public float HitMove { get; set; }
	[Export] public Texture CarryTexture { get; set; }
	[Export] public PackedScene FloatingVariant { get; set; }

	public Player Player;

	public override void _Ready()
	{
		if (IsFloating)
		{
			GetNode<AnimationPlayer>("AnimationPlayer").Play("float");
		}
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

	public void Damage()
	{
		// Need a shake, use elastic or bounce ease

		if (Player != null)
		{
			GlobalPosition += (GlobalPosition - Player.GlobalPosition).Normalized() * HitMove;
		}
	}

	public void OnCrateBodyExited(Node body)
	{
		if (body == GetParent())
		{
			Hide();

			var floatingCrate = FloatingVariant.Instance() as Crate;
			floatingCrate.GlobalPosition = this.GlobalPosition;
			floatingCrate.Scale = Vector2.One * 0.43f;
			floatingCrate.Connect(nameof(Crate.Collected), Player, nameof(Player.OnCrateCollected));

			GetTree().Root.AddChild(floatingCrate);

			CallDeferred("queue_free");
		}
	}

}

