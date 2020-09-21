using UnityEngine;

public class DestroyArea : MonoBehaviour
{
	void OnTriggerExit2D (Collider2D c)
	{
        if(c.tag == "Level")
            Destroy(c.transform.root.gameObject);
        else
            Destroy(c.gameObject);
	}
}