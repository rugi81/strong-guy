using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : CharacterBody2D
{
	// Object refs
	private AnimatedSprite2D spr;
	private AnimationPlayer anim;
	private Node2D wrapper;

	[Export]
	private int playerIndex = 0;

	public const float Speed = 300.0f;
	public const float JumpVelocity = -550.0f;

	// State vars
	private Boolean jumping = false;
	private Boolean attacking = false;
	private Boolean face_right;

	private int msgCount = 0;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite2D>("Wrapper/AnimatedSprite2D");
		anim = GetNode<AnimationPlayer>("Wrapper/AnimationPlayer");
		wrapper = GetNode<Node2D>("Wrapper");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		velocity.Y += gravity * (float)delta;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.

		Vector2 direction = Input.GetVector("input_left"+playerIndex, "input_right"+playerIndex, "input_jump"+playerIndex, "input_down"+playerIndex);
		
		if (  Mathf.Round( direction.X * 100 )/100 != 0 )//Vector2.Zero)
		{
			if (!jumping && !attacking){			
				spr.Play("walk");
				anim.Play("walk");
			}
			velocity.X = direction.X * Speed;
			face_right = !!(direction.X > 0);
			wrapper.Scale = new Vector2( face_right?1:-1, 1 );
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			if (!jumping && !attacking){
				spr.Play("idle");
				anim.Play("idle");
			}
		}

		if (Input.IsActionJustPressed("input_action"+playerIndex) && !attacking){
			GD.Print("Attack: {0}; {1}", msgCount++, playerIndex);
			spr.Play("attack");
			anim.Play("attack");
			attacking = true;
			//attack.Monitoring = true;
			//GD.Print( GetNode<Area2D>("Wrapper/Actions/attack").Monitoring );
		}


		if (IsOnFloor()){
			jumping = false;
		}

		if (!IsOnFloor() && anim.CurrentAnimation != "jump" && anim.CurrentAnimation != "idle" && anim.CurrentAnimation != ""){
			//GD.Print('"'+anim.CurrentAnimation+'"'+" - "+attacking);
			if (!attacking && jumping){
				//GD.Print("Change");
				spr.Play("jump");
				anim.Play("jump");
				anim.Seek(.5);
			}
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("input_jump"+playerIndex) && IsOnFloor()){
			velocity.Y = JumpVelocity;
			spr.Play("jump");
			anim.Play("jump");
			jumping = true;
			if ( attacking ){
				attacking = false;
			}
		}
		
		Velocity = velocity;
		MoveAndSlide();

	}

	private void _on_animation_player_animation_finished(String animStr){
		if (animStr == "attack"){
			//GD.Print("Stop Attack");
			anim.Stop(true);
			spr.Stop();
			attacking = false;
		}
	}

	private void _on_attack_body_entered(Node2D body)
	{	
		GD.Print("contact "+attacking);
		if ( body.GetType().Name == "ghost" && attacking ){
			((ghost)body).getHit( face_right );
		}
		GD.Print( body.GetType().Name );
	}

	private void _on_body_body_shape_entered(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index){
		GD.Print("BODY CONTACT "+body.GetType().Name);
		
	}
}
