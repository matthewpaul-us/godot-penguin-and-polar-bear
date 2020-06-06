using Godot;
using System;

public class Player : KinematicBody2D, IDamageable
{
	[Signal] public delegate void Damaged();
	[Signal] public delegate void HealthChanged();
	[Signal] public delegate void CratePickedUp();
	[Signal] public delegate void CrateDroppedOff();

	[Export] public int Health;
	[Export] public bool IsControllerSwitchingActive;
	[Export] public PackedScene Weapon;
	[Export] public float MaxMoveSpeed = 75;
	[Export] public float MaxAcceleration = 100;
	[Export] public float CratedMaxMoveSpeed = 50;
	[Export] public float CratedMaxAcceleration = 75;
	[Export] public Color HurtColor;


	[Export] public float DampingFactor = 0.1f;
	[Export] public float AngleDampingFactor = 0.1f;

	public IController BearController;
	public IController PenguinController;

	public Vector2 Velocity;
	public bool IsCarryingCrate = false;
	public Camera2D Camera;

	private AnimatedSprite _bearSprite;
	private AnimatedSprite _penguinSprite;
	private Sprite _crateSprite;
	private Position2D _weaponSlot;
	private DebugGUI _debug;
	private Tween _tween;


	private AudioStreamPlayer2D _hurtSound;
	private AudioStreamPlayer2D _penguinAttackSound;
	private AugmentedRandom _rand;
	private AnimationPlayer _bearAnim;

	public override void _Ready()
	{
		_bearSprite = GetNode<AnimatedSprite>("BearSprite");
		_penguinSprite = GetNode<AnimatedSprite>("PenguinSprite");
		_weaponSlot = GetNode<Position2D>("PenguinSprite/WeaponSlot");
		_debug = Globals.DebugGUI;
		_hurtSound = GetNode<AudioStreamPlayer2D>("HurtSound");
		_penguinAttackSound = GetNode<AudioStreamPlayer2D>("PenguinAttackSound");
		_rand = Globals.Random;
		_bearAnim = GetNode<AnimationPlayer>("BearAnimationPlayer");
		_crateSprite = GetNode<Sprite>("BearSprite/CrateSprite");
		Camera = GetNode<Camera2D>("PlayerCamera");
		_tween = GetNode<Tween>("Tween");

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

		if (!Globals.IsGamepad)
		{
			compositeController.Controllers.Add(new MouseController());
		}

		PenguinController = compositeController;

		_debug.AddToGui<Player>(DebugPane.TopLeft, "Velocity", this, p => p.Velocity.ToString());
		_debug.AddToGui<Player>(DebugPane.TopLeft, "Health", this, p => p.Health.ToString());
	}

	public override void _PhysicsProcess(float delta)
	{
		HandleBear(delta);

		HandlePenguin(delta);

		ApplyVelocity(delta);

		HandleDebug();
	}

	public void OnHomeBaseEntered()
	{
		if (IsCarryingCrate)
		{
			UnloadCrate();
		}
	}

	public void OnCrateCollected(Crate crate)
	{
		_crateSprite.Texture = crate.CarryTexture;
		IsCarryingCrate = true;
		EmitSignal(nameof(CratePickedUp));
	}

	public void UnloadCrate()
	{
		_crateSprite.Texture = null;
		IsCarryingCrate = false;
		EmitSignal(nameof(CrateDroppedOff));
	}

	private void HandleDebug()
	{
		if (Input.IsActionJustPressed("debug_swap_controllers"))
		{
			SwapControllers();
		}

		if (Input.IsActionJustPressed("ui_select"))
		{
			ShowHurt();
		}
	}

	private void ApplyVelocity(float delta)
	{
		MoveAndCollide(Velocity * delta);

		Rotation = HandleRotation(delta);

		if (Velocity.Length() > 10f)
		{
			_bearAnim.Play("swim");
		}
		else
		{
			_bearAnim.Play("idle");
		}
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

		_penguinSprite.LookAt(_penguinSprite.GlobalPosition + actions.Direction.Rotated(Mathf.Pi / 2));

		if(actions.IsFirePressed)
		{
			GD.Print("FIRE!");
			FireWeapon();
		}
	}

	public void FireWeapon()
	{
		var weapon = Weapon.Instance() as Snowball;
		weapon.Position = _weaponSlot.GlobalPosition;
		weapon.Rotation = _weaponSlot.GlobalRotation;

		if (weapon.AttackSound != null)
		{
			_penguinAttackSound.Stream = weapon.AttackSound;
			_penguinAttackSound.PitchScale = _rand.Binomial() * 0.3f + 1;
			_penguinAttackSound.Play();
		}

		GetParent().AddChild(weapon);
	}

	private void HandleBear(float delta)
	{
		var actions = BearController.GetInput(this);

		Velocity += actions.Direction * (IsCarryingCrate? CratedMaxAcceleration : MaxAcceleration) * delta;

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
		if(Velocity.Length() > (IsCarryingCrate ? CratedMaxMoveSpeed : MaxMoveSpeed))
		{
			Velocity = Velocity.Normalized() * (IsCarryingCrate ? CratedMaxMoveSpeed : MaxMoveSpeed);
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
		ShowHurt();

		EmitSignal(nameof(Damaged));
		ChangeHealth(Health - 1);

		if (IsControllerSwitchingActive)
		{
			SwapControllers();
		}
	}

	public void ShowHurt()
	{
		_hurtSound.PitchScale = _rand.Binomial() * 0.5f + 1;
		_hurtSound.Play();

		Camera.GetNode<ScreenShake>("ScreenShake").Start(0.2f, 30, 20);

		_tween.InterpolateProperty(_bearSprite, "modulate",
			HurtColor, Colors.White, 1,
			Tween.TransitionType.Cubic, Tween.EaseType.Out);
		_tween.Start();
	}

	public void ChangeHealth(int newHealth)
	{
		Health = newHealth;
		EmitSignal(nameof(HealthChanged), newHealth);
	}
}
