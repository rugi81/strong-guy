using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class baddie : Entity
{

	public Boolean dir;
	public new Boolean gettingHit = false;
	public new int gettingHit_direction = -1;
	public float speed = .5f;
	protected Boolean attackPlayer = false;
	protected Boolean canAttack = true;
	private float deathRoll;

	[Export]
	protected int attackDamage = 5;
	protected new int currentHealth = 30;

	private int health;
	private int mana;
	private int energy;

	//private AnimatedSprite2D anim;
 	private Timer revertColorTimer;
	protected Timer attackDelay;

	//private Boolean jumping;

	public override void _Ready()
	{		
		//anim = GetNode<AnimatedSprite2D>("Wrapper/AnimatedSprite2D");
		revertColorTimer = GetNode<Timer>("RevertColor");
		attackDelay = GetNode<Timer>("Attack Delay");
		GD.Print( "baddie: "+GetTree().Root.Name );

		var r = GD.RandRange( 0, 1 );
		dir = ( r > .5 );
		//GD.Print( dir );

		base._Ready();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;
		else if (IsOnFloor())
			jumping = false;

		// Handle Jump.
		var r = GD.RandRange( 0, 100 );
		if ( r > 99 && IsOnFloor() && !attackPlayer && !attacking && !dying ){
			anim.Play("jump");
			spr.Play("jump");
			velocity.Y = JumpVelocity;
			jumping = true;
		}

		var d = ( face_right )?100:-100;
		if ( attackPlayer && !dying ){
			d *= 10;
		}
		Vector2 direction = new Vector2( d,0 );

		if (IsOnFloor() && !jumping && !dying ){

			if ( attackPlayer )
				velocity.X = Mathf.MoveToward(Velocity.X, Speed * direction.X, speed * 20);
			else
				velocity.X = Mathf.MoveToward(Velocity.X, Speed * direction.X, speed);

			if ( !attacking && !attackPlayer ){
				anim.Play("walk");
				spr.Play("walk");
			}

		}

		if (gettingHit){
		   var hitForce = new Vector2( GD.RandRange( 300, 900 ) * -gettingHit_direction, -GD.RandRange( 300, 900 ) );
			velocity += hitForce;
			gettingHit = false;			
		}

		//GD.Print( attackPlayer );
		if (!gettingHit && !attacking && attackPlayer && canAttack && !dying){
			//face the player
			canAttack = false;
			DoAttack();
		}
		
		Velocity = velocity;
		
		//GD.Print(Position);
/*		if (Position.X < 200){
			Position = new Vector2(200, Position.Y);
		}
		if (Position.X > 1124){
			Position = new Vector2(1124, Position.Y);
		}*/

		if (dying){
			Rotation += deathRoll;
			Scale -= new Vector2(.005f, .005f);
		}

		if (Position.Y > 2000){
			//GD.Print("eek");
			QueueFree();
		}
		
		MoveAndSlide();
		
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			//GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}		

		if ( currentHealth <= 0 && !dying ){
			currentHealth = 0;
			EmitSignal("HealthZero");
		}
	}
	
	public void setHealthAndDamage( int inHealth, int inDamage ){
		currentHealth = inHealth;
		attackDamage = inDamage;
	}

	public override void getHit( Boolean inDir ){
		//GD.Print( "getHit: "+inDir );
		gettingHit = true;
		gettingHit_direction = ( inDir )?-1:1;
		spr.Modulate = new Color(1,0,0);
		revertColorTimer.Start();
	}
    public override void getHit(bool inDir, int dmg)
    {
		GD.Print("baddie OW");
        getHit(inDir);
		currentHealth -= dmg;
    }

    private void _on_change_mind_timeout()
	{
		if (attackPlayer){ return; } // don't want to change our mind if we're mid attack
		var r = GD.RandRange( 0, 1 );
		dir = ( r > .5 );
		face_right = !(dir);
		wrapper.Scale = new Vector2( face_right?1:-1, 1 );

	}
	
	private void _on_player_punch_target(bool direction, double power)
	{
		//GD.Print("SIGNAL PUNCH");
		getHit( direction );
		// Replace with function body.
	}
	
	private void _on_revert_color_timeout()
	{
		// Replace with function body.
		spr.Modulate = new Color(1,1,1);
	}

	private void _on_attack_delay_timeout(){
		if (!canAttack){
			canAttack = true;
		}
	}

	private void _on_attack_range_body_shape_entered(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index){
		attackPlayer = true;

		if ( body.GetType().Name == "Entity" || body.GetType().Name == "Player" ){
			face_right = !( body.Position.X < this.Position.X );
			DoFacing();
		}
	}

	private void _on_attack_range_body_shape_exited(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index){
		attackPlayer = false;
	}
	

	protected new void _on_attack_body_entered(Node2D body)
	{	
		//GD.Print("baddie contact "+body.GetType().Name);
		if ( body.GetType().Name == "Entity" && attacking ){
			((Entity)body).getHit( face_right, attackDamage );
		}
		if ( body.GetType().Name == "Player" && attacking ){
			((Player)body).getHit( face_right, attackDamage );
		}
		//GD.Print( body.GetType().Name );
	}

	protected new void _on_animation_player_animation_finished(String animStr){
		base._on_animation_player_animation_finished(animStr);

		if (animStr == "attack"){
			
		}
	}

	protected override void EntityDie(){
		EmitSignal("EntityDeath", Position, this);
		dying = true;
		deathRoll = (float) GD.RandRange(-1f,.1f);
		GetNode<Timer>("Death Delay").Start(0);
		GD.Print(GetNode<Timer>("Death Delay"));
		//((ShaderMaterial)spr.Material).SetShaderParameter("dying", true);
	}

	protected void EntityDie( Boolean inDying ){
		GD.Print("DEATH TIMER DONE");
		if ( inDying ){
			QueueFree();
		}
	}

	private void _on_death_delay_timeout(){
		GD.Print("DEATH TIMER");
		EntityDie(dying);
	}	

}
