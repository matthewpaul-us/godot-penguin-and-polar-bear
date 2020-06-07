using Godot;
using System;
using System.Linq;

public class World : Node2D
{
	[Export] public string PostGameScene;

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
			crate.Player = _player;
		}

		_ui.SetCrateCount(0, TotalCrateCount);

		Globals.CrateCount = 0;
		Globals.TotalCrateCount = TotalCrateCount;

		Globals.DebugGUI.AddToGui(DebugPane.TopLeft, "Collected", () => Globals.CrateCount.ToString());
		Globals.DebugGUI.AddToGui(DebugPane.TopLeft, "Total", () => Globals.TotalCrateCount.ToString());
	}

	public void OnPlayerLoseHealth(int newHealth)
	{
		if (newHealth <= 0)
		{
			Globals.LevelLoader.LoadScene(PostGameScene);
		}
	}

	public void OnPlayerDroppedOffCrate()
	{
		CrateCount++;
		Globals.CrateCount++;

		_ui.SetCrateCount(CrateCount);

		if (CrateCount >= TotalCrateCount)
		{
			Win();
		}
	}

	public void Win()
	{
		Globals.CrateCount = CrateCount;
		Globals.TotalCrateCount = TotalCrateCount;

		Globals.LevelLoader.LoadScene(PostGameScene);
	}
}
