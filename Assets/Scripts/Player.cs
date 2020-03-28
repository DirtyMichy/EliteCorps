using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.UI;

public class Player : UnitObject
{
    public int currentPlayer = 0; //0 == no Player, 1 = Player 1
    public int maxHP = 10;
    public int currentHP;
    public int ammo = 128;
    public int specialAmmo = 1; public float shotDelay;
    public int bulletsPerShot = 1;
    public GameObject shield;
    public Transform[] shotPositions;
    public Transform[] shotSpecialPositions;
    public GameObject bullet;
    public GameObject specialBullet;
    public GameObject rocket;
    private bool readyToFire = true;
    private bool specialRDY = true;
    public bool wrap = false; //wrap means you can fly off the screen and return on the other side
    public Image healthBar;
    public Image ammoBar;
    public TextMesh specialAmmoText;
    public TextMesh playerNumberText;
    private Vector2 directionCurrent;
    public int localPlayer;
    private GamepadInput.GamePad.Index[] gamePadIndex = { GamePad.Index.One, GamePad.Index.Two, GamePad.Index.Three, GamePad.Index.Four };

    void Awake()
    {
        shotPositions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            shotPositions[i] = transform.GetChild(i);

        animator = GetComponent<Animator>();
    }

    //Setting the playerOwner of this plane from Manager
    public void SetPlaneValue(int playerCount)
    {
        ammo = ammo / playerCount;
        currentHP = maxHP;
        specialAmmo = specialAmmo / playerCount;
        playerUIUpdate();
    }

    //Setting the text of the playerOwner
    public void SetPlayer(int playerNumber)
    {
        currentPlayer = playerNumber;
    }

    void Update()
    {
        for (int i = 1; i <= Manager.current.MAXPLAYERS; i++)
        {
            if (currentPlayer == i)
                directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, gamePadIndex[i]);
        }

        float x = directionCurrent.x;
        float y = directionCurrent.y;
        Vector2 direction = new Vector2(x, y).normalized;

        if (unitName != "EscortPlane" && unitName != "Friendly")
        {
            if (wrap)
            {
                if (y >= 0)
                    GetComponent<Rigidbody2D>().velocity = transform.up.normalized;
                if (y < 0)
                    GetComponent<Rigidbody2D>().velocity = transform.up.normalized * -1;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI - 90, new Vector3(0, 0, 1)), Time.deltaTime * 1000f);
            }
            else
                GetComponent<Rigidbody2D>().velocity = transform.up.normalized * 0;

            if (wrap && direction.x == 0 && direction.y == 0)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(90, new Vector3(0, 0, 1)), Time.deltaTime * 1000f);

            //Move the player
            if (currentPlayer != 0)
                Move(direction);
        }

        for (int i = 0; i <= Manager.current.MAXPLAYERS; i++)
        {
            if (currentPlayer == i && ammo > 0 && readyToFire && (GamePad.GetButton(GamePad.Button.A, gamePadIndex[i]) || currentPlayer == 0))
                StartCoroutine("WaitForBullet");
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (currentPlayer == 1 && ammo > 0 && readyToFire)
                StartCoroutine("WaitForBullet");
        }

        //Specialweapons
        if (specialAmmo > 0)
        {
            if (currentPlayer == 1 && (GamePad.GetButton(GamePad.Button.X, GamePad.Index.One) || Input.GetKey(KeyCode.X)))
            {
                if (specialRDY)
                    StartCoroutine("WaitForSpecialBullet");
            }
            if (currentPlayer == 2 && GamePad.GetButton(GamePad.Button.X, GamePad.Index.Two))
            {
                if (specialRDY)
                    StartCoroutine("WaitForSpecialBullet");
            }
            if (currentPlayer == 3 && GamePad.GetButton(GamePad.Button.X, GamePad.Index.Three))
            {
                if (specialRDY)
                    StartCoroutine("WaitForSpecialBullet");
            }
            if (currentPlayer == 4 && GamePad.GetButton(GamePad.Button.X, GamePad.Index.Four))
            {
                if (specialRDY)
                    StartCoroutine("WaitForSpecialBullet");
            }
        }
    }

    public void AmmoUIUpdate()
    {
        ammoBar.fillAmount = ammo / 100f;

        specialAmmoText.text = "";
        for (int i = 0; i < specialAmmo; i++)
        {
            specialAmmoText.text += "|";
        }
    }

    public void playerUIUpdate()
    {
        if (Manager.current.playerCount > 1)
            playerNumberText.text = "" + currentPlayer;
        else
            playerNumberText.text = "";
    }

    IEnumerator WaitForSpecialBullet()
    {
        specialAmmo--;
        specialRDY = false;
        ShootSpecialViaButton(currentPlayer);
        yield return new WaitForSeconds(0.5f);
        specialRDY = true;
    }

    IEnumerator WaitForBullet()
    {
        ammo--;
        readyToFire = false;
        if (currentPlayer == 0)
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));

        ShootViaButton(currentPlayer);

        yield return new WaitForSeconds(0.1f);
        if (unitName == "Char07")
        {
            ShootViaButton(currentPlayer);
            yield return new WaitForSeconds(0.1f);
            ShootViaButton(currentPlayer);
            yield return new WaitForSeconds(0.1f);
            ShootViaButton(currentPlayer);
            yield return new WaitForSeconds(0.1f);
        }
        readyToFire = true;
    }

    void Move(Vector2 direction)
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        Vector2 pos = transform.position;
        Vector2 viewPortPosition;                              //Objects position in worldspace

        viewPortPosition = Camera.main.WorldToViewportPoint(pos);

        pos += direction * speed * Time.deltaTime;

        if (wrap)
        {
            GetComponent<Rigidbody2D>().freezeRotation = false;

            if (viewPortPosition.x > 1)
                pos.x = Camera.main.ViewportToWorldPoint(Vector2.zero).x;
            if (viewPortPosition.x < 0)
                pos.x = Camera.main.ViewportToWorldPoint(Vector2.one).x;
            if (viewPortPosition.y > 1)
                pos.y = Camera.main.ViewportToWorldPoint(Vector2.zero).y;
            if (viewPortPosition.y < 0)
                pos.y = Camera.main.ViewportToWorldPoint(Vector2.one).y;
        }
        else
        {
            GetComponent<Rigidbody2D>().freezeRotation = true;

            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
        }

        transform.position = pos;
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

    void OnTriggerEnter2D(Collider2D c)
    {
        string layerName = LayerMask.LayerToName(c.gameObject.layer);

        if (layerName != "Bullet (Enemy)")
            return;

        if (c.GetComponent<Bullet>() && !isInvincible)
        {
            currentHP -= c.GetComponent<Bullet>().damage;
            c.GetComponent<Bullet>().Die();

            if (currentHP <= 0)
            {
                Manager.current.PlayDeathSound();
                Explode();
            }
            else
            {
                animator.SetTrigger("Damage");
            }
        }

        if (c.GetComponent<PowerUp>())
        {
            ammo += c.GetComponent<PowerUp>().bonusAmmo;
            specialAmmo += c.GetComponent<PowerUp>().bonusSpecial;
            currentHP += c.GetComponent<PowerUp>().bonusHealth;
            Destroy(c);
        }

        healthBar.fillAmount = 100f / maxHP * currentHP / 100f;
    }
}