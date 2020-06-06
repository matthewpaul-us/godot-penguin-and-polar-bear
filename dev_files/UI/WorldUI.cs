using Godot;
using System;
using System.Collections.Generic;

public class WorldUI : CanvasLayer
{
	[Export] public bool AreAvatarsSwapped = false;
	public int CurrentCrateCount { get; set; }
	public int TotalCrateCount { get; set; }

	private Label _crateLabel;

	private TextureRect _health1;
	private TextureRect _health2;
	private TextureRect _health3;
	private TextureRect _health4;
	private TextureRect _health5;

	private List<TextureRect> _healths;

	private AnimationPlayer _healthAnim;


	public override void _Ready()
	{
		_crateLabel = GetNode<Label>("MarginContainer/HBoxContainer2/VBoxContainer/CurrentCrateCountLabel");
		_healthAnim = GetNode<AnimationPlayer>("HealthAnimationPlayer");

		_health1 = GetNode<TextureRect>("MarginContainer/HBoxContainer2/HBoxContainer/Health");
		_health2 = GetNode<TextureRect>("MarginContainer/HBoxContainer2/HBoxContainer/Health2");
		_health3 = GetNode<TextureRect>("MarginContainer/HBoxContainer2/HBoxContainer/Health3");
		_health4 = GetNode<TextureRect>("MarginContainer/HBoxContainer2/HBoxContainer/Health4");
		_health5 = GetNode<TextureRect>("MarginContainer/HBoxContainer2/HBoxContainer/Health5");

		_healths = new List<TextureRect>() { _health1, _health2, _health3, _health4, _health5 };
	}

	public void SetHealth(int value)
	{
		value = Mathf.Clamp(value, 0, 5);

		bool shouldShowHealth;
		for (int i = 0; i < _healths.Count; i++)
		{
			shouldShowHealth = value > i;

			var health = _healths[i];

			if(shouldShowHealth)
			{
				health.Show();
			}
			else
			{
				health.Hide();
			}
		}
	}

	public void SetCrateCount(int currentCount, int? totalCount = null)
	{
		if(totalCount.HasValue)
		{
			TotalCrateCount = totalCount.Value;
		}

		CurrentCrateCount = currentCount;

		_crateLabel.Text = $"{CurrentCrateCount} of {TotalCrateCount}";
	}

	public void OnPlayerDamaged()
	{
		if (AreAvatarsSwapped)
		{
			_healthAnim.Play("swap_avatars_back");
		}
		else
		{
			_healthAnim.Play("swap_avatars");
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionJustPressed("debug_swap_avatars"))
		{
			OnPlayerDamaged();
		}
	}
}
