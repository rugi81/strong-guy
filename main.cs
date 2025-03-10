using Godot;
using System;
using System.Runtime.Intrinsics;
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

	// Background stuff
	private ParallaxLayer clouds;
	[Export]
	private float cloudMotion = 0;
	private Node2D cloudShadow;
	private Vector2 cloudOrigin;
	private bool cloudCover = false;

	private float groundBoundary = -600;
	private float groundOrigin = 600;

	[Export]
	public int maxPlayers = 4;

	private player_ui_manager playerUI;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam1 = GetNode<Camera2D>("Player1Cam");
		players = GetNode<Node>("Players");
		playerArr = players.GetChildren();
		playerUI = GetNode<player_ui_manager>("Player UI Manager");
		
		for ( var i=0; i<maxPlayers; i++ ){
			player_hud ph = playerUI.AddPlayerHUD( i, !!(i<playerArr.Count) );
			if ( i < playerArr.Count ){
				Player p = (Player) playerArr[i];
				ph.assignPlayer( p );
				p.EntityDeath += _on_player_death;
			}
		}

		if ( hasTrain ){
			ground = GetNode<TileMap>("Ground");
			wheels = GetTree().GetNodesInGroup("train_wheel");
		}

		clouds = GetNode<ParallaxLayer>("Player1Cam/ParallaxBackground/Clouds");
		cloudShadow = GetNode<Node2D>("CloudShadow");
		cloudOrigin = new Vector2( cloudShadow.Position.X, cloudShadow.Position.Y );

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
	
		clouds.MotionOffset += new Vector2( -cloudMotion, 0 );
		var pbg = clouds.GetParent<ParallaxBackground>();
		int cloudsWidth = 5262;

		Vector2 cloudScrollOffset = clouds.MotionOffset % cloudsWidth; 
		Vector2 camOffset = new Vector2( cam1.GetScreenCenterPosition().X - (cam1.GetViewportRect().Size.X/2), 0 );
		camOffset *= -clouds.MotionScale; // Parallax layer motion scale multiplies the CAMERA, not the background scroll offset.

		cloudShadow.Position = cloudOrigin + cloudScrollOffset + camOffset;
		cloudShadow.Position = new Vector2( cloudShadow.Position.X, cam1.GetScreenCenterPosition().Y + cloudOrigin.Y - 320);

//		GD.Print( camOffset + " " + pbg.ScrollOffset + " " + clouds.GlobalPosition );
		Area2D bgsl = GetNode<Area2D>("BGSunLight");
		bgsl.Position = cam1.GetScreenCenterPosition() + new Vector2( 0, -50);

		PointLight2D sun = bgsl.GetNode<PointLight2D>("PointLight2D");
		sun.Visible = !cloudCover;
		if ( !cloudCover ){
			if ( sun.Energy > 2 ){
				sun.Energy = 2;
			}else{
				sun.Energy += (float)delta * 10;
			}
		}else{
			if ( sun.Energy < 0 ){
				sun.Energy = 0;
			}else{
				sun.Energy -= (float)delta * 10;
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

	private void _on_player_death( Vector2 inPos, Player p ){
		//GD.Print("DIE: "+p);
		playerArr.Remove(p);
		// spawn tombstone
		
	}

	private void _on_body_body_shape_entered(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index){
//		GD.Print( "SUN HIT: "+ body.GetType().Name );
	}

	private void _on_bg_cloud_area_entered(Area2D area){
		GD.Print( "enter: " + area.Name );
		GD.Print( cloudCover );

		cloudCover = true;
		//area.GetNode<PointLight2D>("PointLight2D").Visible = false;
		//area.GetNode<PointLight2D>("PointLight2D").Energy = 0;
	}

	private void _on_bg_cloud_area_exited(Area2D area){
		GD.Print( "exit: " + area.Name );
		GD.Print( cloudCover );

		cloudCover = false;
		//area.GetNode<PointLight2D>("PointLight2D").Visible = true;
		//area.GetNode<PointLight2D>("PointLight2D").Energy = 20;
	}

	public void ConnectPlayerDeath( Player p ){
		playerArr.Add(p);
		p.EntityDeath += _on_player_death;
	}
}
