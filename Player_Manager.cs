using Godot;
using System;
using System.Collections.Generic;

public partial class Player_Manager : Node
{
	[Export] 
	private PackedScene player;

	[Export]
	private Node playerFolder;

	private List<Player> players = new List<Player>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddPlayer(int inIndex){
		Player p = player.Instantiate<Player>();
		players.Add(p);
		p.setPlayerIndex( inIndex );
		playerFolder.AddChild(p);
	}

	public void RemovePlayer(int inIndex){

	}
}
