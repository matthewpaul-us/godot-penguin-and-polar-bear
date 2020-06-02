using Godot;
using System;

public class Enemy : KinematicBody2D, IDamageable
{
	[Export] public float CloseRange { get; set; } = 100;
	[Export] public float MoveSpeed { get; set; } = 250f;
	[Export] public float RotateSpeed { get; set; } = Mathf.Pi;

	public Vector2 Velocity { get; set; }
	public EnemyFSM Brain { get; set; }

	private Sprite _sprite;
	public Area2D AttackCollider;
	public AnimationPlayer Anim;
	private AudioStreamPlayer2D _hitSound;

	public override void _Ready()
	{
		Brain = GetNode<EnemyFSM>("Brain");
		_sprite = GetNode<Sprite>("Sprite");
		AttackCollider = GetNode<Area2D>("AttackCollider");
		Anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_hitSound = GetNode<AudioStreamPlayer2D>("HitSound");
	}

	public override void _PhysicsProcess(float delta)
	{
		// Rotate to change from looking sideways from the travel vector to straight on
		LookAt(GlobalPosition + Velocity.Rotated(Mathf.Pi / 2));

		var collision = MoveAndCollide(Velocity * delta);
	}

	public bool IsCloseTo(Vector2 position)
	{
		return (position - Position).Length() <= CloseRange;
	}

	public void Attack(Node body)
	{
		if (body is IDamageable damageable)
		{
			damageable.Damage();
		}
	}

	public void Damage()
	{
		Brain.SetState("im_hit");
	}

	public void PlayDamageSound()
	{
		_hitSound.Play();
	}
}


