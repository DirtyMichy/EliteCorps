using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int speed = 10;          
    public float lifeTime = 10;
    public int powerUpType = 0;     //0 = None, 1 = Ammo, 2 = Special, 3 = Health
    
    public Sprite[] PowerUpSprites;

    void OnEnable()
    {
        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().velocity = transform.up.normalized * speed;

            powerUpType = Random.Range(1, 4);

            GetComponent<SpriteRenderer>().sprite = PowerUpSprites[powerUpType - 1];

            iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(8f, 8f, 8f), "easeType", "easeInOutExpo", "time", .5f));

        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().Play();
    }
}