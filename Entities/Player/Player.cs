using Godot;
using System;
using System.ComponentModel;
using System.Linq;

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
    protected string currentAttack = "normal";

    protected int playerType = 1;

    protected PlayerInput playerInput = new PlayerInput();

    protected string actions;

    protected bool dashing = false;
    protected float dashTimer = .2f;

    protected float actionTimer = 0;

    protected bool    comboActive     = false;
    protected float   comboTimer      = 0;
    protected float   comboMaxTime    = 2;
    protected int     comboCount      = 0;
    protected string  currentCombo    = "";
    protected string  lastAction        = "";
    protected bool    newAction       = false;
    protected bool    comboReady      = false;

    protected GpuParticles2D floorDust;


    public override void _Ready()
    { 
        autoMoveAndSlide = false;
        base._Ready();
        playerInput.SetPlayer(this);

        floorDust = GetNode<GpuParticles2D>("ParticleState/Floor dust");
    }

    public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
        actions = playerInput.processPlayerInput(delta);

        // movement abilities
        if ( !dashing ){
            if ( actions.EndsWith( "dd" ) || actions.EndsWith("aa") ){ // dash
                dashing = true;
                actionTimer = 0;
                playerInput.ClearActions();
                floorDust.Emitting = true;
            }
        }else if ( dashing ){
            floorDust.Emitting = true;
            int dir = face_right?1:-1;
            GD.Print( "::"+Velocity.X );
            Velocity = new Vector2( 2000 * dir * ((dashTimer - actionTimer)/dashTimer), Velocity.Y );
            GD.Print( "="+Velocity.X );
            actionTimer += (float) delta;
            if ( actionTimer > dashTimer ){
                dashing = false;
                floorDust.Emitting = false;
            }
        }

        // build-up action abilities
        // ongoing ability that only clears the action list at the end (if it clears)

        // combo attack
        // first attack starts attack count
        // 

        if ( actions.Length > 0 ){
            lastAction  = ""+actions[ actions.Length-1 ];
        }
    
        if ( comboActive ){
            comboTimer += (float) delta;
            if ( comboTimer > comboMaxTime ){
                currentCombo = "";
                comboCount = 0;
                comboTimer = 0;
                comboActive = false;
            }
        }

        if ( newAction && comboCount == 0 ){

            currentCombo = lastAction;
            comboCount++;
            comboTimer = 0;
            comboActive = true;
            newAction = false;

        }
        
        if ( newAction ){
            if ( comboCount == 1 ){

                if ( currentCombo == "A" ){
                    if ( lastAction == "A" ){
                        comboCount++;
                        comboTimer = 0;
                        // override current action with combo action
                        // override DoAttack?
                        // what if player presses too quick? need to queue action? OR does this extra press get ignored?

                    }
                }

            }else if ( comboCount == 2 ){

                if ( currentCombo == "A" ){
                    if ( lastAction == "A" ){
                        comboCount++;
                        comboTimer = 0;
                    }
                }

            }
        }


        // if comboIng
        //      comboTimer += delta
        //      if comboTimer > comboMaxTime
        //          resetCombo

        // if comboCount == 0
        //      currentCombo = lastAction
        //      comboTimer = 0
        //
        // if currentCombo = A (??)
        //      and THIS action = A
        //          and comboCount == ?,
        // then,
        //      currentCombo += thisAction
        //      comboCount++;
        //      do combo thing( comboCount )
        // else
        //      nada. resetCombo


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

    // ATTACK TYPES HERE
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

    protected Attack getAttackType( string attackName ){
        return new Attack( attackName, 10, new Vector2( GD.RandRange( 300, 900 ) * -gettingHit_direction, -GD.RandRange( 300, 900 ) ));
    }

    public void _on_animated_sprite_2d_frame_changed(){
        //GD.Print("BLAH");
        
        if ( attacking && spr.Animation == "attack" ){
            GD.Print( spr.Animation + " ~ "+ spr.Frame ); 
            if ( spr.Frame >= 2 ){
                comboReady = true; 
                GD.Print( "COMBO READY" );
            }
        }
    }

    
}

public partial class Attack {
    public string name;
    public float attackDamage;
    public Vector2 hitForce;

    public Attack( string inName, float inDmg, Vector2 inHF ){
        name = inName;
        attackDamage = inDmg;
        hitForce = inHF;
    }
}