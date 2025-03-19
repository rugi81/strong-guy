using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class player_hud : Control
{
	
	private List<RichTextLabel> status = new List<RichTextLabel>();
	private RichTextLabel name;
	private TextureProgressBar healthBar;
	private int statusCount = 0;

	private Control gameHUD;
	private player_join_hud joinHUD;
	private debug_hud debugHUD;

	private Player p;
	public int p_index;
	private Boolean visibleHealthText = false;

	private Boolean playerActive = false;
	private Boolean selectingPlayer = false;
	private Boolean awaitingJoin = true; // defaults to join.

	[Export]
	private bool debugMode = true;
	
	[Signal]
	public delegate void HealthChangedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		name = GetNode<RichTextLabel>("GameHUD/TitleLabel");
		healthBar = GetNode<TextureProgressBar>("GameHUD/HealthBar");

		gameHUD = GetNode<Control>("GameHUD");
		joinHUD = GetNode<player_join_hud>("PlayerJoinHUD");
		joinHUD.playerIndex = p_index;

		debugHUD = GetNode<debug_hud>("DebugHUD");
		if ( debugMode ){
			debugHUD.Visible = true;
		} 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_health_changed(){
		var health = p.currentHealth;
		updateStatus( "Health", health.ToString() );
		healthBar.Value = health;
	}

	private void _on_health_zero(){
		//GD.Print("DEEEEEEEEEEEEEAD");
		GetNode<Sprite2D>("GameHUD/Thumbs/Dead").Visible = true;
		GetNode<Sprite2D>("GameHUD/Thumbs/"+p.getPlayerType()).Modulate = new Color(.5f,.5f,.5f);
	}

	private void _on_player_type_change(int playerType, Player p){
		changePlayerType( playerType, p );
	}

	private void _on_player_score_change(int score, Player p){
		updateStatus("Score", score.ToString());
	}

	private void changePlayerType(int playerType, Player p){
		GD.Print("PlayerHUD: player type change "+playerType);
		var c = GetNode<Control>("GameHUD/Thumbs").GetChildren();

		for ( var i=0; i<c.Count; i++ ){
			Sprite2D a = (Sprite2D) c[i];
			a.Visible = !!( a.Name == playerType.ToString() );
			GD.Print( a.Name + " == " + (playerType+1).ToString() + " - " +  !!( a.Name == (playerType+1).ToString() ) );
		}
	}

	public void playerLeft(){

	}

	public void requestJoin(){
		joinHUD.SelectAPlayer( true, p_index );
		
	}

	public void setActive( Boolean active ){
		playerActive = active;
		gameHUD.Visible = active;
		joinHUD.Visible = !active;
	}

	public void setPlayerHUD( String titleLabel ){
		name.Text = titleLabel;
	}
	public void setPlayerColor( Color inColor ){
		name.AddThemeColorOverride("default_color", new Color( inColor ) );
	}
	public void assignPlayer( Player inPlayer ){
		p = inPlayer;
		p.HealthChanged += _on_health_changed;
		p.HealthZero += _on_health_zero;
		p.PlayerTypeChange += _on_player_type_change;
		p.PlayerScoreChange += _on_player_score_change;

		createStatusLabel( "Health", p.currentHealth.ToString() );
		createStatusLabel( "Score", p.score.ToString() );
		healthBar.Value = p.currentHealth;
		joinHUD.playerIndex = p.getPlayerIndex();
		changePlayerType( p.getPlayerType(), p );

		debugHUD.SetPlayer(p);
	}

    private RichTextLabel getStatusByLabel( String statusLabel ){
		String label;
		for ( var i = 0; i < statusCount; i++ ){
			label = status[i].Name;
			if ( label == statusLabel ){
				return status[i];
			}
		}
		return null;
	}

	public void updateStatus( String statusLabel, String newValue ){
		RichTextLabel r = getStatusByLabel(statusLabel);
		if ( r != null ){
			r.Text = statusLabel + ": " + newValue;
		}else{
			createStatusLabel( statusLabel, newValue );
		}
	}

	public void createStatusLabel( String statusLabel, String newValue ){
		RichTextLabel rtl = new RichTextLabel();
		int fieldHeight = 20;

		rtl.Name = statusLabel;
		rtl.Text = statusLabel + ": " + newValue;
		rtl.Size = new Vector2( 100, 200 );
		rtl.Position = new Vector2( 60, statusCount * fieldHeight + 35);
		rtl.FitContent = true;
		rtl.AutowrapMode = TextServer.AutowrapMode.Off;

		if ( statusLabel == "Health" ){
			rtl.Visible = visibleHealthText;
		}

		GetNode("GameHUD").AddChild(rtl);

		status.Add(rtl);
		statusCount++;

		//GD.Print( status.Count );
	}
}
