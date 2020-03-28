using UnityEngine;
using System.Collections;
using GamepadInput;
public class UnitObject : MonoBehaviour
{
    public float speed;
    public float shotDelay;
    public bool isInvincible = false;
    public GameObject bullet;
    public GameObject specialBullet;
    public GameObject rocket;
    public GameObject explosion;
    public GameObject debris;
    public bool canShoot;
    public string unitName = "Default";
    public int bulletsPerShot = 1;
    public GameObject shield;
    protected Transform[] shotPositions;
    protected Transform[] shotSpecialPositions;
    protected Animator animator;
    int localPlayer;
    float shieldSeconds = 0;

    void Awake()
    {
        shotPositions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            shotPositions[i] = transform.GetChild(i);

        animator = GetComponent<Animator>();
        if (unitName == "MinigunCopter" || unitName == "Friendly")
        {
            StartCoroutine("ChopperDance");
        }
    }

    IEnumerator ChopperDance()
    {
        while (true)
        {
            if (GetComponent<Rigidbody2D>())
            {
                GetComponent<Rigidbody2D>().velocity = (transform.right * -1) * speed;

                yield return new WaitForSeconds(1f);

                GetComponent<Rigidbody2D>().velocity = (transform.right) * speed;

                yield return new WaitForSeconds(1f);

                if (unitName == "MinigunCopter")
                    GetComponent<Rigidbody2D>().velocity = (transform.up * -1) * speed;
                else
                    GetComponent<Rigidbody2D>().velocity = transform.up.normalized * 0;

                yield return new WaitForSeconds(2f);
            }
        }
    }

