using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public int speed = 10;
    private int powerUpType = 0;     //0 = None, 1 = Ammo, 2 = Special, 3 = Health
    public int bonusAmmo = 0, bonusSpecial = 0, bonusHealth = 0;        
    public Sprite[] PowerUpSprites;

    void OnEnable()
    {
        powerUpType = Random.Range(1, 4);

        //we add all powerup values to the player getting the powerup, so the wrong ones need to be 0
        switch (powerUpType)
        {
            case 0:
                Debug.Log("ERROR: POWERUP NOT INITIALIZED");
                break;
            case 1:
                {
                    //bonusAmmo = 0;
                    bonusSpecial = 0;
                    bonusHealth = 0;
                    break;
                }
            case 2:
                {
                    bonusAmmo = 0;
                    //bonusSpecial = 0;
                    bonusHealth = 0;
                    break;
                }
            case 3:
                {
                    bonusAmmo = 0;
                    bonusSpecial = 0;
                    //bonusHealth = 0;
                    break;
                }
            default:
                break;
        }       

        GetComponent<Rigidbody2D>().velocity = transform.up.normalized * speed;
        GetComponent<SpriteRenderer>().sprite = PowerUpSprites[powerUpType - 1];
        GetComponent<AudioSource>().Play();

        iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(8f, 8f, 8f), "easeType", "easeInOutExpo", "time", .5f));
    }
}