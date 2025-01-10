using Godot;
using System;
using System.Runtime.Serialization;
using System.Transactions;

public partial class main : Node2D
{
	private Camera2D cam1;
	private Node players;
	private Godot.Collections.Array<Node> playerArr;
	private Vector2 playersMidpoint;
	private float playersMaxDistance;
	private TileMap ground;
	private Godot.Collections.Array<Node> wheels;

	[Export]
	private float minZoom = .25f;
	[Export]
	private Boolean hasTrain = true;
	[Export]
	private float maxZoom = 1.25f;
	[Export]
	private float trainAcceleration = 1;
	private float trainSpeed = 10;
	private float groundBoundary = -600;
	private float groundOrigin = 600;

	private player_ui_manager playerUI;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam1 = GetNode<Camera2D>("Player1Cam");
		players = GetNode<Node>("Players");
		playerArr = players.GetChildren();
		playerUI = GetNode<player_ui_manager>("Player UI Manager");
		
		for ( var i=0; i<playerArr.Count; i++ ){
			player_hud ph = playerUI.AddPlayerHUD( "[b]Player "+(i+1)+"[/b]", playerUI.playerColors[i] );
			Player p = (Player) playerArr[i];
			ph.assignPlayer( p );
			p.PlayerHealthZero += _on_player_death;
		}

		if ( hasTrain ){
			ground = GetNode<TileMap>("Ground");
			wheels = GetTree().GetNodesInGroup("train_wheel");
		}

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

		if (hasTrain){
			ground.Position += new Vector2(-(trainSpeed),0);
			if ( ground.Position.X < groundBoundary ){
				ground.Position = new Vector2(groundOrigin, ground.Position.Y);
			}
			for (int i = 0; i < wheels.Count; i++ ){
				Sprite2D w = (Sprite2D) wheels[i];
				w.Rotate( trainSpeed/100 );
			}
		}
	}

	private Vector2 GetPlayersMidPoint(){

		if ( playerArr.Count == 0 ){
			return cam1.Position;
		}
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

	private void _on_player_death( Player p ){
		//GD.Print("DIE: "+p);
		playerArr.Remove(p);
	}
}
