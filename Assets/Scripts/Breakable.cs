using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider other) {
		Debug.Log ("Collided!!!");
		if(other.gameObject.name == "Player"){
		
			PlatformerControllerCSharp script = other.gameObject.GetComponent<PlatformerControllerCSharp>();
			
			if(script.state == PlayerState.Rock && script.jump.jumping) {
				Destroy(GameObject.Find("Branches"));
			}
		}
	}
}
