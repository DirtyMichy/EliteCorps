using UnityEngine;
using System.Collections;

//This script controls the waves of enemies
public class Emitter : MonoBehaviour
{
	//public GameObject[] waves;		//Prefabs of the waves of enemies
	public GameObject enemy;
	public GameObject Cloud;

	//int currentWave;				//Which wave we are currently on
	//WaveScript[] waveScripts;		//A collection of WaveScripts on the Wave prefabs (this is done for efficiency)

	Vector2 min;
	Vector2 max;

	void Awake()
	{
		/*
		//Create our array
		waveScripts = new WaveScript[waves.Length];
		//Iterate through the wave prefabs
		for (int i = 0; i < waves.Length; i++) 
		{
			//Instantiate them
			waves[i] = (GameObject)Instantiate (waves [i], transform.position, Quaternion.identity);
			//Set their parent
			waves[i].transform.parent = transform;
			//Deactivate them
			waves[i].SetActive(false);
			//Save their WaveScript for future use
			waveScripts[i] = waves[i].GetComponent<WaveScript>();
		}
		*/
		


	}

	//This is set up as a coroutine
	IEnumerator Start ()
	{
		//If we have no wave prefabs then exit
		//if (waves.Length == 0)
		//	yield break;

		//Loop indefinitely
		while (true ) 
		{


			if(Manager.current.menuActive == false)
			{
				min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
				max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

			Instantiate (enemy, transform.position = new Vector2 (Random.Range (min.x, max.x), 6f ), enemy.transform.rotation);
				Instantiate (Cloud, transform.position = new Vector2 (Random.Range (min.x, max.x), 6f ), enemy.transform.rotation);
			}

			yield return new WaitForSeconds (Random.Range (1, 3));
			
			/*
			//If the player is currently not playing then wait
			while(!Manager.current.IsPlaying()) {
				yield return new WaitForEndOfFrame ();
			}
			//Activate our current wave
			waves[currentWave].SetActive(true);
			//While the wave has active ships wait
			while (waveScripts[currentWave].ShipsStillAlive()) {
				yield return new WaitForEndOfFrame ();
			}
			//All enemy ships are inactive so deactivate the wave
			waves[currentWave].SetActive(false);
			//Advance or wrap around the current wave
			if (waves.Length <= ++currentWave)
				currentWave = 0;
			*/

		}
	}
}