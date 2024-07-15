using Godot;
using System;

public partial class player : CharacterBody2D
{
	private AnimatedSprite2D spr;
	private Boolean jumping = false;
	private Boolean kicking = false;
	private Boolean face_right = true;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private int WalkSpeed = 0;
	private const int MaxWalkSpeed = 200;
	private float WalkAcceleration = 10;
	private Area2D punch;
	private Node2D wrapper;
	
	[Signal]
	public delegate void PunchTargetEventHandler( Boolean direction, float power );
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite2D>("Wrapper/AnimatedSprite2D");
		punch = GetNode<Area2D>("Wrapper/Punch");
		wrapper = GetNode<Node2D>("Wrapper");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{	
		var velocity = Velocity;

		velocity.Y += (float)delta * Gravity;
		
			punch.Monitoring = false;
		if (IsOnFloor()){
			jumping = false;
		}

		if (Input.IsActionJustPressed("input_jump") && !jumping){
			spr.Play("sg-jump");
			velocity.Y -= 550;	
			jumping = true;
			kicking = false;
			//GD.Print("!");
		}
		
		if (Input.IsActionPressed("input_left"))
		{
			if (!jumping && !kicking)
				spr.Play("sg-walk");
			//spr.FlipH = true;
			//GD.Print( Scale );
			wrapper.Scale = new Vector2(-1,1);
			face_right = false;
			WalkSpeed -= (int) WalkAcceleration;
			if ( WalkSpeed < -MaxWalkSpeed )
				WalkSpeed = -MaxWalkSpeed;
			velocity.X = WalkSpeed;
		}
		else if (Input.IsActionPressed("input_right"))
		{
			if (!jumping && !kicking)
				spr.Play("sg-walk");
//			spr.FlipH = false;
			wrapper.Scale = new Vector2(1,1);
			face_right = true;

			WalkSpeed += (int) WalkAcceleration;
			if ( WalkSpeed > MaxWalkSpeed )
				WalkSpeed = MaxWalkSpeed;			
			velocity.X = WalkSpeed;
		}
		else
		{
			int Momentum = 10;
			if (WalkSpeed < 0){
				WalkSpeed += Momentum;
			}else if (WalkSpeed > 0){
				WalkSpeed -= Momentum;
			}
			velocity.X = WalkSpeed;
			if (!jumping && !kicking)
				spr.Play("sg-idle");
//			velocity.X = 0;
		}
		
		if (Input.IsActionJustPressed("input_action")){
			//GD.Print("KICK");
			spr.Play("sg-punch");
			kicking = true;
			punch.Monitoring = true;
		}

		Velocity = velocity;

		// "MoveAndSlide" already takes delta time into account.
		MoveAndSlide();
		
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			Node c = (Node)collision.GetCollider();
			
			if ( c.GetType().Name == "baddie" && kicking ){
				//((baddie) c).getHit( face_right );
//				((baddie)collision.GetCollider()).getHit( spr.FlipH );
			}
		}				
	}
	
	private void _on_animated_sprite_2d_animation_finished()
	{
		if (spr.Animation == "kick" || spr.Animation == "sg-punch"){
			kicking = false;
		}
		// Replace with function body.
	}

	private void _on_punch_body_entered(Node2D body)
	{
		//GD.Print("PUNCH!!! "+body.Name+" ~ "+kicking);
		// Replace with function body.
		
		if ( body.GetType().Name == "baddie" && kicking ){
			((baddie)body).getHit( face_right );
//			GD.Print("SIGNAL");
//			EmitSignal(SignalName.PunchTarget, face_right, 200);
		}
		punch.SetDeferred("Monitoring", false);
	}
	
	private void _on_punch_area_entered(Area2D area)
	{
		GD.Print("P AREA");
		// Replace with function body.
	}
}












