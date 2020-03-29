using UnityEngine;
using System.Collections;

//This script controls the enemies and objectives
public class Weapon : MonoBehaviour
{
    public bool firingStarted = false;  //to ensure a coroutine isnt started twice
    public bool canShoot = true;
    public float firingSpeed = 1;
    public float timeBetweenBulletShots = 0;
    public float randomizedFiringDelay = 0;
    public int bulletsPerShot = 1;
    public Transform[] Guns;
    public GameObject bullet;

    //Start shooting
    public void StartShooting()
    {
        if (canShoot && !firingStarted)
        {
            firingStarted = true;
            StartCoroutine("Shoot");
        }
    }

    //Stop shooting
    public void StopShooting()
    {
        firingStarted = false;
        StopCoroutine("Shoot");
    }

    //Setting up hp and behaviour
    void Awake()
    {
        Guns = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            Guns[i] = transform.GetChild(i);

        if (canShoot)
        {
            StartShooting();
        }
    }

    IEnumerator Shoot()
    {
        //stop shooting if units gets near the south viewport, so they don't shoot backwards
        while (firingStarted)
        {
            for (int i = 0; i < Guns.Length; i++)
            {
                for (int j = 0; j < bulletsPerShot; j++)
                {
                    if (GetComponent<AcquireTarget>())
                    {
                        //Units with target acquiring should only shoot when a target is acquired
                        if (GetComponent<AcquireTarget>().currentTarget != null && GetComponent<AcquireTarget>().targetAcquired)
                        {
                            Instantiate(bullet, Guns[i].transform.position, Guns[i].transform.rotation);

                            if (GetComponent<AudioSource>())
                                GetComponent<AudioSource>().Play();
                        }
                    }
                    else
                    {
                        //Spawns Rockets or a straight forward shot, rockets have their own AcquireTarget.cs
                        Instantiate(bullet, Guns[i].transform.position, Guns[i].transform.rotation);

                        if (GetComponent<AudioSource>())
                            GetComponent<AudioSource>().Play();
                    }
                }
                yield return new WaitForSeconds(timeBetweenBulletShots);
            }
            yield return new WaitForSeconds(firingSpeed + Random.Range(0, randomizedFiringDelay));
        }

        firingStarted = false;
        canShoot = false;
        StopCoroutine("Shoot");
    }
}