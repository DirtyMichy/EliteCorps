using UnityEngine;
using System.Collections;

public class IsleRemoval : MonoBehaviour
{
    public float lifeTime = 0;

    void Awake()
    {
        StartCoroutine("lifeTimeCounter");
    }

    void OnTriggerStay2D(Collider2D c)
    {
        //Get item's layer name
        string layerName = LayerMask.LayerToName(c.gameObject.layer);

        if (layerName == "Enemy")
        {
            if (c.GetComponent<Enemy>().isGroundUnit && c.tag != "Cannon" && c.GetComponent<Enemy>().isGroundUnit && c.tag != "BunkerDebris" && !c.GetComponent<Enemy>().isBoss && !c.GetComponent<Enemy>().isObjective)
            {
                //Destroy(c.gameObject);
                Vector3 newPos = c.transform.position;
                newPos.y+=0.1f;
                c.transform.position = newPos;
                Debug.Log("Boat pushed");
            } 
        } 
        else
        {
            if (c.tag == "Island")
            {
                if (c.GetComponent<IsleRemoval>().lifeTime > lifeTime)
                    Destroy(gameObject.transform.parent.gameObject);
                else
                    Destroy(c.transform.parent.gameObject);
                Debug.Log("Isles overlapping - deleted");
            }
        }
    }

    IEnumerator lifeTimeCounter()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            lifeTime++;
        }
    }
}
