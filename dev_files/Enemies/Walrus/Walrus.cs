using Godot;
using System;
using System.Diagnostics;

public class Walrus : KinematicBody2D, IDamageable
{
	[Export] public PackedScene Weapon;
	[Export] public Vector2 TargetPosition;
	[Export] public Color HurtColor;
	public AnimatedSprite Sprite;
	public WalrusFSM Brain;

	public AnimationPlayer Anim;
	private Position2D _weaponSlot;
	private AudioStreamPlayer2D _attackSound;
	private AudioStreamPlayer2D _hurtSound;
	private Tween _tween;

	public override void _Ready()
	{
		Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_attackSound = GetNode<AudioStreamPlayer2D>("AttackSound");
		_hurtSound = GetNode<AudioStreamPlayer2D>("HurtSound");
		_weaponSlot = GetNode<Position2D>("WeaponSlot");
		Brain = GetNode<WalrusFSM>("Brain");
		_tween = GetNode<Tween>("Tween");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionJustPressed("debug_swap_avatars"))
		{
			Attack();
		}
	}
	public void Damage()
	{
		_hurtSound.PitchScale = Globals.Random.Binomial(0.5f) + 1;
		_hurtSound.Play();

		_tween.InterpolateProperty(Sprite, "modulate",
			HurtColor, Colors.White, 1,
			Tween.TransitionType.Cubic, Tween.EaseType.Out);
		_tween.Start();

		Brain.SetState("stunned");
	}

	public void Attack()
	{
		Anim.Play("attack");
	}

	public void Throw()
	{
		var weapon = Weapon.Instance() as IceBall;

		weapon.Position = _weaponSlot.GlobalPosition;
		weapon.Rotation = _weaponSlot.GlobalRotation;
		weapon.TargetPosition = Brain.Target.GlobalPosition;

		GetTree().Root.AddChild(weapon);

		_attackSound.Play();
	}
}
