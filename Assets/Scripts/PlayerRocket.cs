using UnityEngine;
using System.Collections;

public class PlayerRocket : MonoBehaviour
{
    
	public GameObject currentTarget = null;
	public float rotationSpeed = 10f;
	public float range = 32f;
	public float speed = 1f;

	// Use this for initialization
	void Awake ()
	{
		float minimalEnemyDistance = float.MaxValue;

		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
                
		foreach (GameObject enemy in enemies) {
			float distance = Vector3.Distance (transform.position, enemy.transform.position);
            
			if (distance < minimalEnemyDistance && distance < range) {
				currentTarget = enemy;
				minimalEnemyDistance = distance;
			}
		}
	}
    
	// Update is called once per frame
	void Update ()
	{
		float minimalEnemyDistance = float.MaxValue;
        
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
        
		foreach (GameObject enemy in enemies) {
			if (enemy.transform.position.y > transform.position.y) { //Target only upper enemies
				float distance = Vector3.Distance (transform.position, enemy.transform.position);

				if (distance < minimalEnemyDistance && distance < range) {
					currentTarget = enemy;
					minimalEnemyDistance = distance;
				}
			}

		}

		if (currentTarget != null) {
			Vector2 point2Target = (Vector2)transform.position - (Vector2)currentTarget.transform.position;

			float value = Vector3.Cross (point2Target, transform.right).z;

			GetComponent<Rigidbody2D> ().angularVelocity = 200f * value;

			GetComponent<Rigidbody2D> ().velocity = transform.right * speed;
		} else
			GetComponent<Rigidbody2D> ().AddForce (transform.up);
	}
}
