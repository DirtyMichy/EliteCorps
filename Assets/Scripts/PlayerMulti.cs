using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.UI;

public class PlayerMulti : UnitObject
{
    public int currentPlayer = 0; //0 == no Player, 1 = Player 1
    public int maxHP = 10;
    public int currentHP;
    public int ammo = 128;
    public int specialAmmo = 1;
    private bool readyToFire = true;
    private bool specialRDY = true;
    public bool wrap = false; //wrap means you can fly off the screen and return on the other side
    public Image healthBar;
    public Image ammoBar;
    public TextMesh specialAmmoText;
    public TextMesh playerNumberText;
    private Vector2 directionCurrent;
    private GamepadInput.GamePad.Index[] gamePadIndex = { GamePad.Index.One, GamePad.Index.Two, GamePad.Index.Three, GamePad.Index.Four };

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
        if (currentPlayer != 0)
        {

        }

        for (int i = 1; i <= Manager.current.MAXPLAYERS; i++)
        {
            if (currentPlayer == i)
                directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, gamePadIndex[i]);
        }

        //Getting GamePad input, player 1 can be controlled by keyboard
        //Vector2 directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);
        /*


                if (currentPlayer == 1)
                {
                    directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);

                    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
                        directionCurrent = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                }

                if (currentPlayer == 2)
                    directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Two);
                if (currentPlayer == 3)
                    directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Three);
                if (currentPlayer == 4)
                    directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Four);
                    */

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
        /*
        //Normal bullets
        if (ammo > 0)
        {
            if (currentPlayer == 0)
            {
                if (bulletRDY)
                    StartCoroutine("WaitForBullet");
            }
            if (currentPlayer == 1 && (GamePad.GetButton(GamePad.Button.A, GamePad.Index.One) || Input.GetKey(KeyCode.A)))
            {
                if (bulletRDY)
                    StartCoroutine("WaitForBullet");
            }
            if (currentPlayer == 2 && GamePad.GetButton(GamePad.Button.A, GamePad.Index.Two))
            {
                if (bulletRDY)
                    StartCoroutine("WaitForBullet");
            }
            if (currentPlayer == 3 && GamePad.GetButton(GamePad.Button.A, GamePad.Index.Three))
            {
                if (bulletRDY)
                    StartCoroutine("WaitForBullet");
            }
            if (currentPlayer == 4 && GamePad.GetButton(GamePad.Button.A, GamePad.Index.Four))
            {
                if (bulletRDY)
                    StartCoroutine("WaitForBullet");
            }
        }
        */

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