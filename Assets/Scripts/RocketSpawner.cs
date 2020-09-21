using UnityEngine;
using System.Collections;

public class RocketSpawner : MonoBehaviour
{
	public bool hasTargeting = false;
	public bool targetAcquired = false;
	public int bulletsPerShot = 1;    
	public float shotDelay;
	public GameObject rocket;
	bool firingStarted = false;
	protected Transform[] shotPositions;

	public void StopShooting ()
	{
		StopCoroutine ("Shoot");
	}

	void Awake ()
	{       
		firingStarted = true;
	}

	void Update ()
	{       
		if (transform.position.y <= -5f && firingStarted) {
			firingStarted = false;
			StopCoroutine ("Shoot");
		}
	}

	IEnumerator Shoot ()
	{
		while (true) {
			if (GetComponent<AudioSource> ())
				GetComponent<AudioSource> ().Play ();

			for (int j = 0; j < bulletsPerShot; j++) {
				Instantiate (rocket, transform.position, transform.rotation);
				Debug.Log ("Rocket");
			}

			yield return new WaitForSeconds (shotDelay);
		}
	}
}