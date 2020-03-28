using UnityEngine;

public class Background : MonoBehaviour
{
	public float speed = 0.1f;
	private float y = 0.0f;
	void Update ()
	{
		y = Mathf.Repeat (Time.time * speed, 1);
		Vector2 offset = new Vector2 (0, y);
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", offset);
	}
}