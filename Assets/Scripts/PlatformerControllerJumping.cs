using UnityEngine;
using System.Collections;


[System.Serializable]

public class PlatformerControllerJumping {

	// Can the character jump?
	public bool enabled = true;

	// How high do we jump when pressing jump and letting go immediately
	public float height = 1.0f;
	// We add extraHeight units (meters) on top when holding the button down longer while jumping
	public float extraHeight = 2.1f;
	
	// This prevents inordinarily too quick jumping
	// The next line, [System.NonSerialized] , tells Unity to not serialize the variable or show it in the inspector view.  Very handy for organization!
	[System.NonSerialized]
	public float repeatTime = 0.05f;

	[System.NonSerialized]
	public float timeout = 0.15f;

	// Are we jumping? (Initiated with jump button and not grounded yet)
	[System.NonSerialized]
	public bool jumping = false;
	
	// Reached the top of the jump
	[System.NonSerialized]
	public bool reachedApex = false;
  
	// Last time the jump button was clicked down
	[System.NonSerialized]
	public float lastButtonTime = -10.0f;
	
	// Last time we performed a jump
	[System.NonSerialized]
	public float lastTime = -1.0f;

	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	[System.NonSerialized]
	public float lastStartHeight = 0.0f;
	
	public PlatformerControllerJumping(){
	}
}
