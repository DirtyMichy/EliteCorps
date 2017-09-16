using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour 
{
    public float direction = 0f;
	public float speed;						//Ship's speed
    public Sprite Cloud01, Cloud02, Cloud03;

	// Use this for initialization
	void Awake () 
    {
        SetDirection();

		int r = Random.Range (0, 3);

		switch (r) 
        {
		case 0:
			GetComponentInChildren<SpriteRenderer>().sprite = Cloud01;
			break;
		case 1:
			GetComponentInChildren<SpriteRenderer>().sprite = Cloud02;
			break;
		case 2:
			GetComponentInChildren<SpriteRenderer>().sprite = Cloud03;
			break;
		}

        if(tag == "Props")
        {
            Vector3 rot = transform.eulerAngles;
    		rot.z = Random.Range (0f, 360f);
            transform.eulerAngles = rot;
        }
    }

    public void SetDirection()
    {
        if (GetComponent<Rigidbody2D>())
        {
            if(direction == 0f)
                GetComponentInParent<Rigidbody2D>().velocity = (transform.up * -1) * speed;
            else
                if(direction == 1f)
                    GetComponentInParent<Rigidbody2D>().velocity = (transform.right * 1) * speed;
            else
                if(direction == -1f)
                    GetComponentInParent<Rigidbody2D>().velocity = (transform.right * -1) * speed;
        }
    }
}