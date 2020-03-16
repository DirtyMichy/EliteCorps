using UnityEngine;

//This script controls the explosion prefab
public class Explosion : MonoBehaviour
{
	public float lifeTime = 1f;		//Lifetime of the explosion in seconds
    
	void OnEnable ()
	{	
		//Invoke the Die method
		Invoke ("Die", lifeTime);
    }
	
	void Die()
	{
		Destroy(gameObject);
	}
}