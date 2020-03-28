using UnityEngine;
using System.Collections;

//This script controls the enemies and objectives
public class Weapon : MonoBehaviour
{
    public bool hasTargeting = false;   //does the unit has targeting?
    public bool targetAcquired = false; //units with targeting acquire targets
    public bool firingStarted = false;  //to ensure a coroutine isnt started twice
    public bool canShoot;
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
    void OnEnable()
    {
        shotPositions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            shotPositions[i] = transform.GetChild(i);

        //for straight forward shooting, targetAcquired has to be always true
        if (!hasTargeting)
            targetAcquired = true;

        if (canShoot && targetAcquired)
        {
            StartShooting();
        }
    }

    IEnumerator Shoot()
    {
        //stop shooting if units gets near the south viewport, so they don't shoot backwards
        while (transform.position.y <= -5f && firingStarted)
        {
            for (int i = 0; i < shotPositions.Length; i++)
            {
                for (int j = 0; j < bulletsPerShot; j++)
                {
                    if (shotPositions[i] != null)
                    {
                        if (shotPositions[i].tag == "Cannon" && shotPositions[i].GetComponent<AcquireTarget>())
                        {
                            //Units with targetacquiring have cannons with GunOutPuts
                            if (shotPositions[i].GetComponent<AcquireTarget>().currentTarget != null)
                            {
                                //searching for the gunOutPuts - targeting Fire points have gunOutPuts where the bullet spawns - this isn't global defined because an enemy can have multiple targeting guns (Boss01)
                                Transform[] gunOutPuts = new Transform[shotPositions[i].transform.childCount];

                                for (int l = 0; l < gunOutPuts.Length; l++)
                                {
                                    gunOutPuts[l] = shotPositions[i].transform.GetChild(l);

                                    if (GetComponent<AudioSource>())
                                        GetComponent<AudioSource>().Play();

                                    Instantiate(bullet, gunOutPuts[l].transform.position, gunOutPuts[l].transform.rotation);

                                    yield return new WaitForSeconds(0.1f);
                                }
                            }
                        }
                        else
                        {
                            //Spawns Rockets or a straight forward shot
                            Instantiate(bullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);

                            if (GetComponent<AudioSource>())
                                GetComponent<AudioSource>().Play();
                        }
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