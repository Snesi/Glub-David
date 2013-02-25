using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object

public enum PlayerState {
	Normal,
	Rock,
	Squirrel,
	Bird,
	Bottle,
}

[RequireComponent (typeof(CharacterController))]
[AddComponentMenu ("2D Platformer/Platformer Controller CSharp")]


public class PlatformerControllerCSharp : MonoBehaviour {
	
	public PlayerState state = PlayerState.Normal;
	public bool canControl = true;
	public Transform spawnPoint;
	public PlatformerControllerMovement movement;
	public PlatformerControllerJumping jump;
	//public PlatformerControllerAbsorbing absorb;


	private CharacterController controller;

	// Moving Platform support.
	private Transform activePlatform;
	private Vector3 activeLocalPlatformPoint;
	private Vector3 activeGlobalPlatformPoint;
	private Vector3 lastPlatformVelocity;
	
	//private bool absorbing = false;

	// This is used to keep track of special effects in UpdateEffects ();
	//private bool areEmittersOn = false;

	// Use this for initialization
	void Start () {
		//movement = new PlatformerControllerMovement();
		//jump = new PlatformerControllerJumping();
		//absorb = new PlatformerControllerAbsorbing();
		movement.direction = transform.TransformDirection(Vector3.forward);
		//absorb.original_mesh = GetComponent<MeshFilter>().mesh;
		//absorb.original_collider_mesh = GetComponent<MeshCollider>().sharedMesh;
		controller = GetComponent<CharacterController>();
		Spawn();
	}

	void Spawn() {
		// reset character speed
		movement.verticalSpeed = 0.0f;
		movement.speed = 0.0f;

		// reset character position to spawnPoint
		transform.position = spawnPoint.position;
		//transform.rotation = spawnPoint.rotation;
	}

	void onDeath() {
		Spawn();
	}

	void UpdateSmoothMovementDirection(){
		float h = Input.GetAxisRaw("Horizontal");

		if(!canControl) {
			h = 0.0f;
		}

		movement.isMoving = Mathf.Abs(h) > 0.1f;

		if(movement.isMoving) {
			movement.direction = new Vector3(h, 0, 0);
		}

		// Ground controls
		if(controller.isGrounded) {
			// Smooth the speed based on the current target direction
			float curSmooth = movement.speedSmoothing * Time.deltaTime;
			
			// Choose target speed
			float targetSpeed = Mathf.Min (Mathf.Abs(h), 1.0f);
		
			// Pick speed modifier
			if (Input.GetButton ("Fire2") && canControl)
				targetSpeed *= movement.runSpeed;
			else
				targetSpeed *= movement.walkSpeed;
			
			movement.speed = Mathf.Lerp (movement.speed, targetSpeed, curSmooth);
			
			movement.hangTime = 0.0f;
		}
		else {
			// In air controls
			movement.hangTime += Time.deltaTime;
			if(movement.isMoving) {
				movement.inAirVelocity += (new Vector3(Mathf.Sign(h), 0, 0)) * Time.deltaTime * movement.inAirControlAcceleration;
			}
		}
	}

	void FixedUpdate() {
		// Make sure we are absolutely always in the 2D plane.
		//transform.position.z = 0.0f;
	}

