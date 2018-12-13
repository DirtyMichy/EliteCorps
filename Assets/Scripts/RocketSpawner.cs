using UnityEngine;
using System.Collections;

public class RocketSpawner : MonoBehaviour
{

	public bool hasTargeting = false;
	//does the unit has targeting?
	public bool targetAcquired = false;
	//units with targeting acquire targets
	public int bulletsPerShot = 1;
	//How many bullets are spawned per shot
    
	public float shotDelay;
	//Delay between shots
	public GameObject rocket;
	//The prefab of this ship's bullet

	bool firingStarted = false;
	//to ensure a coroutine isnt started twice

	protected Transform[] shotPositions;
	//Fire points on the ship

	public void StopShooting ()
	{
		StopCoroutine ("Shoot");
	}

	void Awake ()
	{       
		//StartCoroutine("Shoot");
		firingStarted = true;
	}

	void Update ()
	{       
		if (transform.position.y <= -5f && firingStarted) {
			firingStarted = false;
			StopCoroutine ("Shoot");
		}
	}

	//Coroutine for AI shooting
	IEnumerator Shoot ()
	{
		//Loop indefinitely
		while (true) {
			//If there is an acompanying audio, play it
			if (GetComponent<AudioSource> ())
				GetComponent<AudioSource> ().Play ();

			for (int j = 0; j < bulletsPerShot; j++) {
				Instantiate (rocket, transform.position, transform.rotation);
				Debug.Log ("Rocket");
			}

			//Wait for it to be time to fire another shot
			yield return new WaitForSeconds (shotDelay);
		}
	}
}
