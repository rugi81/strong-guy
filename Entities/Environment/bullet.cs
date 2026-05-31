using Godot;
using System;

public partial class bullet : RigidBody2D
{
	[Export]
	private float speed = 1;

	[Export]
	public Godot.Vector2 direction;

	[Export]
	private int damage = 50;

	[Export]
	private int splash_damage = 10;

	[Export]
	private bool explode = true;
	[Export]
	private bool explode_on_contact = true; // explode on contact, or explode when life timer expires?
	[Export]
	private float explosion_size = 1.1f;
	[Export]
	private float explosion_time = 1;
	private float explosion_timer = 0;
	private bool exploding = false;


	[Export]
	private bool can_be_damaged = false;
	[Export]
	private bool blockable = true;

	[Export]
	private float life_time = 2; // -1 for infinite until it hits something.
	private float life_timer = 0;

	[Export]
	private int health = 5; 


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LookAt( direction );
		LinearVelocity = speed * direction;
	}

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
		if ( !exploding ){
			base._IntegrateForces(state);
			Rotation = LinearVelocity.Normalized().Angle();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if ( exploding )
		{
			explosion_timer += (float)delta;
			Scale = Scale * explosion_size;
			if ( explosion_timer > explosion_time)
			{
				QueueFree();
			}
		}else{
			life_timer += (float)delta;

			if ( life_timer > life_time)
			{
				if ( explode )
					doExplode();
				else
					QueueFree();
			}
		}
	}

	// ** 

	private void doExplode()
	{
		exploding = true;
		LinearVelocity = Godot.Vector2.Zero;
		GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "explode";
	}

	// ** listeners / emitters

	protected void _on_body_entered(Node2D body)
	{
		GD.Print("KKK");
	}

	protected void _on_body_shape_entered(Rid body_rid, Node2D body, int body_shape_index, int local_shape_index)
	{
		GD.Print("!!");
		if ( explode && explode_on_contact)
		{
			doExplode();
		}
	}
}
