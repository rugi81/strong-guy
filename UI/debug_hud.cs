using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

public partial class debug_hud : Control
{
	private List<string> actions = new List<string>();
	private Player p;
	private bool playerSet = false; 
	private RichTextLabel actionString;
	private Dictionary<string, string> actionDictionary = new Dictionary<string, string>{
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
		{"action-held","[b]Argh[/b]"}
	};
	private int maxShownActions = 6;
	[Export]
	private float actionHoldTime = 1; 	// time in seconds that a held down action counts as a hold.
	private float actionTimer = 0;		// use this to time hold and release
	private bool actionHeld = false; 
	[Export]
	private float actionTimeLimit = 2; // time in seconds for a single combo to decay, from the last action
	private string currentAction = "";
	private string currentActionType = "";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		actionString = GetNode<RichTextLabel>("ActionInput");
		GD.Print("TEST: "+GetActionKey("jump"));
		actionString.Text = "";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
//			GD.Print( "debug ui process" );
		if ( playerSet ){
			int pid = p.getPlayerIndex();	

			/*
				- a key or a combination of keys is an action
				- some keys are mutually exclusive (eg. left and right directions)
				- one action at a time, sequentially
				- the next action will override the previous, but may still register.
					- moving right, then hitting 'a' will override the "action" with "a", but the character will still be moving.
				- the current action can be held
				- the move list will reset X seconds after ( the last registered press action || the last hold action release )

			*/

			if ( actionHeld ){
				actionTimer += (float) delta;
				if ( actionTimer > actionHoldTime && !currentAction.Contains("-held")){
					UpdateLastAction(currentAction+"-held");
				}
			}

			if ( Input.IsActionJustReleased("input_jump"+pid) || 
					Input.IsActionJustReleased("input_action"+pid) || 
					Input.IsActionJustReleased("input_right"+pid) || 
					Input.IsActionJustReleased("input_left"+pid) || 
					Input.IsActionJustReleased("input_up"+pid) || 
					Input.IsActionJustReleased("input_down"+pid) ){
				actionHeld = false;
				actionTimer = 0;
			}

			if ( Input.IsActionPressed("input_action"+pid) ){				
				processAction("action");
			}else if ( Input.IsActionPressed("input_jump"+pid) ){				
				processAction("jump");
			}else if ( checkDiagonal("input_left"+pid, "input_up"+pid) ){
				processAction("nw");
			}else if ( checkDiagonal("input_right"+pid,"input_up"+pid) ){
				processAction("ne");
			}else if ( checkDiagonal("input_left"+pid,"input_down"+pid) ){
				processAction("sw");
			}else if ( checkDiagonal("input_right"+pid,"input_down"+pid) ){
				processAction("se");
			}else{
				if ( Input.IsActionPressed("input_left"+pid) ){
					processAction("w");
				}else if ( Input.IsActionPressed("input_right"+pid) ){
					processAction("e");
				}else if ( Input.IsActionPressed("input_up"+pid) ){
					processAction("n");
				}else if ( Input.IsActionPressed("input_down"+pid) ){
					processAction("s");
				}
			}
		}
	}

	private bool checkDiagonal( string action1, string action2 ){
		float actionStrengthThreshold = 0.9f;
		return Input.IsActionPressed(action1) && Input.GetActionStrength(action1) > actionStrengthThreshold
					&& Input.IsActionPressed(action2)  && Input.GetActionStrength(action2) > actionStrengthThreshold;
	}

	private void processAction(string inAction){
		if ( !(inAction+"-held" == currentAction && actionHeld) ){
			currentAction = inAction;
		}
		if (actions.Count > 0){
			if (currentAction != actions[actions.Count-1] || !actionHeld){
				AddAction(inAction);
				actionTimer = 0;
			}
		}else
			AddAction(inAction);

		actionHeld = true;
	}

	private string GetActionKey(string inAction){
		
		return actionDictionary[inAction];
//		return "";
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
		UpdateActionString();
	}

	private void UpdateLastAction(string inAction){
		actions[actions.Count-1] = inAction;
		currentAction = inAction;
		UpdateActionString();
	}
	private void RemoveAction(){
		actions.RemoveAt(0);
		UpdateActionString();
	}

	private void UpdateActionString(){
		string a = "";
		for( int i=0; i<actions.Count; i++ ){
			a += GetActionKey( actions[i] );
		}
		actionString.Text = a;
		GD.Print( "actionString: " + a );
	}

	private void AddMonitorLabel(){

	}
}
