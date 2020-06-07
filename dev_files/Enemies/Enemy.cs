using Godot;
using System;

[Flags]
public enum Whiskers
{
	None = 0,
	FrontLeft = 1 << 0,
	FrontRight = 1 << 1,
	BackLeft = 1 << 2,
	BackRight = 1 << 3,
}

public class Enemy : KinematicBody2D, IDamageable
{
	[Export] public float CloseRange { get; set; } = 100;
	[Export] public float MoveSpeed { get; set; } = 250f;
	[Export] public float RotateSpeed { get; set; } = Mathf.Pi;
	[Export] public float AverageWaitTime { get; set; } = 10f;
	[Export] public float AngleDampingFactor { get; set; } = 3f;
	[Export] public Color HurtColor { get; set; }

	public Vector2 Velocity { get; set; }
	public EnemyFSM Brain { get; set; }

	public AnimatedSprite Sprite;
	public Area2D AttackCollider;
	public AnimationPlayer Anim;
	private AudioStreamPlayer2D _hitSound;
	private RayCast2D _frontLeftWhisker;
	private RayCast2D _frontRightWhisker;
	private RayCast2D _backLeftWhisker;
	private RayCast2D _backRightWhisker;
	private RayCast2D _backLeftWhisker2;
	private RayCast2D _backRightWhisker2;
	private RayCast2D _backLeftWhisker3;
	private RayCast2D _backRightWhisker3;
	private Tween _tween;

	public override void _Ready()
	{
		Brain = GetNode<EnemyFSM>("Brain");
		Sprite = GetNode<AnimatedSprite>("Sprite");
		AttackCollider = GetNode<Area2D>("AttackCollider");
		Anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_hitSound = GetNode<AudioStreamPlayer2D>("HitSound");

		_frontLeftWhisker = GetNode<RayCast2D>("Whiskers/FrontLeftWhisker");
		_frontRightWhisker = GetNode<RayCast2D>("Whiskers/FrontRightWhisker");
		_backLeftWhisker = GetNode<RayCast2D>("Whiskers/BackLeftWhisker");
		_backRightWhisker = GetNode<RayCast2D>("Whiskers/BackRightWhisker");
		_backLeftWhisker2 = GetNode<RayCast2D>("Whiskers/BackLeftWhisker2");
		_backRightWhisker2 = GetNode<RayCast2D>("Whiskers/BackRightWhisker2");
		_backLeftWhisker3 = GetNode<RayCast2D>("Whiskers/BackLeftWhisker3");
		_backRightWhisker3 = GetNode<RayCast2D>("Whiskers/BackRightWhisker3");

		_tween = GetNode<Tween>("Tween");
	}

	public override void _PhysicsProcess(float delta)
	{
		Rotation = HandleRotation(delta);

		var collision = MoveAndCollide(Velocity * delta);
	}

	private float HandleRotation(float delta)
	{
		// Rotate to change from looking sideways from the travel vector to straight on
		var lookDirection = Velocity.Rotated(Mathf.Pi / 2).Angle();

		var rotateAmount = Mathf.LerpAngle(Rotation, lookDirection, AngleDampingFactor * delta);
		return rotateAmount;
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
		_tween.InterpolateProperty(Sprite, "modulate",
			HurtColor, Colors.White, 1,
			Tween.TransitionType.Cubic, Tween.EaseType.Out);
		_tween.Start();
		Brain.SetState("im_hit");
	}

	public void PlayDamageSound()
	{
		_hitSound.Play();
	}

	public enum Whisker
	{
		FrontLeft,
		FrontRight,
		Left,
		Right,
	}

	public bool IsWhiskerColliding(Whisker whisker)
	{
		switch (whisker)
		{
			case Whisker.FrontLeft:
				return _frontLeftWhisker.IsColliding();

			case Whisker.FrontRight:
				return _frontRightWhisker.IsColliding();

			case Whisker.Left:
				_backLeftWhisker.ForceRaycastUpdate();
				_backLeftWhisker2.ForceRaycastUpdate();
				_backLeftWhisker3.ForceRaycastUpdate();
				return _backLeftWhisker.IsColliding()
					|| _backLeftWhisker2.IsColliding()
					|| _backLeftWhisker3.IsColliding();

			case Whisker.Right:
				_backRightWhisker.ForceRaycastUpdate();
				_backRightWhisker2.ForceRaycastUpdate();
				_backRightWhisker3.ForceRaycastUpdate();
				return _backRightWhisker.IsColliding()
					|| _backRightWhisker2.IsColliding()
					|| _backRightWhisker3.IsColliding();

		}

		return false;
	}

	public Whiskers GetCollidingWhiskers()
	{
		var whiskers = Whiskers.None;

		if (_frontLeftWhisker.IsColliding())
		{
			whiskers |= Whiskers.FrontLeft;
		}

		if (_frontRightWhisker.IsColliding())
		{
			whiskers |= Whiskers.FrontRight;
		}

		if (_backLeftWhisker.IsColliding())
		{
			whiskers |= Whiskers.BackLeft;
		}

		if (_backRightWhisker.IsColliding())
		{
			whiskers |= Whiskers.BackRight;
		}

		return whiskers;
	}
}


