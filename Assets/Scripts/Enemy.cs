using UnityEngine;
using System.Collections;

//This script controls the enemies and objectives
public class Enemy : UnitObject
{
	public int hp = 1;
	//Ship's hit points
	public int point = 100;
	//Ship's point worth
	public int damageModifier = 0;
	//if the bossturrets get destroyed, it takes extra dmg
	public int currentHP;
	//Ship's current hit points

	public bool isObjective = false;
	//If its not a regular enemy
	public bool isGroundUnit = false;
	//Ships or buildings are groundUnits
	public bool isBoss = false;
	//Bosses start the highscore by death
	public bool isTurret = false;
	//Turrets increase the damageModifier

	public bool hasTargeting = false;
	//does the unit has targeting?
	public bool targetAcquired = false;
	//units with targeting acquire targets
	public bool firingStarted = false;
	//to ensure a coroutine isnt started twice
	bool dying = false;
	//some enemies have a dyingtimer, if shot they would award multiple times the pointvalue

	public RectTransform bossHealth;
	//importan for bossHealthBars
	public GameObject powerUp;
	//if this isn't empty, the enemy can drop a powerUp

	//Start shooting
	public void StartShooting ()
	{
		if (canShoot && !firingStarted) {
			firingStarted = true;
			StartCoroutine ("Shoot");
		}
	}

	//Stop shooting
	public void StopShooting ()
	{
		firingStarted = false;
		StopCoroutine ("Shoot");
	}

	//Setting up hp and behaviour
	void OnEnable ()
	{
		//Setting hp 
		if (isBoss) {
			GameObject[] playerAlive = GameObject.FindGameObjectsWithTag ("Player");
			if (playerAlive.Length > 0)
				hp = hp * playerAlive.Length;

			bossHealth = Manager.current.bossHealth;
			currentHP = hp;
			bossHealth.transform.localScale = new Vector3 (100f / hp * currentHP / 100f, bossHealth.transform.localScale.y, bossHealth.transform.localScale.z);
		} else if (unitName == "Cannon") { //Setting hp of bossCannon
			//Cannons are no bosses so we have to set their hp here
			GameObject[] playerAlive = GameObject.FindGameObjectsWithTag ("Player");
			currentHP = hp * playerAlive.Length;
		} else        //Setting hp
            currentHP = hp;

		//Special behaviour for Bosss02
		if (unitName == "Boss02")
		if (GetComponent<Rigidbody2D> ())
			GetComponent<Rigidbody2D> ().velocity = (transform.up) * speed;
		else if (GetComponent<Rigidbody2D> ())
			GetComponent<Rigidbody2D> ().velocity = (transform.up * -1) * speed;

		if (!hasTargeting)
			targetAcquired = true;

		//start shooting
		if (canShoot && targetAcquired) {
			StartShooting ();
		}
	}

	void Update ()
	{
		//stop shooting if units gets near the viewport (shooting enemies from behind was annoying)
		if (transform.position.y <= -5f && firingStarted && !isBoss) {
			firingStarted = false;
			canShoot = false;
			StopCoroutine ("Shoot");
		}

		//Stop Boss02
		if (transform.position.y > 8f && speed != 0f && unitName == "Boss02") {
			speed = 0f;
			GetComponent<Rigidbody2D> ().velocity = (transform.up * -1) * speed;
			firingStarted = false;
			canShoot = true;
			StartShooting ();
		}

		//Stop Scene (Background, Props,...) from moving
		if (transform.position.y <= 0f && speed != 0f && unitName == "Boss01") {
			speed = 0f;
			GetComponent<Rigidbody2D> ().velocity = (transform.up * -1) * speed;

			//when the shipBoss spawnst, all isles have to stop moving
			GameObject[] props = GameObject.FindGameObjectsWithTag ("Island");
			for (int i = 0; i < props.Length; i++) {
				if (props [i].GetComponent<Props> ()) {
					props [i].GetComponent<Props> ().speed = 0f;
					props [i].GetComponent<Rigidbody2D> ().velocity = (transform.up * -1) * 0f;
				}
				if (props [i].GetComponentInChildren<Props> ()) {
					props [i].GetComponentInChildren<Props> ().speed = 0f;
					if (props [i].GetComponentInChildren<Rigidbody2D> ())
						props [i].GetComponentInChildren<Rigidbody2D> ().velocity = (transform.up * -1) * 0f;
				}
			}
		}
	}
	//Start moving
	void Start ()
	{
		if (GetComponent<Rigidbody2D> ())
			GetComponent<Rigidbody2D> ().velocity = (transform.up * -1) * speed;
	}
	//Is called when a bossTurret gets destroyed
	public void IncreaseDamageModifier ()
	{
		damageModifier++;
	}
	//when hitting an isle the boat has to get pushed away
	private void OnTriggerStay2D (Collider2D collision)
	{
		//Pushing Boats
		if (collision.GetComponent<Enemy> ()) {
			if (isGroundUnit && collision.GetComponent<Enemy> ().isGroundUnit && collision.GetComponent<Enemy> ().unitName == "AttackBoat" || collision.GetComponent<Enemy> ().unitName == "FlakShip") {
				GetComponent<Rigidbody2D> ().AddForce (transform.right);
			}
		} else if (collision.GetComponent<PlayerMulti> ())
		if (collision.GetComponent<PlayerMulti> ().unitName == "EscortPlane")
			GetComponent<Rigidbody2D> ().AddForce (transform.right);
	}

	void OnTriggerEnter2D (Collider2D c)
	{
		//Get item's layer name
		string layerName = LayerMask.LayerToName (c.gameObject.layer);
		//If the ship did not collide with a player's bullet, ignore it
		if (layerName != "Bullet (Player)")
			return;

		//Get the bullet's Bullet script
		Bullet obj = c.GetComponent<Bullet> ();
		//Subtract bullet's damage from hit points
		if (currentHP > 0 && damageModifier <= 1) //so healthbars dont get lower than 0hp
            currentHP -= obj.power;
		else {
			if (currentHP > 0) {
				if (unitName == "Boss01" && damageModifier == 3)
					currentHP -= obj.power * 2;
				else if (unitName == "Boss02" && damageModifier == 3)
					currentHP -= obj.power * 2;
				else
					currentHP -= obj.power;
			}
		}
		//Setting the healthbar for the bos
		if (isBoss) {
			bossHealth.transform.localScale = new Vector3 (100f / hp * currentHP / 100f, bossHealth.transform.localScale.y, bossHealth.transform.localScale.z);
		}

		//If the ship is out of hit points...
		if (currentHP <= 0 && !dying) {
			dying = true;

			currentHP = 0;

			if (isTurret && GetComponentInParent<Enemy> ()) {
				transform.root.GetComponent<Enemy> ().IncreaseDamageModifier ();
			}

			//...add to the player's score...
			Manager.current.AddPoint (point, obj.owner, isObjective);
			//...call the parent Explode method...

			//spawn powerUp
			int rng = Random.Range (0, 10);

			if (powerUp && rng == 1)
				Instantiate (powerUp, transform.position, transform.rotation);

			Explode ();
			//...and deactivate this ship.

			if (isBoss)
				Manager.current.BossKilled ();
		} else {
			//play the damaged animation, debris is being ignored
			if (unitName != "DrillingPitDestroyed" && unitName != "BunkerDebris")
				animator.SetTrigger ("Damage");
		}

		if (unitName != "DrillingPitDestroyed" && unitName != "BunkerDebris")
			obj.Die ();
	}
}