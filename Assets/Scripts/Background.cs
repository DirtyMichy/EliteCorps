using UnityEngine;

//This script controls the scrolling of the background
public class Background : MonoBehaviour
{
	public float speed = 0.1f;          //Speed of the scrolling
	private float y = 0.0f;
	void Update ()
	{
		//Keep looping between 0 and 1
		y = Mathf.Repeat (Time.time * speed, 1);
		//Create the offset
		Vector2 offset = new Vector2 (0, y);
		//Apply the offset to the material
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", offset);
	}
}