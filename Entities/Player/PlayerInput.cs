using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class PlayerInput : Node
{
	private List<string> actions = new List<string>();
	private Player p;
	private bool playerSet = false; 
	private RichTextLabel actionString;
	private Dictionary<string, string> actionVisualDictionary = new Dictionary<string, string>{
		{"n","C"},
		{"e","A"},
		{"s","D"},
		{"w","B"},
		{"nw","E"},
		{"ne","G"},
		{"se","H"},
		{"sw","F"},
		{"n-held","[color=red]C[/color]"},
		{"e-held","[color=red]A[/color]"},
		{"s-held","[color=red]D[/color]"},
		{"w-held","[color=red]B[/color]"},
		{"nw-held","[color=red]E[/color]"},
		{"ne-held","[color=red]G[/color]"},
		{"se-held","[color=red]H[/color]"},
		{"sw-held","[color=red]F[/color]"},		
		{"jump","[b]B[/b]"},
		{"jump-held","[b][color=red]B[/color][/b]"},
		{"action","[b]A[/b]"},
		{"action-held","[b][color=red]A[][/b]"}
	};
	private Dictionary<string, string> actionDictionary = new Dictionary<string, string>{
		{"n","w"},
		{"e","d"},
		{"s","s"},
		{"w","a"},
		{"nw","w+d"},
		{"ne","w+a"},
		{"se","s+d"},
		{"sw","s+a"},
		{"n-held","w."},
		{"e-held","d."},
		{"s-held","s."},
		{"w-held","a."},
		{"nw-held","w+d."},
		{"ne-held","w+a."},
		{"se-held","s+d."},
		{"sw-held","s+a."},		
		{"jump","J"},
		{"jump-held","J."},
		{"action","A"},
		{"action-held","A."}
	};
	private int maxShownActions = 6;
	[Export]
	private float actionRegisterTime = .03f; 
	[Export]
	private float actionHoldTime = 1; 	// time in seconds that a held down action counts as a hold.
	private float actionTimer = 0;		// use this to time hold and release
	private float liveTimer = 0;
	private bool actionHeld = false;
	private string currentDirection = "";
	private string previousDirection = "";
	private bool directionHeld = false;

	private string lastActionRegistered = "";
	[Export]
	private float actionTimeLimit = .3f; // time in seconds for a single combo to decay, from the last action
	[Export]
	private float actionClearDelay = .3f; // when the actions are cleared, how long to wait until new combo can happen.
	private bool actionClearing = false;

	private string currentAction = "";
	private string currentActionType = "";
	private string currentAbility = "";
	private bool abilityHeld = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print("TEST: "+GetActionKey("jump"));
		actionString.Text = "";
	}

    public string processPlayerInput( double delta ){
		if ( playerSet && !actionClearing ){
			int pid = p.getPlayerIndex();	

			/*
				- a key or a combination of keys is an action
				- an action should only register after a key or keys are held for an amount of time
				- some keys are mutually exclusive (eg. left and right directions)
				- one action at a time, sequentially
				- the next action will override the previous, but may still register.
					- moving right, then hitting 'a' will override the "action" with "a", but the character will still be moving.
				- the current action can be held
				- the move list will reset X seconds after ( the last registered press action || the last hold action release )

			*/
			if ( actions.Count > 0 && !actionHeld ){
				liveTimer += (float) delta;
				if ( liveTimer > actionTimeLimit ){
					ClearActions();
				}
			}

			if ( actionHeld ){
				actionTimer += (float) delta;
				

				if ( actionTimer > actionRegisterTime ){
					registerAction(currentAction);
				}
				if ( actionTimer > actionHoldTime && !currentAction.Contains("-held")){
					UpdateLastAction(currentAction+"-held");
				}
			}

			if ( Input.IsActionJustReleased("input_jump"+pid) || 
				 Input.IsActionJustReleased("input_action"+pid) ){
				actionHeld = false;
				actionTimer = 0;
				currentAction = "";
				lastActionRegistered = "";
				currentAbility = "";
				abilityHeld = false;
			}
			if ( Input.IsActionJustReleased("input_right"+pid) || 
				 Input.IsActionJustReleased("input_left"+pid) || 
				 Input.IsActionJustReleased("input_up"+pid) || 
				 Input.IsActionJustReleased("input_down"+pid) ){
				actionHeld = false;
				actionTimer = 0;
				currentAction = "";
				lastActionRegistered = "";
				currentDirection = "";
				previousDirection = "";
				directionHeld = false;
			}

			bool 	actionKey = Input.IsActionPressed("input_action"+pid),
					jumpKey = Input.IsActionPressed("input_jump"+pid),
					leftKey = checkDirection( "input_left"+pid ),
					upKey	= checkDirection( "input_up"+pid ),
					rightKey = checkDirection( "input_right"+pid ),
					downKey = checkDirection( "input_down"+pid );

			if (pid > 2){}
			//GD.Print( leftKey + " - "+Input.GetActionStrength("input_left"+pid)+" && " + upKey + " - "+Input.GetActionStrength("input_up"+pid) );

			if ( actionKey || jumpKey ){
				
				if ( actionKey )
					currentAbility = "action";
				if ( jumpKey )
					currentAbility = "jump";

				abilityHeld = true;
				processAction(currentAbility);

			}else{

				string cD = "";
				if ( checkDiagonal(leftKey, upKey) ){
					cD = "nw";
				}else if ( checkDiagonal(rightKey, upKey) ){
					cD = "ne";
				}else if ( checkDiagonal(leftKey, downKey) ){
					cD = "sw";
				}else if ( checkDiagonal(rightKey, downKey) ){
					cD = "se";
				}else{
					if ( leftKey ){
						cD = "w";
					}else if ( rightKey ){
						cD = "e";
					}else if ( upKey ){
						cD = "n";
					}else if ( downKey ){
						cD = "s";
					}
				}

				previousDirection = currentDirection;
				currentDirection = cD;
				bool doDirection = false;
				if ( abilityHeld && directionHeld ){
					if ( currentDirection != previousDirection ){
						doDirection = true;
					}
				} else if (currentDirection != "") {
					doDirection = true;
				}

				if ( doDirection ){
					directionHeld = true;
					processAction(currentDirection);
				}
			}
            return GetActionString();
		}else if ( actionClearing ){
			liveTimer += (float) delta;
			if ( liveTimer > actionClearDelay ){
				actionClearing = false;
				liveTimer = 0;
			}
		}


        return "";
	}

	private bool checkDirection( string action ){
		float actionStrengthThreshold = 0.1f;
		return Input.IsActionPressed(action) && Input.GetActionStrength(action) > actionStrengthThreshold;
	}
	private bool checkDiagonal( string action1, string action2 ){
		float actionStrengthThreshold = 0.1f;
		return Input.IsActionPressed(action1) && Input.GetActionStrength(action1) > actionStrengthThreshold
					&& Input.IsActionPressed(action2)  && Input.GetActionStrength(action2) > actionStrengthThreshold;
	}
	private bool checkDiagonal( bool action1, bool action2 ){
		return action1 && action2;
	}

	private void processAction(string inAction){
		if ( !(inAction+"-held" == currentAction && actionHeld) ){
			currentAction = inAction;
			liveTimer = 0;
		}
		actionHeld = true;
	}

	private void registerAction(string inAction){
		if (actions.Count > 0){
//  		GD.Print( currentAction + " " + inAction + " " + lastActionRegistered + " -- " + (lastActionRegistered != currentAction || lastActionRegistered+"-held" != currentAction ) +" " + ( inAction == currentAction ) );
			if ( (lastActionRegistered != currentAction ) && inAction == currentAction){
				AddAction(inAction);
				actionTimer = 0;
			}
		}else
			AddAction(inAction);

	}

	private string GetActionKey(string inAction){		
		return actionDictionary[inAction];
	}

	public void SetPlayer( Player inPlayer ){
		p = inPlayer;
		playerSet = true;
	}

	private void AddAction(string inAction){
		actions.Add(inAction);
		if ( actions.Count > maxShownActions ){
			RemoveAction();
		}
		lastActionRegistered = inAction;
		UpdateActionString();
	}

	private void UpdateLastAction(string inAction){
		actions[actions.Count-1] = inAction;
		currentAction = inAction;
		lastActionRegistered = inAction;
		UpdateActionString();
	}
	private void RemoveAction(){
		actions.RemoveAt(0);
		UpdateActionString();
	}

	public void ClearActions(){
		liveTimer = 0;
		actionClearing = true;
		actions.Clear();
		UpdateActionString();
	}

    private string GetActionString(){
		string a = "";
		for( int i=0; i<actions.Count; i++ ){
			a += GetActionKey( actions[i] );
		}
        return a;
    }

	private void UpdateActionString(){
        //GD.Print( "actions: "+GetActionString() );
	}
}
