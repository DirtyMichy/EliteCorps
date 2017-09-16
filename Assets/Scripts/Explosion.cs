using UnityEngine;

//This script controls the explosion prefab
public class Explosion : MonoBehaviour
{
	public float lifeTime = 1f;		//Lifetime of the explosion in seconds
    
	void OnEnable ()
	{	
		//Invoke the Die method
		Invoke ("Die", lifeTime);

        if (tag == "PowerUp")
            iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(0f, 0f, 0f), "easeType", "easeInOutExpo", "time", .5f));
    }
	
	void Die()
	{
		Destroy(gameObject);
	}
}