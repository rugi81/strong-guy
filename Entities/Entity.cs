using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class Entity : CharacterBody2D
{
	// Object refs
	protected AnimatedSprite2D spr;
	protected AnimationPlayer anim;
	protected Node2D wrapper;

	[Export]
	protected int playerIndex = 0;

	[Export]
	protected int playerMaxHealth = 100;
	public int currentHealth;


	public const float Speed = 300.0f;
	public const float JumpVelocity = -550.0f;

	// State vars
	protected Boolean jumping = false;
	protected Boolean attacking = false;
	protected Boolean climbing = false;
	protected Boolean climbingUp = false;
	protected Boolean face_right;
	public Boolean gettingHit = false;
	public int gettingHit_direction = -1;
	public Boolean gettingHurt = false;
	protected Boolean dying = false;
	protected int connectedClimbables = 0;
	protected float climbableX;

	protected int msgCount = 0;

	protected String[] anim_names;
	protected Boolean friendlyFire = false;

	// Signals
    [Signal]
    public delegate void HealthChangedEventHandler();
	[Signal]
	public delegate void HealthZeroEventHandler();
	[Signal]
	public delegate void EntityDeathEventHandler( Vector2 inPos );

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	protected float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite2D>("Wrapper/AnimatedSprite2D");
		anim = GetNode<AnimationPlayer>("Wrapper/AnimationPlayer");
		wrapper = GetNode<Node2D>("Wrapper");

		currentHealth = playerMaxHealth;

		anim_names = spr.SpriteFrames.GetAnimationNames();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (dying){
			return;
		}

		Vector2 velocity = Velocity;

		if (!climbing){
			// Add the gravity.
			velocity.Y += gravity * (float)delta;
		}else{
			GD.Print( CollisionLayer );

			velocity = Vector2.Zero;
			Position = new Vector2( climbableX, Position.Y );
			spr.Play("climb");
			anim.Play("climb");

			spr.Pause();
			anim.Pause();

			GD.Print(GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect());
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.

		Vector2 direction = Input.GetVector("input_left"+playerIndex, "input_right"+playerIndex, "input_jump"+playerIndex, "input_down"+playerIndex);
		
		//GD.Print( "GettingHit: "+gettingHit+" - Hurting:"+gettingHurt );

		if (  Mathf.Round( direction.X * 100 )/100 != 0 )//Vector2.Zero)
		{
			if (!jumping && !attacking && !gettingHurt){					
				climbing = false;
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

			if (!jumping && !attacking && !gettingHurt && !climbing){
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

		// Climbing 
		if (Input.IsActionPressed("input_up"+playerIndex) && !gettingHurt && connectedClimbables > 0 ){
			climbing = true;
			velocity = Vector2.Zero;
			velocity.Y -= 500;
			climbingUp = true;
			spr.Play();
		}
		if (Input.IsActionPressed("input_down"+playerIndex) && !gettingHurt  && connectedClimbables > 0 ){
			climbing = true;
			velocity = Vector2.Zero;
			velocity.Y += 500;
			climbingUp = false;
			spr.Play();
		}
		if (connectedClimbables < 1){
			climbing = false;
		}
		// Handle Jump.
		if (Input.IsActionJustPressed("input_jump"+playerIndex) && ( IsOnFloor() || climbing ) && !gettingHurt){
			climbing = false;
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

		if ( currentHealth <= 0 ){
			currentHealth = 0;
			EmitSignal("HealthZero");
		}

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
	public virtual void getHit( Boolean inDir, int dmg ){
		this.getHit(inDir);
		currentHealth -= dmg;
		if ( currentHealth < 0 ){
			currentHealth = 0;
		}
		EmitSignal("HealthChanged");
		//GD.Print( "Player "+playerIndex+" -Health: "+currentHealth );
	}


	protected Vector2 DoGettingHit( Vector2 velocity ){
		var hitForce = new Vector2( GD.RandRange( 300, 900 ) * -gettingHit_direction, -GD.RandRange( 300, 900 ) );
		velocity += hitForce;
		gettingHit = false; // initial impact

		if ( attacking ){
			attacking = false;
		}

		anim.Play("hit");
		spr.Play("hit");
				
		if (gettingHurt){
			anim.Seek(.5);
		}else{
			gettingHurt = true;
		}
		
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

		if (animStr == "death"){
			EntityDie();
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

	protected void _on_body_body_shape_entered(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index){
		
		//GD.Print(GetType().Name);
		if ( this.GetType().Name == "Player" && body.GetType().Name == "TileMap" ){
			TileMap tm = (TileMap) body;
		
			Vector2I tpos = tm.GetCoordsForBodyRid( body_rid );
			TileData data = tm.GetCellTileData( 0, tpos );
			if ( (bool) data.GetCustomData("climbable") ){
				connectedClimbables++;
				climbableX = tm.MapToLocal( tpos ).X;
				//GD.Print( climbableX + " " + tm.MapToLocal( tpos ) + " " + tpos );
			}
		}
		
	}

	protected void _on_body_body_shape_exited(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index){

		if ( this.GetType().Name == "Player" && body.GetType().Name == "TileMap" ){
			TileMap tm = (TileMap) body;
			Vector2I tpos = tm.GetCoordsForBodyRid( body_rid );
			TileData data = tm.GetCellTileData( 0, tpos );
			GD.Print(data);
			if ( (bool) data.GetCustomData("climbable") ){
				connectedClimbables--;
			}
		}

	}

	protected void _on_health_zero(){
		//GD.Print("DEATH");

		var hasDeathAnim = Array.Exists( anim_names, e => e == "death" );

		if ( anim.HasAnimation("death") && hasDeathAnim ){
			//GD.Print("death anim");
			anim.Play("death");
			spr.Play("death");
		}else{
			//GD.Print("no death anim");
			EntityDie();
		}
		dying = true;
	}

	protected virtual void EntityDie(){
		EmitSignal("EntityDeath", Position, this);
		QueueFree();
		// spawn tombstone
	}

	public virtual void setPlayerIndex( int inIndex ){
		playerIndex = inIndex;
	}

	public virtual int getPlayerIndex( ){
		return playerIndex;
	}
}
