using Godot;
using System;
using System.Diagnostics;

public class Walrus : KinematicBody2D, IDamageable
{
	[Export] public PackedScene Weapon;
	[Export] public Vector2 TargetPosition;
	public AnimatedSprite Sprite;
	public WalrusFSM Brain;

	private AnimationPlayer _anim;
	private Position2D _weaponSlot;
	private AudioStreamPlayer2D _attackSound;

	public override void _Ready()
	{
		Sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_attackSound = GetNode<AudioStreamPlayer2D>("AttackSound");
		_weaponSlot = GetNode<Position2D>("WeaponSlot");
		Brain = GetNode<WalrusFSM>("Brain");
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
		throw new NotImplementedException();
	}

	public void Attack()
	{
		_anim.Play("attack");
	}

	public void Throw()
	{
		var weapon = Weapon.Instance() as IceBall;

		weapon.Position = _weaponSlot.GlobalPosition;
		weapon.Rotation = _weaponSlot.GlobalRotation;
		weapon.TargetPosition = Brain.Target.GlobalPosition;

		GetParent().AddChild(weapon);

		_attackSound.PitchScale = Globals.Random.Binomial() * 0.3f + 1;
		_attackSound.Play();
	}
}
