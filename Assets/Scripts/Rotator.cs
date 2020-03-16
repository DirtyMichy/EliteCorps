using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
	public int speed = 2;
	public int direction = 1;
	public float minAngle = 0, maxAngle = 359;
	public bool pingPong = false;
	private Vector3 rotator;

	void Update ()
	{
		rotator = transform.eulerAngles;
		rotator.z += Time.deltaTime * speed * direction;
		transform.eulerAngles = rotator;

		if (transform.eulerAngles.z > maxAngle)
			if (pingPong)
				direction *= -1;
			else
				rotator.z = minAngle;
	}    
}