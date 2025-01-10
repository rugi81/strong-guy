using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class Player : Entity
{
	[Signal]
	public delegate void PlayerHealthZeroEventHandler( Player p );

    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
        if (currentHealth <= 0){
            EmitSignal("PlayerHealthZero", this);
        }
    }
	

    private void _on_health_changed(){
        GD.Print("OUCH: "+currentHealth);
    }

    private void _on_player_health_zero( Player p ){
        // DIE
        //QueueFree();
    }
}
