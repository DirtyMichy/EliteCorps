using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRefiller : MonoBehaviour
{
    private int gameTimeSeconds;

    void Awake()
    {
        StartCoroutine("AmmoRefill");
    }
    
    //Refilling players ammo
    IEnumerator AmmoRefill()
    {
        //Loop indefinitely
        while (true)
        {
            gameTimeSeconds++;
            GameObject[] playerAlive = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < playerAlive.Length; i++)
            {
<<<<<<< HEAD
                if (playerAlive[i].GetComponent<PlayerMulti>())
                {
                    playerAlive[i].GetComponent<PlayerMulti>().ammo++;
                    if (gameTimeSeconds % 60 == 0)
                        playerAlive[i].GetComponent<PlayerMulti>().specialAmmo++;
                    playerAlive[i].GetComponent<PlayerMulti>().AmmoUIUpdate();
=======
                if (playerAlive[i].GetComponent<Player>())
                {
                    playerAlive[i].GetComponent<Player>().ammo++;
                    if (gameTimeSeconds % 60 == 0)
                        playerAlive[i].GetComponent<Player>().specialAmmo++;
                    playerAlive[i].GetComponent<Player>().AmmoUIUpdate();
>>>>>>> 4f64e573fbe3ab20ee454def2875e30b7671ba23
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
