using UnityEngine;
using System.Collections;

public class WaterAnimation : MonoBehaviour {

	//string name = "Water_00001";
	//string extraZero = "00";
	int index = 0;
	public float animationSpeed = 1f;
	//public Sprite[] water;
	public Texture[] water;
    public bool moveOffset = true;

	// Use this for initialization
	public void StartAnimation () {		
		/*
		for(int i =0; i < 10; i++){
			string path = "Assets/Resources/Water_00" + extraZero + i + ".png";

			water[i] = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));

			if(i == 9)
				extraZero = "0";
			if(i == 99)
				extraZero = "";
			if (i == 199) 
			{
				i = 0;
				extraZero = "00";
			}
			//Debug.Log("found: " + water.Length);

		}
*/
		//Debug.Log("found: " + water.Length);
		StartCoroutine ("Animate");
	}

	void Update(){
        if(moveOffset)
        {
		//Keep looping between 0 and 1
		float y = Mathf.Repeat (Time.time * animationSpeed, 1);
		//Create the offset
		Vector2 offset = new Vector2 (0, y);
		//Apply the offset to the material
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", offset);		
        }
        GetComponent<Renderer>().sharedMaterial.SetTexture ("_MainTex",water[index]);
	}

	IEnumerator Animate ()
	{


		if (index == water.Length-1) 
		{
			index = 0;
		}

		//GetComponent<SpriteRenderer>().sprite = water[index];

		yield return new WaitForSeconds(animationSpeed);

		index++;
		//Debug.Log("Index: " + index);
		StartCoroutine ("Animate");
	}
}
