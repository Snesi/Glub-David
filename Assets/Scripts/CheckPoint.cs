using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour 
{
	void OnTriggerEnter(Collider other) {
		Debug.Log ("Collided!!!");
		if(other.gameObject.name == "Player"){
		
			PlatformerControllerCSharp script = other.gameObject.GetComponent<PlatformerControllerCSharp>();
			
			Vector3 newPos = new Vector3(transform.position.x, transform.position.y, 3.6f);
			script.spawnPoint.position = newPos;
		}
	}
}


