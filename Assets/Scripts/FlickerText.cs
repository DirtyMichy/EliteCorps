using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlickerText : MonoBehaviour
{

	public int seconds = 0;

	// Update is called once per frame
	void Start ()
	{
		StartCoroutine ("timer");
	}

	IEnumerator timer ()
	{
		while (true) {
			if (seconds % 2 == 0) {
				GetComponent<Text> ().enabled = false;
			} else {
				GetComponent<Text> ().enabled = true;
			}

			yield return new WaitForSeconds (1f);
			seconds++;
		}
	}
}
