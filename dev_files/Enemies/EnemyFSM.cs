using Godot;
using System;

public class EnemyFSM : AbstractStateMachine<Enemy>
{
	private float _timeToWander;
	private AugmentedRandom _rand;
	public Node2D Target;

	protected override void EnterState(string newState, string oldState)
	{
		switch (newState)
		{
			case "wander":
				_timeToWander = (float)_rand.NextDouble() * _parent.AverageWaitTime + _parent.AverageWaitTime / 2;
				break;

			case "surface":
				_parent.Anim.PlayBackwards("submerge");
				break;

			case "submerge":
				_parent.Anim.Play("submerge");
				break;

			case "im_hit":
				_parent.PlayDamageSound();
				if (_parent.AttackCollider.IsConnected("body_entered",
					this, nameof(OnAttackColliderBodyEntered)))
				{
					_parent.AttackCollider.Disconnect("body_entered",
						this, nameof(OnAttackColliderBodyEntered));
				}
				_parent.Velocity = _parent.Velocity.Rotated(Mathf.Pi / 2 * _rand.Sign());
				break;

			case "move_to_target":
				if (!_parent.AttackCollider.IsConnected("body_entered",
					this, nameof(OnAttackColliderBodyEntered)))
				{
					_parent.AttackCollider.Connect("body_entered",
						this, nameof(OnAttackColliderBodyEntered),
						flags: (uint)ConnectFlags.Oneshot);
				}
				break;

			case "avoid_left":
				_parent.Velocity = _parent.Velocity.Rotated(Mathf.Pi / 2 * -1);
				break;

			case "avoid_right":
				_parent.Velocity = _parent.Velocity.Rotated(Mathf.Pi / 2 * 1);
				break;

			default:
				break;
		}
	}

	protected override void ExitState(string oldState, string newState)
	{
		switch (oldState)
		{
			case "attack":
				_parent.Velocity *= -1f;
				break;
			default:
				break;
		}
	}

	protected override void Initialize()
	{
		_rand = Globals.Random;

		AddState("wander");
		AddState("surface");
		AddState("move_to_target");
		AddState("attack");
		AddState("im_hit");
		AddState("submerge");
		AddState("avoid");
		AddState("avoid_left");
		AddState("avoid_right");

		InitialState = "wander";

		var debug = Globals.DebugGUI;

		debug.AddToGui<EnemyFSM>(DebugPane.TopLeft, "Enemy State", this, e => e._state);
		if (Target != null)
		{
			debug.AddToGui<EnemyFSM>(DebugPane.TopLeft, "Distance To Player", this, e => (e._parent.Position - e.Target.Position).Length().ToString());
		}
	}

	protected override void ProcessState(float delta)
	{
		switch (_state)
		{
			case "wander":
				_timeToWander -= delta;
				// Have to add delta here to keep enemy from wigging out. Not sure if I want
				// to keep it here or push it back into the enemy code next to the velocity
				// getting multiplied by delta
				_parent.Rotation += _rand.Binomial() * _parent.RotateSpeed * delta;
				_parent.Velocity = _parent.MoveSpeed * Vector2.Up.Rotated(_parent.Rotation);

				break;

			case "move_to_target":
				var direction = (Target.Position - _parent.Position).Normalized();
				_parent.Velocity = direction * _parent.MoveSpeed;
				break;
			default:
				break;
		}
	}

	protected override string ProcessTransition(float delta)
	{
		// In any non-avoid state, check for obstacle avoidance
		// If we see either whisker have issues
		if (_state != null && !_state.StartsWith("avoid") &&
			(_parent.IsWhiskerColliding(Enemy.Whisker.FrontLeft)
				|| _parent.IsWhiskerColliding(Enemy.Whisker.FrontRight)))
		{
			return "avoid_left";
		}

		switch (_state)
		{
			case "wander":
				if (_timeToWander <= 0 && Target != null)
				{
					return "surface";
				}
				break;

			case "surface":
				return "move_to_target";


			case "move_to_target":
				if (_parent.IsCloseTo(Target.Position))
				{
					return "attack";
				}
				break;

			case "attack":
				return "submerge";


			case "submerge":
				return "wander";


			case "im_hit":
				return "submerge";


			case "avoid_left":
				if (!_parent.IsWhiskerColliding(Enemy.Whisker.Right))
				{
					return _previousState;
				}
				break;

			case "avoid_right":
				if (!_parent.IsWhiskerColliding(Enemy.Whisker.Left))
				{
					return _previousState;
				}
				break;

			default:
				break;
		}

		return _state;
	}

	public void OnAttackColliderBodyEntered(Node body)
	{
		_parent.Attack(body);
		SetState("attack");
	}
}
