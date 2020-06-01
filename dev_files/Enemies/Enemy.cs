using Godot;
using System;

public class Enemy : KinematicBody2D
{
	[Export] public float CloseRange { get; set; } = 50f;
	[Export] public float MoveSpeed { get; set; } = 250f;

	public Vector2 Velocity { get; set; }
	public EnemyFSM Brain { get; set; }

	public override void _Ready()
	{
		Brain = GetNode<EnemyFSM>("Brain");
	}

	public override void _PhysicsProcess(float delta)
	{
		var collision = MoveAndCollide(Velocity * delta);
	}

	public bool IsCloseTo(Vector2 position)
	{
		return (position - Position).Length() <= CloseRange;
	}

	public void Attack()
	{
		GD.Print("I'm Attacking!");
	}
}
