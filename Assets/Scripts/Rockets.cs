using UnityEngine;
using System.Collections;

//This script controls the playerFollowing rockets
public class Rockets : MonoBehaviour
{
    
	public GameObject currentTarget = null;
	public float rotationSpeed = 10f;
	public float range = 32f;
	public float speed = 1f;

	// Use this for initialization
	void Awake ()
	{
		float minimalEnemyDistance = float.MaxValue;
		GameObject[] playerAlive = null;
        
		GameObject[] escortPlane = GameObject.FindGameObjectsWithTag ("Escort");
        
		if (escortPlane.Length > 0)
			playerAlive = GameObject.FindGameObjectsWithTag ("Escort");
		else
			playerAlive = GameObject.FindGameObjectsWithTag ("Player");
        
		foreach (GameObject player in playerAlive) {
			float distance = Vector3.Distance (transform.position, player.transform.position);
            
			if (distance < minimalEnemyDistance && distance < range) {
				currentTarget = player;
				minimalEnemyDistance = distance;
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if (currentTarget != null) {
			Vector2 point2Target = (Vector2)transform.position - (Vector2)currentTarget.transform.position;

			float value = Vector3.Cross (point2Target, transform.right).z;

			GetComponent<Rigidbody2D> ().angularVelocity = 200f * value;

			GetComponent<Rigidbody2D> ().velocity = transform.right * speed;
		} else
			GetComponent<Rigidbody2D> ().AddForce (transform.up * -1f * speed);
	}
}
