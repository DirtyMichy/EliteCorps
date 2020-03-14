using UnityEngine;

public class Bullet : MonoBehaviour
{
	public int speed = 10;			//How fast the bullet moves
	public float lifeTime = 10;		//How long the bullet lives in seconds
	public int damage = 1;			//Power of the bullet
    public int owner = 0;           //0 = None, 1 = Player1, ...

    public GameObject explosionSmall;

    void OnEnable ()
	{
        if (GetComponent<Rigidbody2D>())
		GetComponent<Rigidbody2D>().velocity = transform.up.normalized * speed;

		Invoke ("Die", lifeTime);
	}
	
	void OnDisable()
	{
		CancelInvoke ("Die");
	}

    public void SetOwner(int i)
    {
        owner = i;
    }

    public void Die()
    {
            if (explosionSmall != null)
                Instantiate(explosionSmall, transform.position, transform.rotation);

        Destroy(gameObject);
	}
}