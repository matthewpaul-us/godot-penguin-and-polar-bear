using Godot;
using System;

public class WalrusFSM : AbstractStateMachine<Walrus>
{
	[Export] public float ReloadTime;
	[Export] public float AttackDistance;

	public Node2D Target;

	private float _timeToReload;
	private DebugGUI _debug;

	protected override void EnterState(string newState, string oldState)
	{
		switch (newState)
		{
			case "attack":
				_parent.Attack();
				break;

			case "reload":
				_timeToReload = ReloadTime;
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
		GD.Print("Initialize");
		AddState("wait");
		AddState("attack");
		AddState("reload");

		InitialState = "wait";

		_debug = Globals.DebugGUI;
		_debug.AddToGui<WalrusFSM>(DebugPane.TopLeft, "Walrus State", this, e => e._state);
		if (Target != null)
		{
			_debug.AddToGui<WalrusFSM>(DebugPane.TopLeft, "Distance To Player", this, e => (e._parent.Position - e.Target.Position).Length().ToString());
		}
	}

	protected override void ProcessState(float delta)
	{
		switch (_state)
		{
			case "reload":
				_timeToReload -= delta;
				break;
			default:
				break;
		}
	}

	protected override string ProcessTransition(float delta)
	{
		switch (_state)
		{
			case "wait":
				if ((_parent.GlobalPosition - Target.GlobalPosition).Length() <= AttackDistance)
				{
					return "reload";
				}
				break;

			case "attack":
				return "reload";


			case "reload":
				if ((_parent.GlobalPosition - Target.GlobalPosition).Length() > AttackDistance)
				{
					return "wait";
				}

				if (_timeToReload <= 0)
				{
					return "attack";
				}
				break;

			default:
				break;
		}

		return _state;
	}
}
