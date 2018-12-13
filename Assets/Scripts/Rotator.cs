using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{

	public int direction = 2;
    
	// Update is called once per frame
	void Update ()
	{
		Vector3 r = transform.eulerAngles;
		r.z += Time.deltaTime * direction * 100f;
		transform.eulerAngles = r;
	}
    
}
