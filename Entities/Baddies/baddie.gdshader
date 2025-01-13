shader_type canvas_item;

uniform bool dying = false;

void vertex() {
	// Called for every vertex the material is visible on.
	// Animate Sprite2D moving in big circle around its location
	
	if ( !dying ){
	  	VERTEX += vec2(cos(TIME)*30.0, sin(TIME*2.0)*30.0-20.0);
	}else{
		VERTEX.x += cos(TIME*100.0)*30.0;
	}
}

void fragment() {
	// Called for every pixel the material is visible on.
	//COLOR = texture( TEXTURE, UV );
	COLOR.a = COLOR.a * 0.6;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
