using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int speed = 10;
    public int powerUpType = 0;     //0 = None, 1 = Ammo, 2 = Special, 3 = Health
    public int bonusAmmo = 0, bonusSpecial = 0, bonusHealth = 0;

    public Sprite[] PowerUpSprites;

    void OnEnable()
    {
        powerUpType = Random.Range(1, 4);

        switch (powerUpType)
        {
            case 0:
                Debug.Log("ERROR: POWERUP NOT INITIALIZED");
                break;
            case 1:
                bonusAmmo = 128;
                break;
            case 2:
                bonusSpecial = 2;
                break;
            case 3:
                bonusHealth = 8;
                break;
            default:
                break;
        }       

        GetComponent<Rigidbody2D>().velocity = transform.up.normalized * speed;
        GetComponent<SpriteRenderer>().sprite = PowerUpSprites[powerUpType - 1];
        GetComponent<AudioSource>().Play();

        iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(8f, 8f, 8f), "easeType", "easeInOutExpo", "time", .5f));
    }
}