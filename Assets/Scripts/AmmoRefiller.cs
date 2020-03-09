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
                if (playerAlive[i].GetComponent<PlayerMulti>())
                {
                    playerAlive[i].GetComponent<PlayerMulti>().ammo++;
                    if (gameTimeSeconds % 60 == 0)
                        playerAlive[i].GetComponent<PlayerMulti>().specialAmmo++;
                    playerAlive[i].GetComponent<PlayerMulti>().AmmoUIUpdate();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
