using UnityEngine;
using System.Collections;

public class KillScript : MonoBehaviour 
{
	
	void OnTriggerStay(Collider other) {
		Debug.Log ("Collided!!!");
		if(other.gameObject.name == "Player"){
		
			PlatformerControllerCSharp script = other.gameObject.GetComponent<PlatformerControllerCSharp>();
			
			script.Spawn();
		}
	}
}

