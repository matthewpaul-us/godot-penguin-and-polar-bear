using Godot;
using System;

public class TitleScreen : TextureRect
{
	[Export] public string NextLevel { get; set; }
	public override void _Ready()
	{

	}

	public void OnTimerTimeout()
	{
		GD.Print("Timeout");
		Globals.LevelLoader.LoadScene(NextLevel);
	}
}
