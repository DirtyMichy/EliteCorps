using UnityEngine;

public class Explosion : MonoBehaviour
{
	public float lifeTime = 1f;
    
	void OnEnable ()
	{	
		Invoke ("Die", lifeTime);
    }
	
	void Die()
	{
		Destroy(gameObject);
	}
}