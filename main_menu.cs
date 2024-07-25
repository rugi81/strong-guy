using Godot;
using System;

public partial class main_menu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_start_pressed()
	{
		GD.Print("Start");
		GetTree().ChangeSceneToFile("./main.tscn");
	}
	private void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}
