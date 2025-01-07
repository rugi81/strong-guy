using Godot;
using System;
using System.Collections.Generic;

public partial class player_ui_manager : CanvasLayer
{
	[Export]
	private PackedScene playerHUD;

	[Export]
	private Color[] playerColors;
	
	[Export]
	private int playerCount = 1;

	private List<player_hud> playerHUDs = new List<player_hud>();
	private HBoxContainer playersUIContainer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playersUIContainer = GetNode<HBoxContainer>("Players");

		for ( var i = 0; i < playerCount; i++ ){
			playerHUDs.Add( AddPlayerHUD("[b]Player "+(i+1)+"[/b]", playerColors[i]) );
		}

		playerHUDs[0].createStatusLabel("Health", (500).ToString());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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

	private void RemovePlayerHUD(){

	}
}
