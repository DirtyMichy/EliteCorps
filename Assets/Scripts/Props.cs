//Pushing boats from islands and randomizing islandsprites on start

using UnityEngine;
using System.Collections;

public class Props : MonoBehaviour
{
    public float speed;
    public Sprite[] propVariants;
    protected Transform[] isles;

    void Awake()
    {
        GetComponentInParent<Rigidbody2D>().velocity = (transform.up * -1) * speed;

        if (GetComponentInChildren<SpriteRenderer>())
            GetComponentInChildren<SpriteRenderer>().sprite = propVariants[Random.Range(0, propVariants.Length - 1)];

        if (gameObject.tag == "Island")
        {
            isles = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                isles[i] = transform.GetChild(i);

            int rng = Random.Range(0, isles.Length);
            isles[rng].gameObject.SetActive(true);

            Vector3 rot = transform.eulerAngles;
            rot.z = Random.Range(0f, 360f);
            transform.eulerAngles = rot;
        }

    }

    //push boats to the top of the isle
    void OnTriggerStay2D(Collider2D c)
    {
        if (c.GetComponent<Enemy>() && gameObject.tag == "Island")
            if (c.GetComponent<Enemy>().unitName == "AttackBoat" || c.GetComponent<Enemy>().unitName == "FlakShip")
            {
                c.transform.position = new Vector2(c.transform.position.x, c.transform.position.y + 0.1f);
            }
    }
}