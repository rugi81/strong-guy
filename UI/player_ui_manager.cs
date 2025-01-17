using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class player_ui_manager : CanvasLayer
{
	[Export]
	private PackedScene playerHUD;

	[Export]
	public Color[] playerColors;
	
	[Export]
	private int playerCount = 1;
	
	private List<player_hud> playerHUDs = new List<player_hud>();
	private HBoxContainer playersUIContainer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playersUIContainer = GetNode<HBoxContainer>("Players");

//		for ( var i = 0; i < maxPlayers; i++ ){
	//		AddPlayerHUD( i, !!(i < playerCount) );
//		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public player_hud AddPlayerHUD( int index ){
		player_hud ph = AddPlayerHUD("[b]Player "+(index+1)+"[/b]", playerColors[index]);
		ph.setActive(false);
		return ph;
	}

	public player_hud AddPlayerHUD( int index, Boolean active ){
		player_hud ph = AddPlayerHUD("[b]Player "+(index+1)+"[/b]", playerColors[index]);
		ph.setActive(active);
		return ph;
	}

	public player_hud AddPlayerHUD( String playerLabel ){
		player_hud ph = playerHUD.Instantiate<player_hud>();
		var xSpacing = 130;

		ph.Position = new Vector2( playerHUDs.Count * xSpacing, 0 );

		playersUIContainer.AddChild(ph);
		ph.setPlayerHUD(playerLabel);	
		playerHUDs.Add(ph);
		
		return ph;
	}

	public player_hud AddPlayerHUD( String playerLabel, Color inColor ){
		player_hud ph = AddPlayerHUD( playerLabel );
		ph.setPlayerColor( inColor );
		return ph;
	}

	public player_hud GetPlayerHUD( int index ){
		return playerHUDs[index];
	}

	private void RemovePlayerHUD(){

	}
}
