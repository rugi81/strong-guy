using Godot;
using System;

public partial class MonsterGenerator : Node2D
{
	[Export]
	public PackedScene Baddies {get;set;}

	[Export]
	private int MaxEntities = 1;
	private int EntityCount = 0;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		SpawnBaddie();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void SpawnBaddie (){
		if ( EntityCount < MaxEntities ){
			baddie b = Baddies.Instantiate<baddie>();
			b.Position = new Vector2( GD.RandRange(100,1500) , -1000 );
			b.Scale = new Vector2(.5f,.5f);
			AddChild(b);
			EntityCount++;
		}
	}

	private void _on_spawn_timer_timeout()
	{
		SpawnBaddie();
		// Replace with function body.
	}	
	
}


