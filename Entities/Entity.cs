using Godot;
using System;

public partial class Entity : CharacterBody2D
{
	// Object refs
	private AnimatedSprite2D spr;
	private Node2D wrapper;

	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	// State vars
	private Boolean jumping = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite2D>("Wrapper/AnimatedSprite2D");
		wrapper = GetNode<Node2D>("Wrapper");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;
		else
			jumping = false;

		// Handle Jump.
		if (Input.IsActionJustPressed("input_jump") && IsOnFloor()){
			velocity.Y = JumpVelocity;
			spr.Play("jump");
			jumping = true;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.

		Vector2 direction = Input.GetVector("input_left", "input_right", "input_up", "input_down");
		if (direction != Vector2.Zero)
		{
			if (!jumping)
				spr.Play("walk");
			velocity.X = direction.X * Speed;
			wrapper.Scale = new Vector2( direction.Normalized().X, 1 );
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);

			if (!jumping)
				spr.Play("idle");
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
