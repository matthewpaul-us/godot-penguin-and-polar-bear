using Godot;
using System;

public class EndScreen : CanvasLayer
{
	[Export] public string RestartScenePath;

	private Label _cratesCollectedLabel;
	private Label _superlativeLabel;
	public override void _Ready()
	{
		_superlativeLabel = GetNode<Label>("SuperlativeLabel");
		_cratesCollectedLabel = GetNode<Label>("CratesCollectedLabel");

		SetCrateText(Globals.CrateCount, Globals.TotalCrateCount);
		_superlativeLabel.Text = GetSuperlative();
	}

	private void SetCrateText(int crateCount, int totalCrateCount)
	{
		_cratesCollectedLabel.Text = $"{crateCount} of {totalCrateCount}";
	}

	public void OnRestartButtonPressed()
	{
		Globals.LevelLoader.LoadScene(RestartScenePath);
	}

	public void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

	public string GetSuperlative()
	{
		//TODO - Credit this. from Quora: https://qr.ae/pNK8Yx
		var firstQuarterWords = new string[]
		{
			"Fair",
			"Mediocre",
			"Limited",
		};

		var secondQuarterWords = new string[]
		{
			"Great",
			"Good",
			"Fine",
			"Decent",
		};

		var thirdQuarterWords = new string[]
		{
			"Amazing",
			"Exceptional",
			"Excellent",
			"Superior",
		};

		var fourthQuarterWords = new string[]
		{
			"Phenomenal",
			"World-Class",
			"Incredible",
		};

		float percentCollected = 0;
		if (Globals.TotalCrateCount > 0)
		{
			percentCollected = (float)Globals.CrateCount / Globals.TotalCrateCount;
		}

		var _rand = Globals.Random;

		GD.Print(percentCollected);

		if (percentCollected >= 1)
		{
			return _rand.Random(fourthQuarterWords);
		}
		else if (percentCollected >= .75)
		{
			return _rand.Random(thirdQuarterWords);
		}
		else if (percentCollected >= .5)
		{
			return _rand.Random(secondQuarterWords);
		}
		else
		{
			return _rand.Random(firstQuarterWords);
		}
	}
}
