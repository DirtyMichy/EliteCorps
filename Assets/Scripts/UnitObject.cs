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
    public Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }    

    protected void Explode()
    {
        if (unitName == "Boss01")
        {
            StartCoroutine("BossExplosion");
        }
        else
        {
            if (unitName == "DrillingRig" || unitName == "FlakBunker")
            {
                GameObject spawnedDebris = (GameObject)Instantiate(debris, transform.position, transform.rotation);
            }
            else
                Instantiate(explosion, transform.position, transform.rotation);

            StopCoroutine("Shoot");

            if (unitName == "Cannon")
                debris.SetActive(true);//BossE1 uses smokeparticles for destroyed cannons
            Destroy(gameObject);
        }
    }

    IEnumerator BossExplosion()
    {
        for (int i = 0; i < 10; i++)
        {
            animator.SetTrigger("Damage");

            for (int j = 0; j < 2; j++)
            {
                Vector3 pos = transform.position;
                pos.x += Random.Range(-5, 5);
                pos.y += Random.Range(-0.5f, 0.5f);

                Quaternion rot = transform.rotation;
                rot.z = Random.Range(0f, 360f);

                Instantiate(explosion, pos, rot);
            }
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }
    
}