using Godot;
using System;
using System.Runtime.Serialization;

public partial class main : Node2D
{
	private Camera2D cam1;
	private Node players;
	private Godot.Collections.Array<Node> playerArr;
	private Vector2 playersMidpoint;
	private float playersMaxDistance;

	[Export]
	private float minZoom = .25f;

	[Export]
	private float maxZoom = 1.25f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam1 = GetNode<Camera2D>("Player1Cam");
		players = GetNode<Node>("Players");
		playerArr = players.GetChildren();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 newPos = GetPlayersMidPoint();
		
		float distance = playersMaxDistance; // 0; // pl1.Position.DistanceTo( pl2.Position );

		//GD.Print( distance );
		float zoomAmt = 300/distance;

		if (zoomAmt > maxZoom){ zoomAmt = maxZoom; }
		if (zoomAmt < minZoom){ zoomAmt = minZoom; }

		cam1.Zoom = new Vector2(zoomAmt,zoomAmt);
		cam1.Position = newPos;
	}

	private Vector2 GetPlayersMidPoint(){

		float 	posX = 0,
				posY = 0;
		CharacterBody2D tP = (CharacterBody2D) playerArr[0];
		float 	lowX = tP.Position.X,
				highX= tP.Position.X,
				lowY = tP.Position.Y,
				highY = tP.Position.Y;

		for ( var i = 0; i < playerArr.Count; i++ ){
			tP = (CharacterBody2D) playerArr[i];
			posX += tP.Position.X;
			posY += tP.Position.Y;

			if ( tP.Position.X < lowX ){
				lowX = tP.Position.X;
			}else if ( tP.Position.X > highX ){
				highX = tP.Position.X;
			}

			if ( tP.Position.Y < lowY ){
				lowY = tP.Position.Y;
			}else if ( tP.Position.Y > highY ){
				highY = tP.Position.Y;
			}
		}
		
		playersMaxDistance = new Vector2( lowX, lowY ).DistanceTo( new Vector2( highX, highY ) );

		posX /= playerArr.Count;
		posY /= playerArr.Count;

		playersMidpoint = new Vector2( posX, posY );
		return playersMidpoint;
	}

	private float GetMidPoint( float a, float b ){ // a is the larger number
		if ( a < b ){
			return a + ( (b - a) / 2 );
		}else{
			return b + ( (a - b) / 2 );
		}
	}

	
}
