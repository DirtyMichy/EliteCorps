using UnityEngine;
using System.Collections;

//This script controls some rotating props, like cranes from the drillingPit
public class Rotation180 : MonoBehaviour
{
	public int degree = 90;
	//rotate from 0-180
	public float direction = 0f;
	//-1/1 for left/right
	public float speed = 1f;
	//should be 0.25 or 0.5 or 1.0
    
	void Awake ()
	{
		if (Random.value > 0.5f)
			direction = 1f;
		else
			direction = -1f;
	}

	// Update is called once per frame
	void Update ()
	{         
		degree += (int)(direction);

		if (degree == 180 || degree == 0) //change direction
            direction *= -1f;

		Vector3 rot = transform.eulerAngles;
		rot.z += direction * speed * Time.deltaTime;
		transform.eulerAngles = rot;        
	}
    
}
