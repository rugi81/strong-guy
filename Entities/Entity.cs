using Godot;
using System;
using System.Collections.Generic;

public partial class Entity : CharacterBody2D
{
	// Object refs
	protected AnimatedSprite2D spr;
	protected AnimationPlayer anim;
	protected Node2D wrapper;

	[Export]
	protected int playerIndex = 0;

	public const float Speed = 300.0f;
	public const float JumpVelocity = -550.0f;

	// State vars
	protected Boolean jumping = false;
	protected Boolean attacking = false;
	protected Boolean face_right;
	public Boolean gettingHit = false;
	public int gettingHit_direction = -1;
	public Boolean gettingHurt = false;

	protected int msgCount = 0;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	protected float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

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
			if (!jumping && !attacking && !gettingHurt){			
				spr.Play("walk");
				anim.Play("walk");
			}
			velocity.X = direction.X * Speed;
			face_right = !!(direction.X > 0);
			DoFacing();
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			if (!jumping && !attacking && !gettingHurt){
				spr.Play("idle");
				anim.Play("idle");
			}
		}

		if (Input.IsActionJustPressed("input_action"+playerIndex) && !attacking && !gettingHurt){
			DoAttack();
		}


		if (IsOnFloor()){
			jumping = false;
		}

		if (!IsOnFloor() && anim.CurrentAnimation != "jump" && anim.CurrentAnimation != "idle" && anim.CurrentAnimation != "" && !gettingHurt){
			//GD.Print('"'+anim.CurrentAnimation+'"'+" - "+attacking);
			if (!attacking && jumping){
				//GD.Print("Change");
				spr.Play("jump");
				anim.Play("jump");
				anim.Seek(.5);
			}
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("input_jump"+playerIndex) && IsOnFloor() && !gettingHurt){
			velocity.Y = JumpVelocity;
			spr.Play("jump");
			anim.Play("jump");
			jumping = true;
			if ( attacking ){
				attacking = false;
			}
		}

		if (gettingHit){
			velocity = DoGettingHit( velocity );
		}
		
		Velocity = velocity;
		MoveAndSlide();

	}

	protected void DoFacing(){
		wrapper.Scale = new Vector2( face_right?1:-1, 1 );
	}

	protected void DoAttack(){
		//GD.Print("Attack: {0}; {1}", msgCount++, playerIndex);
		//GD.Print("Attacking - "+this.GetType().Name);
		spr.Play("attack");
		anim.Play("attack");
		attacking = true;
		//GD.Print("Attacking - "+attacking);
	}

	public virtual void getHit( Boolean inDir ){
		gettingHit = true;
		gettingHit_direction = ( inDir )?-1:1;
	}

	protected Vector2 DoGettingHit( Vector2 velocity ){
		var hitForce = new Vector2( GD.RandRange( 300, 900 ) * -gettingHit_direction, -GD.RandRange( 300, 900 ) );
		velocity += hitForce;
		gettingHit = false; // initial impact
		gettingHurt = true;
		
		anim.Play("hit");
		spr.Play("hit");
		return velocity;
	}

	protected void _on_animation_player_animation_finished(String animStr){
		if (animStr == "attack"){
			//GD.Print("Stop Attack");
			anim.Stop(true);
			spr.Stop();
			attacking = false;
		}

		if (animStr == "hit"){
			anim.Stop(true);
			spr.Stop();
			gettingHurt = false;
			gettingHit = false;
		}
	}

	protected void _on_attack_body_entered(Node2D body)
	{	
		//GD.Print("contact "+attacking);
		if ( body.GetType().Name == "baddie" && attacking ){
			((baddie)body).getHit( face_right );
		}
		//GD.Print( body.GetType().Name );
	}

	private void _on_body_body_shape_entered(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index){
		//GD.Print("BODY CONTACT "+body.GetType().Name);
		
	}
}
