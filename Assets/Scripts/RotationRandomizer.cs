using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRandomizer : MonoBehaviour
{
	void Awake()
	{
		Vector3 rotator = transform.eulerAngles;
		rotator.z = Random.Range(0, 360);
		transform.eulerAngles = rotator;
	}
}
