using UnityEngine;
using System.Collections;

//This script controls the enemies and objectives
public class Weapon : MonoBehaviour
{
    public bool hasTargeting = false;   //does the unit has targeting?
    public bool firingStarted = false;  //to ensure a coroutine isnt started twice
    public bool canShoot = true;
    public float firingSpeed = 1;
    public float randomizedFiringDelay = 0;
    public int bulletsPerShot = 1;
    public Transform[] shotPositions;
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
        shotPositions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            shotPositions[i] = transform.GetChild(i);

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
            for (int i = 0; i < shotPositions.Length; i++)
            {
                for (int j = 0; j < bulletsPerShot; j++)
                {
                    if (GetComponent<AcquireTarget>())
                    {
                        //Units with targetacquiring have cannons with GunOutPuts
                        if (GetComponent<AcquireTarget>().currentTarget != null && GetComponent<AcquireTarget>().targetAcquired)
                        {
                            //targeting Fire points have gunOutPuts where the bullet spawns - this isn't global defined because an enemy can have multiple targeting guns (Boss01)                            
                            for (int l = 0; l < transform.childCount; l++)
                            {
                                if (GetComponent<AudioSource>())
                                    GetComponent<AudioSource>().Play();

                                Instantiate(bullet, transform.GetChild(l).transform.position, transform.GetChild(l).transform.rotation);

                                yield return new WaitForSeconds(0.1f);
                            }
                        }
                    }
                    else
                    {
                        //Spawns Rockets or a straight forward shot, rockets have their own AcquireTarget.cs
                        Instantiate(bullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);

                        if (GetComponent<AudioSource>())
                            GetComponent<AudioSource>().Play();
                    }
                }
            }
            yield return new WaitForSeconds(firingSpeed + Random.Range(0, randomizedFiringDelay));
        }

        firingStarted = false;
        canShoot = false;
        StopCoroutine("Shoot");
    }
}