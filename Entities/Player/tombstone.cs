using Godot;
using System;

public partial class tombstone : RigidBody2D
{
	private int playerIndex;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void setPlayer(int inPlayer){ // store the playerIndex, so... if we're ever "brought back to life", use this tombstone as a reference/position point.
		playerIndex = inPlayer;
	}
}
