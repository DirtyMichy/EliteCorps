using UnityEngine;
using System.Collections;

public class MoveObjectDown : MonoBehaviour
{
    public float speed = 1f;
    void OnEnable()
    {       
            if (GetComponent<Rigidbody2D>())
                GetComponent<Rigidbody2D>().velocity = (transform.up) * -speed;
    }
}