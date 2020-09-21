using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
	public float speed = 2f;
	private float scale = 1f;
	private Vector3 originalScale;

	private void Start()
	{
		originalScale = transform.localScale;
	}

	private void Update()
	{
		scale = Mathf.Lerp(1, 2, Mathf.PingPong(Time.time * speed, 1));
		transform.localScale = originalScale * scale;
	}
}