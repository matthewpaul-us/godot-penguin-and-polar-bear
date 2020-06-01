using Godot;
using System;

public class Player : KinematicBody2D
{

	[Export] public PackedScene Weapon;
	[Export] public float MoveSpeed = 50;

	public IController BearController;
	public IController PenguinController;

	public Vector2 Velocity;

	private Sprite _bearSprite;
	private Sprite _penguinSprite;
	private Position2D _weaponSlot;

	public override void _Ready()
	{
		_bearSprite = GetNode<Sprite>("BearSprite");
		_penguinSprite = GetNode<Sprite>("PenguinSprite");
		_weaponSlot = GetNode<Position2D>("PenguinSprite/WeaponSlot");

		BearController = new ActionController();
		PenguinController = new MouseController();
	}

	public override void _PhysicsProcess(float delta)
	{
		HandleBear();

		HandlePenguin();

		ApplyVelocity(delta);
	}

	private void ApplyVelocity(float delta)
	{
		MoveAndCollide(Velocity * delta);

		// Rotate to change from looking sideways from the travel vector to straight on
		_bearSprite.LookAt(GlobalPosition + Velocity.Rotated(Mathf.Pi / 2));
	}

	private void HandlePenguin()
	{
		var actions = PenguinController.GetInput(this);

		_penguinSprite.LookAt(GlobalPosition + actions.Direction.Rotated(Mathf.Pi / 2));

		if(actions.IsFirePressed)
		{
			GD.Print("FIRE!");
			FireWeapon();
		}
	}

	public void FireWeapon()
	{
		var weapon = Weapon.Instance() as KinematicBody2D;
		weapon.Position = _weaponSlot.GlobalPosition;
		weapon.Rotation = _weaponSlot.GlobalRotation;

		GetParent().AddChild(weapon);
	}

	private void HandleBear()
	{
		var actions = BearController.GetInput(this);

		Velocity = actions.Direction.Normalized() * MoveSpeed;
	}
}
