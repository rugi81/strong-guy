using Godot;
using System;

public partial class main : Node2D
{
	private Camera2D cam1;
	private CharacterBody2D pl1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam1 = GetNode<Camera2D>("Player1Cam");
		pl1 = GetNode<CharacterBody2D>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		cam1.Position = pl1.Position;
	}
}
