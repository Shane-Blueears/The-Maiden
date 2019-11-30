using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    public float speed = 1f;

    public int damage = 5;

    Vector2 velocity;
    Rigidbody2D rb;
    Transform projectileSource;
    GameObject player;
    float lifeLength = 0f;
    public float totalLifeLength = 10f;
    public float offset = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponentInParent<TurretEnemyAI>().player;
        projectileSource = GetComponentInParent<TurretEnemyAI>().projectileSource;

        // Finds the vector pointing towards the player
        velocity = new Vector2(player.transform.position.x - projectileSource.position.x, 
                               player.transform.position.y - projectileSource.position.y).normalized;
        velocity += new Vector2(Random.Range(-offset, offset), Random.Range(-offset, offset));
        velocity = velocity.normalized;
        rb.velocity = velocity * speed;
    }

    // Update is called once per frame
    void Update()
    {
        //rb.AddForce(velocity * speed * Time.deltaTime);
        lifeLength += Time.deltaTime;

        if (lifeLength >= totalLifeLength)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.collider.Equals(player.GetComponent<Collider2D>()))
        {
            player.GetComponent<PlayerScript>().takeDamage(damage);
        }

        Destroy(this.gameObject);
    }
}
