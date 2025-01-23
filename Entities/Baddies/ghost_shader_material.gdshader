shader_type spatial;
uniform bool dying = false;
varying vec2 tex_position;
void vertex() {
	// Called for every vertex the material is visible on.
	if ( !dying ){
	  	//VERTEX += vec3(cos(TIME)*30.0, sin(TIME*2.0)*30.0-20.0);
	}else{
		VERTEX.x += cos(TIME*100.0)*30.0;
  VERTEX.y += cos(VERTEX.x * 4.0) * sin(VERTEX.z * 4.0);
		VERTEX.z += cos(TIME*100.0)*30.0;
	}
}

void fragment() {
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
