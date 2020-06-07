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
	private Sprite _sprite;


	public override void _Ready()
	{
		_lifespanTimer = GetNode<Timer>("LifespanTimer");
		_collider = GetNode<CollisionShape2D>("CollisionShape2D");
		_hitSound = GetNode<AudioStreamPlayer2D>("HitSound");
		_sprite = GetNode<Sprite>("Sprite");
		_rand = Globals.Random;

		// Rotate the up vector by the objects rotation to get the
		//   forward vector
		Velocity = Vector2.Up.Rotated(Rotation) * StartingSpeed;

		_lifespanTimer.Start(TimeToLive);
	}

	public override void _PhysicsProcess(float delta)
	{
		float scale = Mathf.Lerp(0, 1, _lifespanTimer.TimeLeft / _lifespanTimer.WaitTime);
		_sprite.Scale = new Vector2(scale, scale);

		KinematicCollision2D collision = MoveAndCollide(Velocity * delta);

		if (collision != null && collision.Collider is Node collider)
		{
			GD.Print($"We hit {collider.Name}");

			PlayHit();

			Velocity = Vector2.Zero;

			_collider.Disabled = true;

			KillProjectile();

			if (collider is IDamageable damageable)
			{
				damageable.Damage();
			}
			else if (collider.GetParent() is IDamageable damageableParent)
			{
				damageableParent.Damage();
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


