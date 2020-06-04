using Godot;
using System;
using System.Threading.Tasks;

public class LevelLoader : Node2D
{
	[Signal] public delegate void SceneChangeFinished();

	[Export] public Color LoweredCurtainColor;
	[Export] public float CurtainTransitionTime = 1.5f;

	[Export] public PackedScene StartScene { get; set; }

	public Node LoadedScene { get; set; }
	public string LoadedScenePath { get; set; }

	private Tween _tween;
	private ColorRect _curtain;

	private Color _curtainOriginalColor;

	public override void _Ready()
	{
		_tween = GetNode<Tween>("Tween");
		_curtain = GetNode<ColorRect>("Curtain/CurtainRect");

		_curtainOriginalColor = _curtain.Color;

		LoadScene(StartScene);
	}

	public async void LoadScene(PackedScene scene)
	{
		if (LoadedScene != null)
		{
			await SetCurtainRaised(false);
			RemoveChild(LoadedScene);
			LoadedScene.CallDeferred("QueueFree");
		}

		var level = scene.Instance();

		AddChild(level);

		LoadedScene = level;

		await SetCurtainRaised(true);

	}

	public async void LoadScene(string resourcePath)
	{

		var levelBluePrint = GD.Load<PackedScene>(resourcePath);

		if (levelBluePrint == null)
		{
			GD.PrintErr($"Level '{resourcePath}' not found!");
		}


		LoadScene(levelBluePrint);

		LoadedScenePath = resourcePath;
	}

	private async Task SetCurtainRaised(bool isRaised)
	{
		Color fromColor, toColor;

		if (isRaised)
		{
			fromColor = LoweredCurtainColor;
			toColor = _curtainOriginalColor;
			_curtain.MouseFilter = Control.MouseFilterEnum.Ignore;

		}
		else
		{
			_curtainOriginalColor = _curtain.Color;

			fromColor = _curtainOriginalColor;
			toColor = LoweredCurtainColor;
			_curtain.MouseFilter = Control.MouseFilterEnum.Stop;
		}

		_tween.InterpolateProperty(_curtain, "color",
			fromColor, toColor,
			CurtainTransitionTime,
			Tween.TransitionType.Linear, Tween.EaseType.In
			);

		_tween.Start();

		await ToSignal(_tween, "tween_completed");
	}
}
