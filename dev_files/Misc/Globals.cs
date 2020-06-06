using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Globals : Node
{
	public static AugmentedRandom Random { get; private set; }
	public static DebugGUI DebugGUI { get; private set; }
	public static LevelLoader LevelLoader { get; private set; }
	public static bool IsGamepad {get; set;}
public override void _Ready()
	{
		Random = new AugmentedRandom();

		LevelLoader = GetParent().GetNode<LevelLoader>("LevelLoader");

		var guiScene = GD.Load<PackedScene>("res://Debug/DebugGUI.tscn");
		DebugGUI = guiScene.Instance() as DebugGUI;

		AddChild(DebugGUI);
	}
}
