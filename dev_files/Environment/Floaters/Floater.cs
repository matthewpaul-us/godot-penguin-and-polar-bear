using Godot;
using System;

public class Floater : KinematicBody2D
{
	public Vector2 Velocity { get; set; }
	[Export] public float MaxVelocity { get; set; } = 50;
	[Export] public float Weight { get; set; }
	[Export] public float VelocityDampingFactor { get; set; }

	private AnimationPlayer _anim;
	private AudioStreamPlayer2D _touchSound;
	public override void _Ready()
	{
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_touchSound = GetNode<AudioStreamPlayer2D>("TouchSound");
	}

	public override void _PhysicsProcess(float delta)
	{
		var collision = MoveAndCollide(Velocity * delta);

		if (collision != null)
		{
			if (_touchSound.Stream != null && !_touchSound.Playing)
			{
				_touchSound.PitchScale = 1f + Globals.Random.Binomial(0.3f);
				_touchSound.Play();
			}

			Velocity = collision.ColliderVelocity / Weight;
			_anim.Play("wiggle");

			if(Velocity.Length() > MaxVelocity)
			{
				Velocity = Velocity.Normalized() * MaxVelocity;
			}
		}

		Velocity = Velocity.LinearInterpolate(Vector2.Zero, VelocityDampingFactor * delta);

		// Cut it off so we don't keep super small numbers
		if (Velocity.Length() < 0.05)
		{
			Velocity = Vector2.Zero;
		}
	}


}
