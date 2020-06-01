using Godot;
using System;

public class World : Node2D
{
	private Player _player;
	private Enemy _enemy;

	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
		_enemy = GetNode<Enemy>("Enemy");

		_enemy.Brain.Target = _player;
	}
}
