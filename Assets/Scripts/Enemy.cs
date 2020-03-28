using UnityEngine;
using System.Collections;

//This script controls the enemies and objectives
public class Enemy : UnitObject
{
    public int maxHP = 1;
    public int currentHP;
    public int point = 100;
    public int damageModifier = 0;      //if the bossturrets get destroyed, it takes extra dmg
    public bool isObjective = false;
    public bool isGroundUnit = false;
    public bool isBoss = false;         //Bosses start the highscore by death
    public bool isTurret = false;       //Turrets increase the damageModifier
    public bool hasTargeting = false;   //does the unit has targeting?
    public bool targetAcquired = false; //units with targeting acquire targets
    public bool firingStarted = false;  //to ensure a coroutine isnt started twice
    public RectTransform bossHealth;    //important for bossHealthBars
    public GameObject powerUp;          //if this isn't null, the enemy can drop a powerUp
    private bool isDying = false;         //some enemies have a dyingtimer, if shot they would award multiple times the pointvalue

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
        //Setting hp 
        if (isBoss)
        {
            GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
            if (playerAlive.Length > 0)
                maxHP = maxHP * playerAlive.Length;

            currentHP = maxHP;
            bossHealth.transform.localScale = new Vector3(100f / maxHP * currentHP / 100f, bossHealth.transform.localScale.y, bossHealth.transform.localScale.z);
        }
        else if (unitName == "Cannon")
        {
            GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");
            currentHP = maxHP * playerAlive.Length;         //cannons aren't bosses so we have to set their hp here
        }
        else
            currentHP = maxHP;

        //Special behaviour for Bosss02
        if (unitName == "Boss02")
            if (GetComponent<Rigidbody2D>())
                GetComponent<Rigidbody2D>().velocity = (transform.up) * speed;
            else if (GetComponent<Rigidbody2D>())
                GetComponent<Rigidbody2D>().velocity = (transform.up * -1) * speed;

        //for straight forward shooting, targetAcquired has to be always true
        if (!hasTargeting)
            targetAcquired = true;

        //start shooting
        if (canShoot && targetAcquired)
        {
            StartShooting();
        }
    }

    void Update()
    {
        //stop shooting if units gets near the south viewport, so they don't shoot backwards
        if (transform.position.y <= -5f && firingStarted && !isBoss)
        {
            firingStarted = false;
            canShoot = false;
            StopCoroutine("Shoot");
        }

        //Stop Boss02, put this in an extra script
        if (transform.position.y > 8f && speed != 0f && unitName == "Boss02")
        {
            speed = 0f;
            GetComponent<Rigidbody2D>().velocity = (transform.up * -1) * speed;
            firingStarted = false;
            canShoot = true;
            StartShooting();
        }

        //Stop Scene (Background, Props,...) from moving, put this in an extra script
        if (transform.position.y <= 0f && speed != 0f && unitName == "Boss01")
        {
            speed = 0f;
            GetComponent<Rigidbody2D>().velocity = (transform.up * -1) * speed;
        }
    }
    //Is called when a bossTurret gets destroyed
    public void IncreaseDamageModifier()
    {
        damageModifier++;
    }
    //when hitting an isle the boat has to get pushed away
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Pushing Boats
        if (collision.GetComponent<Enemy>())
        {
            if (isGroundUnit && collision.GetComponent<Enemy>().isGroundUnit && collision.GetComponent<Enemy>().unitName == "AttackBoat" || collision.GetComponent<Enemy>().unitName == "FlakShip")
            {
                GetComponent<Rigidbody2D>().AddForce(transform.right);
            }
        }
        else if (collision.GetComponent<PlayerMulti>())
            if (collision.GetComponent<PlayerMulti>().unitName == "EscortPlane")
                GetComponent<Rigidbody2D>().AddForce(transform.right);
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        string layerName = LayerMask.LayerToName(c.gameObject.layer);
        if (layerName != "Bullet (Player)")
            return;

        Bullet obj = c.GetComponent<Bullet>();

        if (currentHP > 0 && damageModifier <= 1) //healthbars shouldn't get lower than 0
            currentHP -= obj.damage;
        else
        {
            if (currentHP > 0)
            {
                if (unitName == "Boss01" && damageModifier == 3)
                    currentHP -= obj.damage * 2;
                else if (unitName == "Boss02" && damageModifier == 3)
                    currentHP -= obj.damage * 2;
                else
                    currentHP -= obj.damage;
            }
        }
        //Setting the healthbar for the boss
        if (isBoss)
        {
            bossHealth.transform.localScale = new Vector3(100f / maxHP * currentHP / 100f, bossHealth.transform.localScale.y, bossHealth.transform.localScale.z);
        }

        //If the ship is out of hit points...
        if (currentHP <= 0 && !isDying)
        {
            isDying = true;

            currentHP = 0;

            if (isTurret && GetComponentInParent<Enemy>())
            {
                transform.root.GetComponent<Enemy>().IncreaseDamageModifier();
            }

            Manager.current.AddPoint(point, obj.owner, isObjective);

            //spawn powerUp
            int rng = Random.Range(0, 10);

            if (powerUp && rng == 1)
                Instantiate(powerUp, transform.position, transform.rotation);

            Explode();

            if (isBoss)
            {
                Manager.current.objectiveComplete = true;
                Manager.current.ShowHighScore();
            }
        }
        else
        {
            //play the damaged animation, debris is being ignored
            if (unitName != "DrillingPitDestroyed" && unitName != "BunkerDebris")
                animator.SetTrigger("Damage");
        }

        //Buildings stay as debris
        if (unitName != "DrillingPitDestroyed" && unitName != "BunkerDebris")
            obj.Die();
    }
}