using Godot;
using System;

public class IceBall : KinematicBody2D
{
	[Export] public float MoveSpeed;
	public Vector2 TargetPosition;

	public Vector2 Velocity;

	private CollisionShape2D _collider;
	private AudioStreamPlayer2D _hitSound;

	public override void _Ready()
	{
		_collider = GetNode<CollisionShape2D>("CollisionShape2D");
		_hitSound = GetNode<AudioStreamPlayer2D>("HitSound");

	}

	public override void _PhysicsProcess(float delta)
	{
		Velocity = (TargetPosition - GlobalPosition).Normalized() * MoveSpeed;

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

		if ((GlobalPosition - TargetPosition).Length() <= 25f)
		{
			KillProjectile();
		}
	}

	private void KillProjectile()
	{
		Hide();
		CallDeferred("queue_free");
	}

	private void PlayHit()
	{
		_hitSound.PitchScale = 0.8f + (float)Globals.Random.NextDouble() * 0.3f - 0.3f / 2;
		_hitSound.Play();
	}
}
