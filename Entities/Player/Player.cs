using Godot;
using System;
using System.ComponentModel;
//using System.Numerics;
using System.Runtime.CompilerServices;

public partial class Player : Entity
{
	[Signal]
	public delegate void PlayerHealthZeroEventHandler( Vector2 inPos, Player p );
	[Signal]
	public new delegate void EntityDeathEventHandler( Vector2 inPos, Player p );
    [Signal]
    public delegate void PlayerTypeChangeEventHandler( int playerType, Player p );
    [Signal]
    public delegate void PlayerScoreChangeEventHandler( int scoreChange, Player p );

    [Export]
    public PackedScene tombStone;
    [Export]
    protected int attackDamage = 10;
    protected int playerType = 1;

    protected PlayerInput playerInput = new PlayerInput();

    protected string actions;

    protected bool dashing = false;
    protected float dashTimer = .2f;

    protected float actionTimer = 0;

    public override void _Ready()
    { 
        autoMoveAndSlide = false;
        base._Ready();
        playerInput.SetPlayer(this);
    }

    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
        actions = playerInput.processPlayerInput(delta);

        if ( !dashing ){
            if ( actions.EndsWith( "dd" ) || actions.EndsWith("aa") ){ // dash
                dashing = true;
                actionTimer = 0;
                playerInput.ClearActions();
            }
        }else if ( dashing ){
            int dir = face_right?1:-1;
            GD.Print( "::"+Velocity.X );
            Velocity = new Vector2( 2000 * dir * ((dashTimer - actionTimer)/dashTimer), Velocity.Y );
            GD.Print( "="+Velocity.X );
            actionTimer += (float) delta;
            if ( actionTimer > dashTimer ){
                dashing = false;
            }
        }

        //

        if (Position.Y > 2000){
			//GD.Print("eek");
			currentHealth = 0;
		    EmitSignal("HealthChanged");
		}

        if (currentHealth <= 0 && !dying){
            EmitSignal("PlayerHealthZero", this);
        }

        MoveAndSlide();
    }
	

    protected override void EntityDie(){
        EmitSignal( "EntityDeath", Position, this );
        //GD.Print( GetParent() );

        tombstone ts = tombStone.Instantiate<tombstone>();
        ts.Position = Position;
        ts.setPlayer( this.playerIndex );
        GetParent().AddChild( ts );
        QueueFree();
    }

    public int getPlayerType(){
        return playerType;
    }

    public void setPlayerType( int inType ){
        playerType = inType;
        changePlayer();
    }

    private void changePlayer(){
        // 1 = strong guy
        // 2 = norm
        // 3 = the robut

        switch ( playerType ){
            case 1: {
                Scale = new Vector2(.7f, .7f);
                spr.SpriteFrames = (SpriteFrames) ResourceLoader.Load("res://Entities/Player/strongGuy.tres");
                break;
            }
            case 2: {
                Scale = new Vector2(.7f, .7f);
                spr.SpriteFrames = (SpriteFrames) ResourceLoader.Load("res://Entities/Player/baseEntity.tres");
                spr.Scale = new Vector2(1,1);

                break;
            }
            case 3: {
                Scale = new Vector2(.5f, .5f);
                spr.SpriteFrames = (SpriteFrames) ResourceLoader.Load("uid://ckbvojvmqqqce");
                spr.Scale = new Vector2(1,1);
                break;
            }

        }
    }

    private void _on_health_changed(){
        GD.Print("OUCH: "+currentHealth);
    }

    private void _on_player_health_zero( Vector2 inPos, Player p ){
        // DIE
        //QueueFree();
    }

	protected new void _on_attack_body_entered(Node2D body)
	{	
		

        if ( friendlyFire ){
    		if ( body.GetType().Name == "Entity" && attacking ){
	    		((Entity)body).getHit( face_right, attackDamage, (Entity) this );
    		}
	    	if ( body.GetType().Name == "Player" && attacking ){
		    	((Player)body).getHit( face_right, attackDamage, (Entity) this );
		    }
        }

   		if ( body.GetType().Name == "baddie" && attacking ){
    		((baddie)body).getHit( face_right, attackDamage, (Entity) this );
   		}
		
	}   

	public override void changeScore( int scoreChange ){
		score += scoreChange;
        EmitSignal("PlayerScoreChange", score, this );
	}    

}
