using UnityEngine;
using System.Collections;

public class BackGroundManager : MonoBehaviour
{

	public GameObject[] episodeBackGrounds;

	// Use this for initialization
	public void SetBackground (int episode)
	{
		for (int i = 0; i < episodeBackGrounds.Length; i++)
		{
			episodeBackGrounds[i].SetActive(false);

		}
        
		if (episode == 1) {
			episodeBackGrounds [0].SetActive (true);
		}
		if (episode == 2)
			episodeBackGrounds [1].SetActive (true);
	}
}
