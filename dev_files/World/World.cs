using Godot;
using System;
using System.Linq;

public class World : Node2D
{
	[Export] public string LoseScene;

	private Player _player;
	private WorldUI _ui;

	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_ui = GetNode<WorldUI>("WorldUI");

		_player.Connect(nameof(Player.HealthChanged), _ui, nameof(_ui.SetHealth));
		_player.Connect(nameof(Player.HealthChanged), this, nameof(OnPlayerLoseHealth));

		foreach (var enemy in GetTree().GetNodesInGroup("enemies").Cast<Enemy>())
		{
			enemy.Brain.Target = _player;
		}
	}

	public void OnPlayerLoseHealth(int newHealth)
	{
		if (newHealth <= 0)
		{
			Globals.LevelLoader.LoadScene(LoseScene);
		}
	}
}
