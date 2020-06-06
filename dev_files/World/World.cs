using Godot;
using System;
using System.Linq;

public class World : Node2D
{
	[Export] public string LoseScene;
	[Export] public float HurtShakeFrequency;
	[Export] public float HurtShakeAmplitude;
	[Export] public float HurtShakeDuration;

	private Player _player;
	private HomeBase _homeBase;
	private WorldUI _ui;

	private int CrateCount = 0;
	private int TotalCrateCount = 0;


	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_homeBase = GetNode<HomeBase>("HomeBase");
		_ui = GetNode<WorldUI>("WorldUI");

		_player.Connect(nameof(Player.HealthChanged), _ui, nameof(_ui.SetHealth));
		_player.Connect(nameof(Player.HealthChanged), this, nameof(OnPlayerLoseHealth));
		_player.Connect(nameof(Player.CrateDroppedOff), this, nameof(OnPlayerDroppedOffCrate));
		_player.Connect(nameof(Player.Damaged), _ui, nameof(WorldUI.OnPlayerDamaged));

		_homeBase.Connect(nameof(HomeBase.PlayerEnteredHomeBase), _player, nameof(Player.OnHomeBaseEntered));


		foreach (var enemy in GetTree().GetNodesInGroup("enemies"))
		{
			if (enemy is Enemy e)
			{
				e.Brain.Target = _player;
			}
			else if (enemy is Walrus w)
			{
				w.Brain.Target = _player;
			}

		}

		foreach (var crate in GetTree().GetNodesInGroup("crates").Cast<Crate>())
		{
			TotalCrateCount++;
			crate.Connect(nameof(Crate.Collected), _player, nameof(Player.OnCrateCollected));
		}

		_ui.SetCrateCount(0, TotalCrateCount);
	}

	public void OnPlayerLoseHealth(int newHealth)
	{
		if (newHealth <= 0)
		{
			Globals.LevelLoader.LoadScene(LoseScene);
		}
	}

	public void OnPlayerDroppedOffCrate()
	{
		CrateCount++;

		_ui.SetCrateCount(CrateCount);
	}
}
