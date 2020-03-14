using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
	public int directionAndSpeed = 2;
	private Vector3 rotator;
	// Update is called once per frame
	void Update ()
	{
		rotator = transform.eulerAngles;
		rotator.z += Time.deltaTime * directionAndSpeed;
		transform.eulerAngles = rotator;
	}    
}