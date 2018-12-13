using UnityEngine;
using System.Collections;

public class Isle : MonoBehaviour
{
    
	public float speed;
	//speed
	public Sprite isle01, isle02, isle03, isle04;
    
	// Use this for initialization
	void Awake ()
	{
		GetComponentInParent<Rigidbody2D> ().velocity = (transform.up * -1) * speed;
        
		int r = Random.Range (0, 4);
		switch (r) {
		case 0:
			GetComponentInChildren<SpriteRenderer> ().sprite = isle01;
			break;
		case 1:
			GetComponentInChildren<SpriteRenderer> ().sprite = isle02;
			break;
		case 2:
			GetComponentInChildren<SpriteRenderer> ().sprite = isle03;
			break;
		case 3:
			GetComponentInChildren<SpriteRenderer> ().sprite = isle04;
			break;
		}
        
		Vector3 rot = transform.eulerAngles;
		rot.z = Random.Range (0f, 360f);
		transform.eulerAngles = rot;

	}
}
