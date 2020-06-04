using Godot;
using System;

public class Snowball : KinematicBody2D
{
	[Export] public float StartingSpeed = 200f;
	[Export] public float TimeToLive = 3f;
	[Export] public AudioStream AttackSound;
	/// <summary>
	/// The absolute amount from 100% the sound pitch can range.
	/// </summary>
	[Export] float PitchRange = 0.3f;
	public Vector2 Velocity { get; set; }

	private Timer _lifespanTimer;
	private CollisionShape2D _collider;
	private AudioStreamPlayer2D _hitSound;
	private AugmentedRandom _rand;


	public override void _Ready()
	{
		_lifespanTimer = GetNode<Timer>("LifespanTimer");
		_collider = GetNode<CollisionShape2D>("CollisionShape2D");
		_hitSound = GetNode<AudioStreamPlayer2D>("HitSound");
		_rand = Globals.Random;

		// Rotate the up vector by the objects rotation to get the
		//   forward vector
		Velocity = Vector2.Up.Rotated(Rotation) * StartingSpeed;

		_lifespanTimer.Start(TimeToLive);
	}

	public override void _PhysicsProcess(float delta)
	{
		KinematicCollision2D collision = MoveAndCollide(Velocity * delta);

		if (collision != null)
		{
			GD.Print($"We hit {((Node)collision.Collider).Name}");

			PlayHit();

			Velocity = Vector2.Zero;

			_collider.Disabled = true;

			KillProjectile();

			if (collision.Collider is IDamageable damageable)
			{
				damageable.Damage();
			}
		}
	}

	public void PlayHit()
	{
		_hitSound.PitchScale = 1f + (float)_rand.NextDouble() * PitchRange - PitchRange / 2;
		_hitSound.Play();
	}

	public void KillProjectile()
	{
		Hide();
	}

	private void OnLifespanTimerTimeout()
	{
		KillProjectile();
		CallDeferred(nameof(QueueFree));
	}
}


