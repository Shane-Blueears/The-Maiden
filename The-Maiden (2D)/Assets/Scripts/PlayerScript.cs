using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //movement setup
        Vector3 movement = new Vector3();
        movement.x = Input.GetAxis("Horizontal");

        //jump setup
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.y = 5 * speed;
        }

        //Dash setup
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            movement.x = 10 * speed;
        }
        //Move the player
        this.GetComponent<Rigidbody>().AddForce(movement * speed);
    }
}
