using UnityEngine;
using System.Collections;
using GamepadInput;
public class UnitObject : MonoBehaviour
{
    public bool canShoot;
    public bool isInvincible = false;
    public float speed;
    public float shieldSeconds = 0;
    public string unitName = "Default";
    public GameObject explosion;
    public GameObject debris;

    protected void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        if (debris != null)
        {
            GameObject spawnedDebris = (GameObject)Instantiate(debris, transform.position, transform.rotation);
            spawnedDebris.transform.parent = transform.parent;
        }

        Destroy(gameObject);
    }
}