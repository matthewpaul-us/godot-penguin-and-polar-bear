using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

public abstract class AbstractStateMachine<T>: Node where T : Node
{
    [Signal] delegate void StateChanged(string state);

    [Export] public string InitialState;

    protected T _parent;

    protected HashSet<string> _states;
    protected string _state;
    public string State { get { return _state; } }
    protected string _previousState;

    public override void _Ready()
    {
        _parent = GetParent<T>();
        _states = new HashSet<string>();

        Initialize();

        CallDeferred(nameof(SetInitialState));
    }

    private void SetInitialState()
    {
        SetState(InitialState);
    }

    public void AddState(string newState)
    {
        _states.Add(newState);
    }

    public override void _PhysicsProcess(float delta)
    {
        if(_parent == null)
        {
            return;
        }

        if(_state != null)
        {
            ProcessState(delta);
        }

        string transition = ProcessTransition(delta);

        if(transition != null && transition != _state)
        {
            SetState(transition);
        }
    }

    public void SetState(string newState)
    {
        if(!_states.Contains(newState))
        {
            GD.PrintErr($"State machine doesn't contain state {newState}");
            return;
        }

        _previousState = _state;
        _state = newState;

        if (_state != _previousState)
        {
            //EmitSignal(nameof(StateChanged), newState);
        }

        if (_previousState != null)
        {
            ExitState(_previousState, _state);
        }

        if (_state != null)
        {
            EnterState(_state, _previousState);
        }
    }

    protected abstract void Initialize();
    protected abstract void ProcessState(float delta);
    protected abstract string ProcessTransition(float delta);
    protected abstract void EnterState(string newState, string oldState);
    protected abstract void ExitState(string oldState, string newState);
}
