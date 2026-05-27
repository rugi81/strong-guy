using Godot;
using System;
using System.Collections.Generic;

public partial class player_selector : Panel
{
	private Player p;
	private int ControllerID;
	private bool playerReady = false;
	private int character_selection = 0;
	private int max_character = 4;

	private bool selectionChanged = false;

	[Export]
	private string title = "Player";
	[Export]
	private Color playerColor = new Color( 255, 255, 255, 1 );


// UI
	private RichTextLabel playerLabel;
	private RichTextLabel characterName;

	[Export]
	public bool active = false;

	[Export]
	public bool isListening = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerLabel = GetNode<RichTextLabel>("Active/Active Text/Container2/PlayerLabel");
		characterName = GetNode<RichTextLabel>("Active/Active Text/Container/CharacterName");

		playerLabel.Text = title;
		playerLabel.AddThemeColorOverride("default_color", new Color( playerColor ) );
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	
		//listenForInput( playerInputs );

	}

	public void listenForInput( double delta, List<int> unassignedControllers )
	{

		if ( isListening ){

			if ( !active ){
				bool updateControllerList = false;

				foreach ( var i in unassignedControllers ){
					// is this controller available??
					if ( Input.IsActionPressed("input_jump"+i) )
					{
						GD.Print("Input - "+i);
						updateControllerList = true;
						activatePlayer(i);
					}
				}

				if ( updateControllerList)
				{
					unassignedControllers.Remove( ControllerID );
				}
			}
			else
			{

					if ( Input.IsActionPressed("input_action"+ControllerID))
					{
						unassignedControllers.Add( ControllerID );
						deActivatePlayer();	
					}
					
					// need to sort controller issue - THUMB STICK WILL TRIGGER LEFT THEN RIGHT
					if ( Input.IsActionPressed("input_left"+ControllerID) && !selectionChanged )
					{
						GD.Print("left");
						// prev character
						prevCharacter();
					}
					if ( Input.IsActionPressed("input_right"+ControllerID) && !selectionChanged)
					{
						GD.Print("right");
						// next character
						nextCharacter();
					}
					if (Input.IsActionJustReleased("input_right" + ControllerID) || Input.IsActionJustReleased("input_left" + ControllerID))
					{
						selectionChanged = false;
					} 


			}

		}		
	}

	// fn: Activate Selector
	private void activatePlayer(int inControllerID)
	{
		GD.Print("activatePlayer: "+playerLabel);
		GD.Print("character: "+character_selection);
		ControllerID = inControllerID;
		active = true;
		updateUIActivation();
	}

	private void deActivatePlayer()
	{
		ControllerID = -1;
		active = false;
		updateUIActivation();
	}

	private void updateUIActivation()
	{
		GetNode<PanelContainer>("Disabled").Visible = !active;
		GetNode<PanelContainer>("Active").Visible = active;
	}
	// fn: set Player

	// fn: next Char
	private void nextCharacter()
	{
		character_selection = character_selection+1;
		if ( character_selection > max_character)
		{
			character_selection = 0;
		}
		selectionChanged = true;
		GD.Print("character: "+character_selection);
	}

	// fn: prev Char
	private void prevCharacter()
	{
		character_selection = character_selection-1;
		if ( character_selection < 0)
		{
			character_selection = max_character;
		}
		selectionChanged = true;
		GD.Print("character: "+character_selection);
	}

}
