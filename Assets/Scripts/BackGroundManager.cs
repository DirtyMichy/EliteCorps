using UnityEngine;
using System.Collections;

public class BackGroundManager : MonoBehaviour {

    public GameObject[] episodeBackGrounds;

	// Use this for initialization
	public void SetBackground (int episode) {
        episodeBackGrounds[0].SetActive(false);
        episodeBackGrounds[1].SetActive(false);

        
        if(episode == 1)
        {
            episodeBackGrounds[0].SetActive(true);
            episodeBackGrounds[0].GetComponentInChildren<WaterAnimation>().StartAnimation();
        }
        if(episode == 2)
            episodeBackGrounds[1].SetActive(true);
    }	
}
