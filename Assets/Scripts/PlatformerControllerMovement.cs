using UnityEngine;
using System.Collections;
[System.Serializable]
public class PlatformerControllerMovement {
	// The speed when walking 
	public float walkSpeed = 5.0f;
	// when pressing "Fire1" button (control) we start running
	public float runSpeed = 10.0f;

	public float inAirControlAcceleration = 1.0f;

	// The gravity for the character
	public float gravity = 60.0f;
	public float maxFallSpeed = 30.0f;

	// How fast does the character change speeds?  Higher is faster.
	public float speedSmoothing = 5.0f;

	// This controls how fast the graphics of the character "turn around" when the player turns around using the controls.
	public float rotationSmoothing = 10.0f;

	// The current move direction in x-y.  This will always been (1,0,0) or (-1,0,0)
	// The next line, @System.NonSerialized , tells Unity to not serialize the	floatiable or show it in the inspector view.  Very handy for organization!
	[System.NonSerialized]
	public Vector3 direction = Vector3.zero;

	// The current vertical speed
	[System.NonSerialized]
	public float verticalSpeed = 0.0f;

	// The current movement speed.  This gets smoothed by speedSmoothing.
	[System.NonSerialized]
	public float speed = 0.0f;

	// Is the user pressing the left or right movement keys?
	[System.NonSerialized]
	public bool isMoving = false;

	// The last collision flags returned from controller.Move
	[System.NonSerialized]
	public CollisionFlags collisionFlags; 

	// We will keep track of an approximation of the character's current velocity, so that we return it from GetVelocity () for our camera to use for prediction.
	[System.NonSerialized]
	public Vector3 velocity;
	
	// This keeps track of our current velocity while we're not grounded?
	[System.NonSerialized]
	public Vector3 inAirVelocity = Vector3.zero;

	// This will keep track of how long we have we been in the air (not grounded)
	[System.NonSerialized]
	public float hangTime = 0.0f;
	
	public PlatformerControllerMovement(){
	}
}
