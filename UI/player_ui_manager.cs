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
		ph.p_index = index;
		return ph;
	}

	public player_hud AddPlayerHUD( String playerLabel ){
		player_hud ph = playerHUD.Instantiate<player_hud>();
		var xSpacing = 10;

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

	public void _on_player_manager_player_add_request( int i ){
		GD.Print( "Player "+i+" Add request!");
		
		player_hud ph = playerHUDs[i];
		ph.requestJoin();
	}

	public void _on_player_manager_player_added( Player p, int i ){
		GD.Print( "Player Added");
		player_hud ph = playerHUDs[i];
		ph.setActive(true);
		ph.assignPlayer( p );
		GetParent<main>().ConnectPlayerDeath(p);
	}	
}
