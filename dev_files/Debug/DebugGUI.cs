using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public enum DebugPane
{
	TopLeft,
}
public class DebugGUI : CanvasLayer
{
	private Label _topLeftLabel;

	public List<IDebugField> TopLeftFields;

	public DebugGUI()
	{
		TopLeftFields = new List<IDebugField>();
	}
	public override void _Ready()
	{
		_topLeftLabel = GetNode<Label>("TopLeftLabel");

		AddToGui(DebugPane.TopLeft, "FPS", () => Performance.GetMonitor(Performance.Monitor.TimeFps).ToString());
		AddToGui(DebugPane.TopLeft, "Memory", () => FormatByteLength((long)Performance.GetMonitor(Performance.Monitor.MemoryStatic)));
	}

	public void AddToGui(DebugPane pane, string name, Func<string> getter)
	{
		List<IDebugField> paneToAdd;
		switch (pane)
		{
			case DebugPane.TopLeft:
				paneToAdd = TopLeftFields;
				break;
			default:
				GD.PrintErr("Unknown Debug pane location");
				return;
		}

		paneToAdd.Add(new DebugMethod(name, getter));
	}
	public void AddToGui<T>(DebugPane pane, string name, T node, Func<T, string> getter)
	{
		List<IDebugField> paneToAdd;
		switch (pane)
		{
			case DebugPane.TopLeft:
				paneToAdd = TopLeftFields;
				break;
			default:
				GD.PrintErr("Unknown Debug pane location");
				return;
		}

		paneToAdd.Add(new DebugField<T>(name, node, getter));
	}

	public override void _PhysicsProcess(float delta)
	{
		string output = "";
		foreach(var field in TopLeftFields)
		{
			output += $"{field.GetField()}\n";
		}

		_topLeftLabel.Text = output;
	}

	private string FormatByteLength(long i)
	{
		long abs_i = (i < 0 ? -i : i);

		string suffix;
		double readable;

		if (abs_i >= 0x1000000000000000) // Exabyte
		{
			suffix = "EiB";
			readable = (i >> 50);
		}
		else if (abs_i >= 0x4000000000000) // Petabyte
		{
			suffix = "PiB";
			readable = (i >> 40);
		}
		else if (abs_i >= 0x10000000000) // Terabyte
		{
			suffix = "TiB";
			readable = (i >> 30);
		}
		else if (abs_i >= 0x40000000) // Gigabyte
		{
			suffix = "GiB";
			readable = (i >> 20);
		}
		else if (abs_i >= 0x100000) // Megabyte
		{
			suffix = "MiB";
			readable = (i >> 10);
		}
		else if (abs_i >= 0x400) // Kilobyte
		{
			suffix = "KiB";
			readable = i;
		}
		else
		{
			return i.ToString("0 B"); // Byte
		}
		// Divide by 1024 to get fractional value
		readable = (readable / 1024);
		// Return formatted number with suffix
		return readable.ToString("0.### ") + suffix;
	}
}
