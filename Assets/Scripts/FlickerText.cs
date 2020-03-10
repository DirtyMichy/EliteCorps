using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlickerText : MonoBehaviour
{
	public float speed = 2f;
	private float alpha = 1f;

	private void Update()
	{
		alpha = Mathf.Lerp(0, 1, Mathf.PingPong(Time.time * speed, 1));
		Color color = GetComponent<Text>().color;
		color.a = alpha;
		GetComponent<Text>().color = color;
	}
}
