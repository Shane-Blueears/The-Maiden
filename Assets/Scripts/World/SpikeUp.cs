using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeUp : MonoBehaviour
{
    Rigidbody2D rb;
    Vector3 change;
    int damage = 15;
    float fallspeed = 0.3f;
    bool willMove;
    Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        willMove = false;
        change = new Vector3(0,0.1f,0);
        startingPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //This moves it upwards
        if (willMove)
        {
            this.transform.position += change;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //rb.isKinematic = false;
            willMove = true;
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
        willMove = false;
        this.transform.position = startingPos;
    }
}
