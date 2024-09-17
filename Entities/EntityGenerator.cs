using Godot;
using System;

public partial class EntityGenerator : Node2D
{
	[Export]
	public PackedScene Entities {get;set;}

	private int player_count = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SpawnEntity (){
//		baddie b = Baddies.Instantiate<baddie>();
//		b.Position = new Vector2( GD.RandRange(100,1500) , -1000 );
//		b.Scale = new Vector2(.5f,.5f);
//		AddChild(b);
	}

	public void SpawnPlayer (){
		Entity e = Entities.Instantiate<Entity>();
		AddChild(e);

//		baddie b = Baddies.Instantiate<baddie>();
//		b.Position = new Vector2( GD.RandRange(100,1500) , -1000 );
//		b.Scale = new Vector2(.5f,.5f);
//		AddChild(b);
	}

	private void _on_spawn_timer_timeout()
	{
		//SpawnBaddie();
		// Replace with function body.
	}		
}
