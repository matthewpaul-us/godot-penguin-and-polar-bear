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
				_timeToWander = (float)_rand.NextDouble() * 3f + 2f;
				break;

			case "surface":
				GD.Print("Surfacing!");
				break;

			case "attack":
				_parent.Attack();
				break;

			default:
				break;
		}
	}

	protected override void ExitState(string oldState, string newState)
	{
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

		InitialState = "wander";

		var debug = Globals.DebugGUI;

		debug.AddToGui<EnemyFSM>(DebugPane.TopLeft, "Enemy State", this, e => e._state);
		debug.AddToGui<EnemyFSM>(DebugPane.TopLeft, "Enemy TimeToWander", this, e => e._timeToWander.ToString());
		debug.AddToGui<EnemyFSM>(DebugPane.TopLeft, "Distance To Player", this, e => (e._parent.Position - e.Target.Position).Length().ToString());
	}

	protected override void ProcessState(float delta)
	{
		switch (_state)
		{
			case "wander":
				_timeToWander -= delta;
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
				break;

			case "move_to_player":
				if (_parent.IsCloseTo(Target.Position))
				{
					return "attack";
				}
				break;

			case "attack":
				return "wander";
				break;

			default:
				break;
		}

		return _state;
	}
}
