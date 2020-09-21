using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int speed = 10;
    public float lifeTime = 10;
    public int damage = 1;
    public int owner = 0;           //0 = None, 1 = Player1, ...
    public GameObject explosion;

    void OnEnable()
    {
        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().velocity = transform.up.normalized * speed;

        Invoke("Die", lifeTime);
    }

    public void SetOwner(int i)
    {
        owner = i;
    }

    public void Die()
    {
        if (explosion != null)
            Instantiate(explosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}