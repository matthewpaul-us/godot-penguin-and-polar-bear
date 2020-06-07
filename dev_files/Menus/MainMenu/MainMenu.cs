using Godot;
using System;

public class MainMenu : CanvasLayer
{
	[Export] public string PlayScene { get; set; }

	private bool _isKeyboardSelected;
	private bool _isGamePadSelected;
	private AnimationPlayer _anim;
	private AudioStreamPlayer _uiSound;

	public override void _Ready()
	{
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
		_uiSound = GetNode<AudioStreamPlayer>("UISound");
	}

	public override void _PhysicsProcess(float delta)
	{
		if((_isGamePadSelected && Input.IsActionJustPressed("gamepad_confirm"))
			|| (_isKeyboardSelected && Input.IsActionJustPressed("keyboard_confirm")))
		{
			Globals.IsGamepad = _isGamePadSelected;
			Globals.LevelLoader.LoadScene(PlayScene);
		}

		if(Input.IsActionJustPressed("gamepad_select"))
		{
			SelectGamePad();
		}
		else if (Input.IsActionJustPressed("keyboard_select"))
		{
			SelectKeyboard();
		}

	}

	public void SelectGamePad()
	{
		if (!_isGamePadSelected)
		{
			_anim.Seek(0f, true);
			_anim.Play("select_gamepad");
		}

		GD.Print("Gamepad Selected!");
		_isGamePadSelected = true;
		_isKeyboardSelected = false;
	}

	public void SelectKeyboard()
	{
		if (!_isKeyboardSelected)
		{
			_anim.Seek(0f, true);
			_anim.Play("select_keyboard");
		}

		GD.Print("Keyboard Selected!");
		_isGamePadSelected = false;
		_isKeyboardSelected = true;
	}

	public void OnPlayButtonPressed()
	{
		Globals.LevelLoader.LoadScene(PlayScene);
	}
}
