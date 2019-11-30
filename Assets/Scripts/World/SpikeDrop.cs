using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDrop : MonoBehaviour
{
    Rigidbody2D rb;
    int damage = 15;
    float fallspeed = 50f;
    Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startingPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //This moves it upwards
        //this.transform.position += Vector3.up;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            rb.isKinematic = false;
            rb.gravityScale = fallspeed;
            Invoke("resetTrap", 4.0f);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    { 
        collision.collider.SendMessage("takeDamage", damage, SendMessageOptions.DontRequireReceiver);
        //Destroy(this.gameObject);
        resetTrap();
        Debug.Log(collision.collider.tag);
    }

    void resetTrap()
    {
        rb.isKinematic = true;
        rb.gravityScale = 0;
        this.transform.position = startingPos;
    }
}
