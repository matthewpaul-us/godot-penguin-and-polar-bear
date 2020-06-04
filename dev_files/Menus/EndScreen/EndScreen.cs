using Godot;
using System;

public class EndScreen : CanvasLayer
{
	[Export] public string RestartScenePath;
	public override void _Ready()
	{

	}

	private void OnRestartButtonPressed()
	{
		Globals.LevelLoader.LoadScene(RestartScenePath);
	}
}