	void ApplyJumping() {
		// Prevent jumping too fast after each other
		if(jump.lastTime + jump.repeatTime > Time.time) {
			return;
		}

		if(controller.isGrounded) {
			// Jump
			// - Only when pressing the button down
			if(jump.enabled && Time.time < jump.lastButtonTime + jump.timeout) {
				movement.verticalSpeed = CalculateJumpVerticalSpeed(jump.height);
				movement.inAirVelocity = lastPlatformVelocity;
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void ApplyGravity() {
		// Apply gravity
		bool jumpButton = Input.GetButton("Jump");

		if(!canControl){
			jumpButton = false;
		}

		// When we reach the apex of the jump we send out a message
		if(jump.jumping && !jump.reachedApex && movement.verticalSpeed <= 0.0) {
			jump.reachedApex = true;
			SendMessage ("DidReachApex", SendMessageOptions.DontRequireReceiver);
		}

		// * When jumping up we don't apply gravity for some time when the user is holding the jump button
		//   This gives more control over jump height by pressing the button longer
		bool extraPowerJump = jump.jumping && movement.verticalSpeed > 0.0 && jumpButton && transform.position.y < jump.lastStartHeight + jump.extraHeight && !IsTouchingCeiling();

		if(extraPowerJump) {
			return;
		}
		else if (controller.isGrounded){
			movement.verticalSpeed = -movement.gravity * Time.deltaTime;
		}
		else {
			movement.verticalSpeed -= movement.gravity * Time.deltaTime;
		}

		// Make sure we don't fall any faster than maxFallSpeed.  This gives our character a terminal velocity.
		movement.verticalSpeed = Mathf.Max (movement.verticalSpeed, -movement.maxFallSpeed);
	}

	void ApplyAbsorbation(GameObject oponent) {
		
		if (Input.GetButtonDown("Fire3")) {
			
			if(oponent.tag == "Rock"){
				Debug.Log ("In function!");
				
				state = PlayerState.Rock;
				
				GameObject aux = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				MeshFilter mf1 = GetComponent (typeof(MeshFilter)) as MeshFilter;
				MeshFilter mf2 = aux.GetComponent(typeof(MeshFilter)) as MeshFilter;
				//Debug.Log ("Number of vertices before change: " + mf1.mesh.vertexCount);
				
				mf1.mesh = mf2.mesh;
				
				mf1.mesh.RecalculateNormals();
				mf1.mesh.RecalculateBounds();
				
				//Debug.Log ("Number of vertices after change: " + mf1.mesh.vertexCount);
				Destroy(oponent);
				
				movement.walkSpeed = 3f;
				movement.runSpeed = 5f;
				jump.height = 0.5f;
				jump.extraHeight = 1f;
				
			}
			
		}
		
	}
	
	void ApplySpit() {
		bool spit = Input.GetButtonDown("Fire3");
		if(spit && state != PlayerState.Normal) {
			
			state = PlayerState.Normal;
			
			GameObject oldBody = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			Rigidbody rb;
			rb = ball.AddComponent("Rigidbody") as Rigidbody;
			
			MeshFilter mf1 = GetComponent(typeof(MeshFilter)) as MeshFilter;
			MeshFilter mf2 = oldBody.GetComponent(typeof(MeshFilter)) as MeshFilter;
			
			mf1.mesh = mf2.mesh;
			
			ball.tag = "Rock";
			
			ball.transform.position = transform.position;
			
			ball.rigidbody.AddForce(new Vector3(movement.direction.x*5f, 10f, 0f), ForceMode.Impulse);
			
			movement.walkSpeed = 5.0f;
			movement.runSpeed = 10.0f;
			jump.height = 1.0f;
			jump.extraHeight = 2.1f;
			
		}
	}

	float CalculateJumpVerticalSpeed (float targetJumpHeight) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
	}

	void DidJump () {
		jump.jumping = true;
		jump.reachedApex = false;
		jump.lastTime = Time.time;
		jump.lastStartHeight = transform.position.y;
		jump.lastButtonTime = -10;
	}

	/*function UpdateEffects () {
		wereEmittersOn = areEmittersOn;
		areEmittersOn = jump.jumping && movement.verticalSpeed > 0.0;
		
		// By comparing the previous value of areEmittersOn to the new one, we will only update the particle emitters when needed
		if (wereEmittersOn != areEmittersOn) {
			for (var emitter in GetComponentsInChildren (ParticleEmitter)) {
				emitter.emit = areEmittersOn;
			}
		}
	}*/

	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Jump") && canControl) {
			jump.lastButtonTime = Time.time;
		}

		UpdateSmoothMovementDirection();
		
		ApplySpit();

		// Apply gravity
		// - extra power jump modifies gravity
		ApplyGravity();

		// Apply jumping logic
		ApplyJumping();

		// Apply absorbation
		//ApplyAbsorbation();

		// Moving platform support
		if(activePlatform != null) {
			Vector3 newGlobalPlatformPoint = activePlatform.TransformPoint(activeLocalPlatformPoint);
			Vector3 moveDistance = (newGlobalPlatformPoint - activeGlobalPlatformPoint);
			transform.position = transform.position + moveDistance;
			lastPlatformVelocity = (newGlobalPlatformPoint - activeGlobalPlatformPoint) / Time.deltaTime;
		}
		else {
			lastPlatformVelocity = Vector3.zero;
		}

		activePlatform = null;

		// Save lastPosition for velocity calculation.
		Vector3 lastPosition = transform.position;

		// Calculate actual motion
		Vector3 currentMovementOffset = movement.direction * movement.speed + new Vector3(0, movement.verticalSpeed, 0) + movement.inAirVelocity;

		// We always want the movement to be framerate independent. Multiplying by Time.deltaTime does this.
		currentMovementOffset *= Time.deltaTime;

		// Move our character!
		movement.collisionFlags = controller.Move (currentMovementOffset);

		// Calculate the velocity based on the current and previous position.
		// This means our velocity will only be the amount the character actually moved as a result of collisions
		movement.velocity = (transform.position - lastPosition) / Time.deltaTime;

		// Moving platforms support
		if (activePlatform != null){
			activeGlobalPlatformPoint = transform.position;
			activeLocalPlatformPoint = activePlatform.InverseTransformPoint(transform.position);
		}

		// Set rotation to the move direction
		if (movement.direction.sqrMagnitude > 0.01) {
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (movement.direction), Time.deltaTime * movement.rotationSmoothing);
		}

		// We are in jump mode but just became grounded
		if (controller.isGrounded) {
			movement.inAirVelocity = Vector3.zero;
			if (jump.jumping) {
				jump.jumping = false;
				SendMessage ("DidLand", SendMessageOptions.DontRequireReceiver);

				Vector3 jumpMoveDirection = movement.direction * movement.speed + movement.inAirVelocity;
				if(jumpMoveDirection.sqrMagnitude > 0.01) {
					movement.direction = jumpMoveDirection.normalized;
				}
			}
		}

		//UpdateEffects();
	}
	
	public float pushPower = 2.0F;

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.moveDirection.y > 0.01) {
			return;
		}

		// Make sure we are really standing on a straight platform
		// Not on the underside of one and not falling down from it either
		if(hit.moveDirection.y < -0.9 && hit.normal.y > 0.9) {
			activePlatform = hit.collider.transform;
		}

		// Check if it's the bottle
		if(hit.gameObject.name == "Ampolla") {
			ApplyAbsorbation(hit.gameObject);
		}
		
		ApplyAbsorbation(hit.gameObject);
	}

	// Various helper functions below:
	float GetSpeed () {
		return movement.speed;
	}

	Vector3 GetVelocity () {
		return movement.velocity;
	}


	bool IsMoving () {
		return movement.isMoving;
	}

	bool IsJumping () {
		return jump.jumping;
	}

	bool IsTouchingCeiling () {
		return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}

	Vector3 GetDirection () {
		return movement.direction;
	}

	float GetHangTime() {
		return movement.hangTime;
	}

	void Reset () {
		gameObject.tag = "Player";
	}

	void SetControllable (bool controllable) {
		canControl = controllable;
	}
}
	


