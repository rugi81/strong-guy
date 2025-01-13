using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class Player : Entity
{
	[Signal]
	public delegate void PlayerHealthZeroEventHandler( Vector2 inPos, Player p );
	[Signal]
	public new delegate void EntityDeathEventHandler( Vector2 inPos, Player p );

    [Export]
    public PackedScene tombStone;
    [Export]
    protected int attackDamage = 10;

    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
        if (currentHealth <= 0 && !dying){
            EmitSignal("PlayerHealthZero", this);
        }
    }
	

    private void _on_health_changed(){
        GD.Print("OUCH: "+currentHealth);
    }

    private void _on_player_health_zero( Vector2 inPos, Player p ){
        // DIE
        //QueueFree();
    }

    protected override void EntityDie(){
        EmitSignal( "EntityDeath", Position, this );
        GD.Print( GetParent() );

        tombstone ts = tombStone.Instantiate<tombstone>();
        ts.Position = Position;
        ts.setPlayer( this.playerIndex );
        GetParent().AddChild( ts );
        QueueFree();
    }

	protected new void _on_attack_body_entered(Node2D body)
	{	
		

        if ( friendlyFire ){
    		if ( body.GetType().Name == "Entity" && attacking ){
	    		((Entity)body).getHit( face_right, attackDamage );
    		}
	    	if ( body.GetType().Name == "Player" && attacking ){
		    	((Player)body).getHit( face_right, attackDamage );
		    }
        }

   		if ( body.GetType().Name == "baddie" && attacking ){
    		((baddie)body).getHit( face_right, attackDamage );
   		}
		
	}    
}
