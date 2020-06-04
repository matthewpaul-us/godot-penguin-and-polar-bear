using Godot;
using System;

public class MainMenu : CanvasLayer
{
	[Export] public string PlayScene { get; set; }
	public override void _Ready()
	{

	}

	public void OnPlayButtonPressed()
	{
		Globals.LevelLoader.LoadScene(PlayScene);
	}
}
