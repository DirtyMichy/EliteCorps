using UnityEngine;
using System.Collections;

public class WaterAnimation : MonoBehaviour
{

	int index = 0;
	public float animationSpeed = 1f;
	public Texture[] water;
	public bool moveOffset = true;

	// Use this for initialization
	public void StartAnimation ()
	{		
		
		StartCoroutine ("Animate");
	}

	void Update ()
	{
		if (moveOffset) {
			//Keep looping between 0 and 1
			float y = Mathf.Repeat (Time.time * animationSpeed, 1);
			//Create the offset
			Vector2 offset = new Vector2 (0, y);
			//Apply the offset to the material
			GetComponent<Renderer> ().sharedMaterial.SetTextureOffset ("_MainTex", offset);		
		}
		GetComponent<Renderer> ().sharedMaterial.SetTexture ("_MainTex", water [index]);
	}

	IEnumerator Animate ()
	{


		if (index == water.Length - 1) {
			index = 0;
		}


		yield return new WaitForSeconds (animationSpeed);

		index++;
		//Debug.Log("Index: " + index);
		StartCoroutine ("Animate");
	}
}
