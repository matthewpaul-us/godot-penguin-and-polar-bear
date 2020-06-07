using Godot;
using System;

public class WalrusFSM : AbstractStateMachine<Walrus>
{
	[Export] public float ReloadTime;
	[Export] public float StunTime;
	[Export] public float AttackDistance;

	public Node2D Target;

	private float _timeToReload;
	private float _timeUntilStunWearsOff;
	private DebugGUI _debug;

	protected override void EnterState(string newState, string oldState)
	{
		switch (newState)
		{
			case "wait":
				_parent.CancelReload();
				break;

			case "attack":
				_parent.Attack();
				break;

			case "reload":
				_timeToReload = ReloadTime;
				_parent.StartReload();
				break;

			case "stunned":
				_timeUntilStunWearsOff = StunTime;
				_parent.Anim.Play("stunned");
				_parent.CancelReload();
				break;

			default:
				break;
		}
	}

	protected override void ExitState(string oldState, string newState)
	{
		if (oldState == "stunned")
		{
			_parent.Anim.Play("wait");
		}
	}

	protected override void Initialize()
	{
		GD.Print("Initialize");
		AddState("wait");
		AddState("attack");
		AddState("reload");
		AddState("stunned");

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

			case "stunned":
				_timeUntilStunWearsOff -= delta;
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
				if (IsInRange())
				{
					return "reload";
				}
				break;

			case "attack":
				if (!_parent.Anim.IsPlaying())
				{
					return "reload";
				}
				break;


			case "reload":
				if (!IsInRange())
				{
					return "wait";
				}

				if (_timeToReload <= 0)
				{
					return "attack";
				}
				break;

			case "stunned":
				if (_timeUntilStunWearsOff <= 0)
				{
					if (IsInRange())
					{
						return "reload";
					}
					else
					{
						return "wait";
					}
				}
				break;

			default:
				break;
		}

		return _state;
	}

	private bool IsInRange()
	{
		return (_parent.GlobalPosition - Target.GlobalPosition).Length() <= AttackDistance;
	}
}
