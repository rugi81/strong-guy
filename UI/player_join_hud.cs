using Godot;
using System;

public partial class player_join_hud : Control
{
	public int playerIndex = -1;
	public bool selectingPlayer = false;
	private int oldCharacter = 1;
	private int currentCharacter = 1;
	private int characterNumber = 3;


	private Control joinHUD;
	private Control selectHUD;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		joinHUD = GetNode<Control>("JoinHUD");
		selectHUD = GetNode<Control>("SelectCharacter");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{		
		if ( selectHUD.Visible ){
			if (Input.IsActionJustPressed("input_left"+playerIndex)){
				currentCharacter--;
				if ( currentCharacter < 1 ){
					currentCharacter = characterNumber;
				}
			}			
			if (Input.IsActionJustPressed("input_right"+playerIndex)){
				currentCharacter++;
				if ( currentCharacter > characterNumber ){
					currentCharacter = 1;
				}
			}			
			if (Input.IsActionJustPressed("input_action"+playerIndex)){
				DoSelect();
			}			

			selectHUD.GetNode<Control>("Cha"+oldCharacter).Visible = false;
			selectHUD.GetNode<Control>("Cha"+currentCharacter).Visible = true;

			oldCharacter = currentCharacter;
		}
	}

	public void SelectAPlayer( bool activate,int inPlayerIndex ){
		joinHUD.Visible = !activate;
		selectHUD.Visible = activate;
		playerIndex = inPlayerIndex;
	}

	private void DoSelect(){
//		gettree().root.get_child(0)
		
		GetTree().Root.GetChild(0).GetNode<Player_Manager>("Player Manager").AddPlayer(playerIndex, currentCharacter);
		SelectAPlayer( false, playerIndex );
	}
}
