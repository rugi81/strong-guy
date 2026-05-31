using Godot;
using System;
using System.Numerics;

public partial class Simon : Player
{
    public string HelloBolts = "Hello Bolts!";
    private Godot.Vector2 Aim;
	[Export]
	private PackedScene Projectile {get;set;}
    [Export]
    private float fire_rate = 0.25f;

    private float fire_timer = 0;

    public override void _Ready()
    {
        canDash = false;
        base._Ready();
        DoFacing();
    }

    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);

		Godot.Vector2 direction = Input.GetVector("input_aimleft" + playerIndex, "input_aimright" + playerIndex, "input_aimup" + playerIndex, "input_aimdown" + playerIndex);
        Node2D g = GetNode<Node2D>("Wrapper/Avatar/Waist/Upper Body/Gun");

        if ( direction != Godot.Vector2.Zero ){
            float xdir = (face_right)? direction.X:-direction.X;
            Aim = new Godot.Vector2( xdir, direction.Y );
            g.Rotation = Aim.Angle();
        }
        else
        {             
            Aim = Godot.Vector2.FromAngle(0);
            g.Rotation = 0;
        }

//        GD.Print( "=PI? "+ (Aim == Math.PI) );

        lastAction = playerInput.GetLastAction();

        fire_timer += (float)delta;

        if (Input.IsActionPressed("input_rtrigger" + playerIndex) && fire_timer > fire_rate)
        {
            bullet b = Projectile.Instantiate<bullet>();
            b.Position = this.Position;
            GetParent().AddChild(b);

            
            b.direction = direction;
            b.Transform.Scaled( new Godot.Vector2(.1f,.1f) );
            b.Rotation = direction.Angle();            
            b.LinearVelocity = (direction * 1000);

            fire_timer = 0;
        }
    }
}