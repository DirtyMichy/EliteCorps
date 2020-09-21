using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AcquireTarget : MonoBehaviour
{
    public GameObject currentTarget = null;
    public float rotationSpeed = 10f;
    public float range = 8f;
    public bool targetAcquired = false;
    void Update()
    {
        if (transform.position.y < 6f) //Dont target players if outside of viewport (important for campaign)
        {
            float minimalEnemyDistance = float.MaxValue;
            GameObject[] playerAlive = null;

            GameObject[] escortPlane = GameObject.FindGameObjectsWithTag("Escort");

            if (escortPlane.Length > 0)
                playerAlive = GameObject.FindGameObjectsWithTag("Escort");
            else
                playerAlive = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in playerAlive)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);

                if (distance < minimalEnemyDistance && distance < range)
                {
                    currentTarget = player;
                    minimalEnemyDistance = distance;
                    targetAcquired = true;
                }
            }

            if (currentTarget != null)
            {
                Vector3 direction = transform.position - currentTarget.transform.position;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI + 90, new Vector3(0, 0, 1)), Time.deltaTime * 50f);

                float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

                if (distance > range)
                    currentTarget = null;
            }
        }
    }
}