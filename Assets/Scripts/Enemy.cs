﻿using UnityEngine;
using System.Collections;

public class Enemy : UnitObject
{
    [Range(1, 100)]
    public int chanceToSpawnPowerUp = 100;
    public int maxHP = 1;
    public int currentHP;
    public int point = 100;
    public bool isObjective = false;
    public bool isGroundUnit = false;
    public bool isBoss = false;         //Bosses start the highscore by death
    public bool firingStarted = false;  //to ensure a coroutine isnt started twice
    public bool stopMovingInCenter = false;
    public RectTransform bossHealth;    //important for bossHealthBars
    public GameObject powerUp;          //if this isn't null, the enemy can drop a powerUp
    public GameObject healthContainer;  //incoming damage will also be applied to this object
    private bool isDying = false;       //some enemies have a dyingtimer, if shot they would award multiple times the pointvalue

    void Awake()
    {

        if (isBoss)
        {
            //the more players, the more hitPoints the boss will have
            GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
            if (playerAlive.Length > 0)
                maxHP = maxHP * playerAlive.Length;
            GameObject bossCanvas = GameObject.Find("BossHealth");
            bossHealth = (RectTransform)bossCanvas.transform;
            currentHP = maxHP;
            bossHealth.transform.localScale = new Vector3(100f / maxHP * currentHP / 100f, bossHealth.transform.localScale.y, bossHealth.transform.localScale.z);
        }
        else
            currentHP = maxHP;

        if (GetComponent<Rigidbody2D>())
            if (unitName == "Boss02")
                GetComponent<Rigidbody2D>().velocity = (transform.up) * speed;
            else
                GetComponent<Rigidbody2D>().velocity = (transform.up * -1) * speed;
    }

    void Update()
    {
        //Stop Boss02, put this maybe in an extra script
        if (transform.position.y > 8f && speed != 0f && unitName == "Boss02")
        {
            speed = 0f;
            GetComponent<Rigidbody2D>().velocity = (transform.up * -1) * speed;
            firingStarted = false;
        }

        //Stop Scene (Background, Props,...) from moving, put this in an extra script
        if (transform.position.y <= 0f && speed != 0f && unitName == "Boss01")
        {
            speed = 0f;
            GetComponent<Rigidbody2D>().velocity = (transform.up * -1) * speed;
        }

        if (stopMovingInCenter)
        {
            speed = 0f;
            GetComponent<Rigidbody2D>().velocity = (transform.up * -1) * speed;

            if (GetComponent<AcquireTarget>())
            {
                Vector3 direction = transform.position - GetComponent<AcquireTarget>().currentTarget.transform.position;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI + 90, new Vector3(0, 0, 1)), Time.deltaTime * 50f);
            }
        }
        
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        string layerName = LayerMask.LayerToName(c.gameObject.layer);
        if (layerName != "Bullet (Player)")
            return;

        Bullet obj = c.GetComponent<Bullet>();

        //healthbars shouldn't get lower than 0
        if (currentHP > 0)
            currentHP -= obj.damage;

        if (healthContainer != null)
            if(healthContainer.GetComponent<Enemy>().currentHP > 0)
                healthContainer.GetComponent<Enemy>().currentHP -= obj.damage;

        //Setting the healthbar for the boss
        if (isBoss)
        {
            bossHealth.transform.localScale = new Vector3(100f / maxHP * currentHP / 100f, bossHealth.transform.localScale.y, bossHealth.transform.localScale.z);
        }

        if (currentHP <= 0 && !isDying)
        {
            isDying = true;
            currentHP = 0;
            Manager.current.AddPoint(point, obj.owner, isObjective);

            int rng = Random.Range(0, 100 / chanceToSpawnPowerUp);
            Debug.Log(rng);
            if (powerUp && rng == 0 && powerUp != null)
                Instantiate(powerUp, transform.position, transform.rotation);

            Explode();

            if (isBoss)
            {
                StartCoroutine("BossExplosion");

                Manager.current.objectiveComplete = true;
                Manager.current.ShowHighScore();
            }
        }
        else
        {
            //play the damaged animation, debris is being ignored
            if (unitName != "DrillingPitDestroyed" && unitName != "BunkerDebris")
            {
                GetComponent<Animator>().SetTrigger("Damage");

                foreach (Transform child in transform)
                {
                    transform.gameObject.GetComponent<Animator>().SetTrigger("Damage");
                }
            }
        }

        //Buildings stay as debris
        if (unitName != "DrillingPitDestroyed" && unitName != "BunkerDebris")
            obj.Die();
    }

    IEnumerator BossExplosion()
    {
        for (int i = 0; i < 10; i++)
        {
            GetComponent<Animator>().SetTrigger("Damage");

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