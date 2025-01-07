using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class player_hud : Control
{
	
	private List<RichTextLabel> status = new List<RichTextLabel>();
	private RichTextLabel name;
	private int statusCount = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		name = GetNode<RichTextLabel>("TitleLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void setPlayerHUD( String titleLabel ){
		name.Text = titleLabel;
	}
	public void setPlayerColor( Color inColor ){
		name.AddThemeColorOverride("default_color", new Color( inColor ) );
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
		rtl.Position = new Vector2( 0, statusCount * fieldHeight + 25);
		rtl.FitContent = true;
		rtl.AutowrapMode = TextServer.AutowrapMode.Off;

		AddChild(rtl);

		status.Add(rtl);
		statusCount++;

		GD.Print( status.Count );
	}
}
