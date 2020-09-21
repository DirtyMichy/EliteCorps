using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Invoke("DestroySpawnedAudio", GetComponent<AudioSource>().clip.length);
    }

    private void DestroySpawnedAudio()
    {
        Destroy(gameObject);
    }
}