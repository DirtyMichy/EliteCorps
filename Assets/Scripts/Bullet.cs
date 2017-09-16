using UnityEngine;

//This script will handle the bullet adding itself back to the pool
public class Bullet : MonoBehaviour
{
	public int speed = 10;			//How fast the bullet moves
	public float lifeTime = 10;		//How long the bullet lives in seconds
	public int power = 1;			//Power of the bullet
    public int owner = 0;           //0 = None, 1 = Player1, ...
    public int powerUpType = 0;     //0 = None, 1 = Ammo, 2 = Special, 3 = Health

    public GameObject explosionSmall;

    public Sprite[] PowerUpSprites;

    void OnEnable ()
	{
		//Send the bullet "forward"
        if (GetComponent<Rigidbody2D>())
		GetComponent<Rigidbody2D>().velocity = transform.up.normalized * speed;

		//Invoke the Die method
        if(tag=="PowerUp")
        {
            powerUpType = Random.Range(1, 4);

            GetComponent<SpriteRenderer>().sprite = PowerUpSprites[powerUpType-1];

            iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(8f, 8f, 8f), "easeType", "easeInOutExpo", "time", .5f));
        }
        else
		Invoke ("Die", lifeTime);
	}

    void Awake()
    {
        if (tag != "PowerUp")
            if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().Play();
    }

	void OnDisable()
	{
		//Stop the Die method (in case something else put this bullet back in the pool)
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