using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    public float time = 0.25f;

    public void Awake()
    {
        StartCoroutine("PulseLoop");
    }

    public IEnumerator PulseLoop()
    {
        while (true)
        {
            iTween.ScaleBy(gameObject, iTween.Hash("amount", new Vector3(2f,2f,0f), "easeType", "easeInOutExpo", "looptype", "pingpong", "time", time));

            yield return new WaitForSeconds(time*2);
        }
    }
}