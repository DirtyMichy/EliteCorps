using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.UI;

//This script manages the player
public class PlayerMulti : Spaceship
{   

    public int currentPlayer = 0;       //0 == no Player, 1 = Player 1
    public int hp = 10;                 //Ship's max hit points    
    public int currentHP;               //Ship's current hit points
    public int ammo = 100, maxAmmo;     //Ship's ammo and maxAmmo
    public int specialAmmo = 4;         //Ship's special ammo

    private bool bulletRDY = true;      //you can only shoot when your gun is rdy
    private bool specialRDY = true;     //same goes for specialWeapons
    public bool wrap = false;           //wrapping means you can fly off the screen and return on the other side

    public Image healthBar;             //players healthBar
    public Image ammoBar;               //players ammoBar
    public TextMesh specialAmmoText;    //players specialAmmoBar |||
    public TextMesh playerNumberText;   //playerOwnersNumber

    //Setting the playerOwner of this plane from Manager
    public void SetPlaneValue(int playerCount)
    {        
        ammo = ammo/playerCount;
        currentHP = hp;
        maxAmmo = ammo;
        specialAmmo = specialAmmo/playerCount;

        //Debug.Log ("playerCount: " + playerCount);
    }

    //Setting the text of the playerOwner
    public void SetPlayer(int playerNumber)
    {
        currentPlayer = playerNumber;
    }
    
    void Update()
    {
        //Getting GamePad input, player 1 can be controlled by keyboard
        Vector2 directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);

        if (currentPlayer == 1)
        {
            directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One);

            if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
                directionCurrent = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        }

        if (currentPlayer == 2)
            directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Two);
        if (currentPlayer == 3)
            directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Three);
        if (currentPlayer == 4)
            directionCurrent = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.Four);

        //Debug.Log (directionCurrent.x);

        //Get our raw inputs
        float x = directionCurrent.x;
        float y = directionCurrent.y;
        
        //Normalize the inputs
        Vector2 direction = new Vector2(x, y).normalized;

//        Debug.Log("Math: " + (Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI -90));
//        Debug.Log("Z: " + transform.eulerAngles);

        //if(wrap && direction.x != 0 && direction.y != 0)
        if(unitName != "EscortPlane" && unitName != "Friendly")
        {
            if(wrap)
            {
                if(y >= 0)                
                    GetComponent<Rigidbody2D>().velocity = transform.up.normalized;
                if(y < 0)                
                    GetComponent<Rigidbody2D>().velocity = transform.up.normalized *-1;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI-90 , new Vector3(0, 0, 1)), Time.deltaTime * 1000f);

                //transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI -90, new Vector3(0, 0, 1));
            }
            else                           
                GetComponent<Rigidbody2D>().velocity = transform.up.normalized * 0;            

            if(wrap && direction.x == 0 && direction.y == 0)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(90, new Vector3(0, 0, 1)), Time.deltaTime * 1000f);

            //Move the player
            if (currentPlayer != 0)
                Move(direction);
        }
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

        //Specialweapons
        if (specialAmmo > 0)
        {
            if (currentPlayer == 1 && (GamePad.GetButton(GamePad.Button.X, GamePad.Index.One)  || Input.GetKey(KeyCode.X)))
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
        ammoBar.fillAmount = 100f/maxAmmo*ammo/100f;
        
        specialAmmoText.text = "";
        for(int i = 0; i < specialAmmo; i++)
        {
            specialAmmoText.text += "|";
        }
    }

    public void playerUIUpdate()
    {
        if(Manager.current.playerCount > 1)
            playerNumberText.text = ""+currentPlayer;
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
        bulletRDY = false;
        if(currentPlayer == 0)
            yield return new WaitForSeconds(Random.Range(0.5f,1f));

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
        bulletRDY = true;

    }
    
    void Move(Vector2 direction)
    {
        //Find the screen limits to the player's movement
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        //Get the player's current position
        Vector2 pos = transform.position;         //Objects current position
        //Calculate the proposed position
        //Debug.Log("X: " + pos.x + "Y: " + pos.y);
        //Debug.Log(wrap);

        Vector2 viewPortPosition;                              //Objects position in worldspace
        
        viewPortPosition = Camera.main.WorldToViewportPoint(pos);

        pos += direction * speed * Time.deltaTime;

        if(wrap)
        {
            GetComponent<Rigidbody2D>().freezeRotation=false;

            //Debug.Log("World: " + viewPortPosition);

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
            GetComponent<Rigidbody2D>().freezeRotation=true;
            //Ensure that the proposed position isn't outside of the limits

            pos.x = Mathf.Clamp(pos.x, min.x, max.x);
            pos.y = Mathf.Clamp(pos.y, min.y, max.y);
        


        }
        //Update the player's position
        transform.position = pos;
    }
    
    void OnTriggerEnter2D(Collider2D c)
    {
        //Get item's layer name
        string layerName = LayerMask.LayerToName(c.gameObject.layer);
        //If the ship did not collide with a player's bullet, ignore it
        if (layerName != "Bullet (Enemy)") 
            return;

        //Get the bullet's Bullet script
        Bullet obj = c.GetComponent<Bullet>();

        if (c.GetComponent<Bullet>().powerUpType == 0)
        {
            if (!isInvincible)
            {
                //Subtract bullet's damage from hit points
                currentHP -= obj.power;
                healthBar.fillAmount = 100f / hp * currentHP / 100f;
            }
        }
        else
        {
            if (c.GetComponent<Bullet>().powerUpType == 1)
            {
                ammo += 128;
            }else
            if (c.GetComponent<Bullet>().powerUpType == 2)
            {
                specialAmmo += 3;
            }
            else
            if (c.GetComponent<Bullet>().powerUpType == 3)
            {
                currentHP += 10;
                healthBar.fillAmount = 100f / hp * currentHP / 100f;
            }
        }

        obj.Die();

        //If the ship is out of hit points...
        if (currentHP <= 0)
           {
            Manager.current.PlayDeathSound();
            Explode();
                        
        }
        else
        {
            //Otherwise, play the damaged animation
            animator.SetTrigger("Damage");          
        }
    }
}