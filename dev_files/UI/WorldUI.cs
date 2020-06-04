using Godot;
using System;
using System.Collections.Generic;

public class WorldUI : CanvasLayer
{
	private TextureRect _health1;
	private TextureRect _health2;
	private TextureRect _health3;
	private TextureRect _health4;
	private TextureRect _health5;

	private List<TextureRect> _healths;
	public override void _Ready()
	{
		_health1 = GetNode<TextureRect>("MarginContainer/HBoxContainer/Health");
		_health2 = GetNode<TextureRect>("MarginContainer/HBoxContainer/Health2");
		_health3 = GetNode<TextureRect>("MarginContainer/HBoxContainer/Health3");
		_health4 = GetNode<TextureRect>("MarginContainer/HBoxContainer/Health4");
		_health5 = GetNode<TextureRect>("MarginContainer/HBoxContainer/Health5");

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
}
