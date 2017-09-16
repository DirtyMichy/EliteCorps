using UnityEngine;
using System.Collections;

public class BoatWaves : MonoBehaviour 
{
    protected Transform[] wavePositions;
    public GameObject wave;

    void Awake()
    {
        wavePositions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)        
            wavePositions [i] = transform.GetChild(i);
        StartCoroutine("WaveSpawn");
    }
    
    IEnumerator WaveSpawn()
    {
        while(true)
        {   
            //Watching performance, to many gameobjects seem to crash the engine
            GameObject[] allGameobjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            //Debug.Log(allGameobjects.Length);

            for (int i = 0; i < wavePositions.Length; i++)
            {                
                if (allGameobjects.Length < 666)
                { 
                    GameObject lastWave = (GameObject)Instantiate(wave, wavePositions[i].transform.position, wavePositions[i].transform.rotation);
                    lastWave.GetComponent<Cloud>().direction = -1 + i;
                    lastWave.GetComponent<Cloud>().SetDirection();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}