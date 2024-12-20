using Godot;
using System;

public partial class baddie : Entity
{

	public Boolean dir;
	public Boolean gettingHit = false;
	public int gettingHit_direction = -1;
	public float speed = .5f;
	

	private int health;
	private int mana;
	private int energy;

	private AnimatedSprite2D anim;
 	private Timer revertColorTimer;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("Wrapper/AnimatedSprite2D");
		revertColorTimer = GetNode<Timer>("RevertColor");
		GD.Print( "baddie: "+GetTree().Root.Name );

		var r = GD.RandRange( 0, 1 );
		dir = ( r > .5 );
		//GD.Print( dir );
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		var r = GD.RandRange( 0, 100 );
		if ( r > 99 && IsOnFloor() ){
			velocity.Y = JumpVelocity;
		}

		var d = ( dir )?100:-100;
		Vector2 direction = new Vector2( d,0 );
		if (IsOnFloor()){

			velocity.X = Mathf.MoveToward(Velocity.X, Speed * direction.X, speed);
			anim.Animation = "walk";

		}

		if (gettingHit){
		
		GD.Print( velocity );
		   var hitForce = new Vector2( GD.RandRange( 300, 900 ) * -gettingHit_direction, -GD.RandRange( 300, 900 ) );
			velocity += hitForce;
			GD.Print( hitForce +" * "+ gettingHit_direction );
			//velocity.X += -6000;
			gettingHit = false;			
		GD.Print( velocity );
		}

		
		Velocity = velocity;
		
		//GD.Print(Position);
/*		if (Position.X < 200){
			Position = new Vector2(200, Position.Y);
		}
		if (Position.X > 1124){
			Position = new Vector2(1124, Position.Y);
		}*/
		if (Position.Y > 2000){
			GD.Print("eek");
			QueueFree();
		}
		
		MoveAndSlide();
		
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			//GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}		
	}
	
	public void getHit( Boolean inDir ){
		//GD.Print( "getHit: "+inDir );
		gettingHit = true;
		gettingHit_direction = ( inDir )?-1:1;
		anim.Modulate = new Color(1,0,0);
		revertColorTimer.Start();
	}
	
	private void _on_change_mind_timeout()
	{
		var r = GD.RandRange( 0, 1 );
		dir = ( r > .5 );
		// Replace with function body.
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
		anim.Modulate = new Color(1,1,1);
	}

}
