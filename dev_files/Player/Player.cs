using Godot;
using System;

public class Player : KinematicBody2D, IDamageable
{
	[Signal] public delegate void Damaged();

	[Export] public PackedScene Weapon;
	[Export] public float MaxMoveSpeed = 50;
	[Export] public float MaxAcceleration = 75;
	[Export] public float DampingFactor = 0.1f;
	[Export] public float AngleDampingFactor = 0.1f;

	public IController BearController;
	public IController PenguinController;

	public Vector2 Velocity;

	private Sprite _bearSprite;
	private Sprite _penguinSprite;
	private Position2D _weaponSlot;
	private DebugGUI _debug;
	private AudioStreamPlayer2D _hurtSound;
	private AugmentedRandom _rand;

	public override void _Ready()
	{
		_bearSprite = GetNode<Sprite>("BearSprite");
		_penguinSprite = GetNode<Sprite>("PenguinSprite");
		_weaponSlot = GetNode<Position2D>("PenguinSprite/WeaponSlot");
		_debug = Globals.DebugGUI;
		_hurtSound = GetNode<AudioStreamPlayer2D>("HurtSound");
		_rand = Globals.Random;

		BearController = new ActionController()
		{
			Left = "player1_left",
			Right = "player1_right",
			Up = "player1_up",
			Down = "player1_down",
			Fire = "player1_fire",
		};

		var compositeController = new CompositeController();
		compositeController.Controllers.Add(new ActionController()
		{
			Left = "player2_left",
			Right = "player2_right",
			Up = "player2_up",
			Down = "player2_down",
			Fire = "player2_fire",
		});
		compositeController.Controllers.Add(new MouseController());

		PenguinController = compositeController;

		_debug.AddToGui<Player>(DebugPane.TopLeft, "Velocity", this, p => p.Velocity.ToString());
	}

	public override void _PhysicsProcess(float delta)
	{
		HandleBear(delta);

		HandlePenguin(delta);

		ApplyVelocity(delta);

		HandleDebug();
	}

	private void HandleDebug()
	{
		if (Input.IsActionJustPressed("debug_swap_controllers"))
		{
			SwapControllers();
		}
	}

	private void ApplyVelocity(float delta)
	{
		MoveAndCollide(Velocity * delta);

		Rotation = HandleRotation(delta);
	}

	private float HandleRotation(float delta)
	{
		// Rotate to change from looking sideways from the travel vector to straight on
		var lookDirection = Velocity.Rotated(Mathf.Pi / 2).Angle();

		var rotateAmount = Mathf.LerpAngle(Rotation, lookDirection, AngleDampingFactor * delta);
		return rotateAmount;
	}

	private void HandlePenguin(float delta)
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

	private void HandleBear(float delta)
	{
		var actions = BearController.GetInput(this);

		Velocity += actions.Direction * MaxAcceleration * delta;

		if(actions.Direction == Vector2.Zero)
		{
			// Provide some damping so he slows down
			Velocity = Velocity.LinearInterpolate(Vector2.Zero, DampingFactor * delta);

			// Cut it off so we don't keep super small numbers
			if(Velocity.Length() < 0.05)
			{
				Velocity = Vector2.Zero;
			}
		}

		// Make sure we don't break the speed limit
		if(Velocity.Length() > MaxMoveSpeed)
		{
			Velocity = Velocity.Normalized() * MaxMoveSpeed;
		}
	}

	public void SwapControllers()
	{
		var controller = BearController;
		BearController = PenguinController;
		PenguinController = controller;
	}

	public void Damage()
	{
		_hurtSound.PitchScale = _rand.Binomial() * 0.5f + 1;
		_hurtSound.Play();
		EmitSignal(nameof(Damaged));
	}
}
