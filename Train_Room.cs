using Godot;
using System;

public partial class Train_Room : Node2D
{
	[Export] private int size_x = 5;
	[Export] private int size_y = 3;
	[Export] private String wall_type = "";
	[Export] private Vector3[] exits; // x,y coords + exit type; 0 = hatch, 1 = door etc.
	[Export] private Boolean autogenerate = true;

	private Vector3I[] health;

	private TileMap tm;
	private TileMap bg;
	private TileSet ts; 


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tm = GetNode<TileMap>("Shell");
		bg = GetNode<TileMap>("Bg");

		if ( autogenerate ){

			// set walls
			int paint_x = 0,
				paint_y = 0;
			Vector2I paint_tile = new Vector2I(3,4);
			for ( int i = 0; i < (size_y*2); i++ ){
				if ( i > size_y-1 ){
					paint_x = size_x-1;
					paint_y = i - size_y;
					paint_tile.X = 1;
					paint_tile.Y = 4;
				}
				tm.SetCell(0, new Vector2I(paint_x,paint_y++), 0, paint_tile);
			}

			// set floors
			paint_x = 0;
			paint_y = 0;
			paint_tile.X = 0;
			paint_tile.Y = 4;
			for ( int i = 0; i < (size_x*2); i++ ){
				if ( i > size_x-1 ){
					paint_x = i - size_x;
					paint_y = size_y-1;
					paint_tile.X = 2;
					paint_tile.Y = 4;
				}
				tm.SetCell(1, new Vector2I(paint_x++,paint_y), 0, paint_tile);
			}

			// set exits
				// 0x = hatch; reinforced hatch etc
				// 1x = door; reinforced door etc

			// set background
				// has windows?
				// has pipes?
				// has panels?

		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void hitTile(){
		
	}
}
