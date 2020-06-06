using Godot;
using System;
using System.Runtime.CompilerServices;

public class ScreenShake : Node
{

	private Camera2D _camera;
	private float _priority;
	private float _amplitude;

	private Timer _durationTimer;
	private Timer _frequencyTimer;
	private Tween _tween;

	public override void _Ready()
	{
		_durationTimer = GetNode<Timer>("Duration");
		_frequencyTimer = GetNode<Timer>("Frequency");
		_tween = GetNode<Tween>("ShakeTween");

		_camera = GetParent() as Camera2D;

		if (_camera == null)
		{
			GD.PrintErr("ScreenShake's parent is not a camera!");
		}
	}

	public void Start(float duration = 0.2f, float frequency = 15, float amplitude = 16, float priority = 0)
	{
		if (priority < _priority)
		{
			return;
		}

		_priority = priority;
		_amplitude = amplitude;

		_durationTimer.Start(duration);
		_frequencyTimer.Start(1 / frequency);

		StartNewShake();

	}

	private void StartNewShake()
	{
		var random = Globals.Random.Vector(_amplitude);

		_tween.InterpolateProperty(_camera, "offset",
			_camera.Offset, random,
			_frequencyTimer.WaitTime,
			Tween.TransitionType.Sine, Tween.EaseType.InOut);
		_tween.Start();
	}

	private void Reset()
	{
		_tween.InterpolateProperty(_camera, "offset",
			_camera.Offset, Vector2.Zero,
			_frequencyTimer.WaitTime,
			Tween.TransitionType.Sine, Tween.EaseType.InOut);
		_tween.Start();

		_priority = 0;
	}

	public void OnFrequencyTimerTimeout()
	{
		StartNewShake();
	}

	public void OnDurationTimerTimeout()
	{
		Reset();
		_frequencyTimer.Stop();
	}
}
