using Godot;
using System;
using System.Collections.Generic;

public partial class character_selection : Control
{
	private  Godot.Collections.Array<Node> player_Selectors;
	private List<int> availableControllers = new List<int>();
	private List<int> availableCharacters = new List<int>();

	private bool testChanged = false;
	private string testString = "";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player_Selectors = GetNode<Node>("UI Wrapper/UI Main/Players").GetChildren();
		availableControllers.Add(0);
		availableControllers.Add(1);
		availableControllers.Add(2);
		availableControllers.Add(3);

		foreach ( int i in availableControllers)
		{
			GD.Print( "Ready:"+i );			
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print( "av:"+ availableControllers.ToString() );
		string test_output = "";

		foreach ( int i in availableControllers)
		{
			test_output += " "+i;	
		}

		foreach ( player_selector ps in player_Selectors)
		{
			ps.listenForInput( delta, availableControllers );
		}

		if ( test_output != testString)
		{
			testString = test_output;
			GD.Print( test_output );
		}
	}
}
