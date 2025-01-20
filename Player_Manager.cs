using Godot;
using System;
using System.Collections.Generic;

public partial class Player_Manager : Node
{
	[Export] 
	private PackedScene player;

	[Export]
	private Node playerFolder;

	[Export]
	private int maxPlayers = 4;

	private List<Player> players = new List<Player>();
	private Boolean[] playerExists = {true,true,false,false};

	[Signal]
	public delegate void PlayerAddedEventHandler( Player p, int i );
	[Signal]
	public delegate void PlayerAddRequestEventHandler( int i );



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var p = playerFolder.GetChildren();
		for ( var i=0; i<p.Count; i++ ){
			Player pl = (Player) p[i];
			int p_index = pl.getPlayerIndex();
			players.Add( pl );
			playerExists[ p_index ] = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
			GD.Print( "0 "+playerExists[0] );
			GD.Print( "1 "+playerExists[0] );
			GD.Print( "2 "+playerExists[0] );
			GD.Print( "3 "+playerExists[0] );
		//if (Input.IsActionJustPressed("input_action"+playerIndex) && !attacking && !gettingHurt){
		for ( var i=0; i<maxPlayers; i++ ){
			if ( !playerExists[i] ){
				GD.Print(i + " Add player?");
				if ( Input.IsActionJustPressed("input_action"+i) ){
					AddPlayer(i);
					EmitSignal("PlayerAddRequest", i);
				}
			}
		}
	}

	public void AddPlayer(int inIndex){
		Player p = player.Instantiate<Player>();
		players.Add(p);
		p.setPlayerIndex( inIndex );
		playerExists[ inIndex ] = true;
		playerFolder.AddChild(p);
		EmitSignal("PlayerAdded", p, inIndex);
	}

	public void RemovePlayer(int inIndex){

	}
}