    IEnumerator Shield()
    {
        isInvincible = true;

        float shieldAmount = 5f;

        shield.GetComponentInChildren<AudioSource>().mute = false;

        if (shieldSeconds > 0)
            shieldSeconds += shieldAmount;
        else
        {
            shieldSeconds += shieldAmount;

            while (shieldSeconds > 0)
            {
                shield.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 1f, 1f, shieldSeconds / 10f);

                yield return new WaitForSeconds(1);
                shieldSeconds--;

            }
        }
        if (shieldSeconds <= 0)
        {
            isInvincible = false;
            shield.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            shield.GetComponentInChildren<AudioSource>().mute = true;
        }
    }

    IEnumerator LineShot()
    {
        for (int j = 0; j < 32; j++)
        {
            for (int i = 0; i < shotPositions.Length; i++)
            {
                if (shotPositions[i].gameObject.tag == "Special")
                {
                    GameObject lastBullet = (GameObject)Instantiate(specialBullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                    lastBullet.SendMessage("SetOwner", localPlayer);
                }
            }
            if (GetComponent<AudioSource>())
                GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(shotDelay / 2);
        }
    }

    IEnumerator MultiShot()
    {
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < shotPositions.Length; i++)
            {
                if (shotPositions[i].gameObject.tag == "Special" || shotPositions[i].gameObject.tag == "Cannon")
                {
                    GameObject lastBullet = (GameObject)Instantiate(specialBullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                    lastBullet.SendMessage("SetOwner", localPlayer);
                }
            }

            if (GetComponent<AudioSource>())
                GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(shotDelay * 2);
        }
    }

    IEnumerator SprayShot()
    {
        int weaponPos = 0;

        //Find SpecialWeapon
        for (int i = 0; i < shotPositions.Length; i++)
        {
            if (shotPositions[i].gameObject.tag == "Special")
            {
                weaponPos = i;
            }
        }

        Vector3 r = shotPositions[weaponPos].eulerAngles;
        r.z = -45f;

        for (int j = 0; j < 30; j++)
        {
            shotPositions[weaponPos].eulerAngles = r;
            GameObject lastBullet = (GameObject)Instantiate(specialBullet, shotPositions[weaponPos].transform.position, shotPositions[weaponPos].transform.rotation);//new Quaternion(0f,0f,0f,0f));
            lastBullet.SendMessage("SetOwner", localPlayer);
            r.z += 3;

            if (GetComponent<AudioSource>())
                GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(shotDelay / 2);
        }
    }

    IEnumerator Laser()
    {
        for (int i = 0; i < shotPositions.Length; i++)
        {
            if (shotPositions[i].gameObject.tag == "Special")
            {
                if (shotPositions[i].GetComponent<AudioSource>())
                    shotPositions[i].GetComponent<AudioSource>().Play();
                for (int j = 0; j < 60; j++)
                {
                    GameObject lastBullet = (GameObject)Instantiate(specialBullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                    lastBullet.SendMessage("SetOwner", localPlayer);
                    yield return new WaitForSeconds(shotDelay / 10);
                }
            }
        }
    }

    IEnumerator Rockets()
    {
        for (int i = 0; i < shotPositions.Length; i++)
        {
            if (shotPositions[i].gameObject.tag == "Special")
            {
                GameObject lastBullet = (GameObject)Instantiate(specialBullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                lastBullet.SendMessage("SetOwner", localPlayer);
                yield return new WaitForSeconds(0.25f);

                GameObject secondLastBullet = (GameObject)Instantiate(specialBullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                secondLastBullet.SendMessage("SetOwner", localPlayer);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    public void ShootSpecialViaButton(int currentPlayer)
    {
        if (unitName == "Char01")
        {
            localPlayer = currentPlayer;
            StartCoroutine("MultiShot");
        }

        if (unitName == "Char02")
        {
            for (int i = 0; i < shotPositions.Length; i++)
            {
                if (shotPositions[i].gameObject.tag == "Special")
                {
                    GameObject lastBullet = (GameObject)Instantiate(specialBullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                    lastBullet.SendMessage("SetOwner", currentPlayer);
                }
            }
        }

        if (unitName == "Char03")
        {
            localPlayer = currentPlayer;
            StartCoroutine("Rockets");
        }

        if (unitName == "Char04")
        {
            StartCoroutine("Shield");
        }

        if (unitName == "Char05")
        {
            localPlayer = currentPlayer;
            StartCoroutine("SprayShot");
        }

        if (unitName == "Char06")
        {
            localPlayer = currentPlayer;
            StartCoroutine("Laser");
        }

        if (unitName == "Char07")
        {
            localPlayer = currentPlayer;
            StartCoroutine("LineShot");
        }
    }

    public void ShootViaButton(int currentPlayer)
    {
        if (GetComponent<AudioSource>())
            GetComponent<AudioSource>().Play();

        for (int i = 0; i < shotPositions.Length; i++)
        {
            if (shotPositions[i].gameObject.tag == "Cannon")
            {
                GameObject lastBullet = (GameObject)Instantiate(bullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                lastBullet.SendMessage("SetOwner", currentPlayer);
            }
        }

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
                Instantiate(debris, transform.position, transform.rotation);
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

    IEnumerator Shoot()
    {
        Vector2 min = new Vector2(0, 0);

        yield return new WaitForSeconds(Random.Range(1f, 3f));

        while (true && transform.position.y > min.y)
        {
            min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

            if (transform.position.y > min.y)
            {
                for (int i = 0; i < shotPositions.Length; i++)
                {
                    for (int j = 0; j < bulletsPerShot; j++)
                    {
                        if (shotPositions[i] != null)
                        {
                            if (shotPositions[i].tag == "Cannon" && shotPositions[i].GetComponent<AcquireTarget>())
                            {    //Units with targetacquiring have cannons with GunOutPuts
                                if (shotPositions[i].GetComponent<AcquireTarget>().currentTarget != null)
                                {
                                    //searching for the gunOutPuts - targeting Fire points have gunOutPuts where the bullet spawns - this isn't global defined because an enemy can have multiple targeting guns
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
                                if (shotPositions[i].tag == "Cannon")
                                {
                                    if (GetComponent<AudioSource>())
                                        GetComponent<AudioSource>().Play();

                                    Instantiate(bullet, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                                }

                                if (shotPositions[i].tag == "RocketSpawn")
                                {
                                    //rockets use extra delaytime
                                    Instantiate(rocket, shotPositions[i].transform.position, shotPositions[i].transform.rotation);
                                    yield return new WaitForSeconds(shotDelay);

                                }
                            }
                        }
                    }
                }

                if (unitName == "EasyPlane")
                    yield return new WaitForSeconds(Random.Range(shotDelay / 2, shotDelay));
                else
                    yield return new WaitForSeconds(shotDelay);
            }
        }
    }
}