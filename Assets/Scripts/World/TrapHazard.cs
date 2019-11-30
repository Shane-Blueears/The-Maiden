using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class TrapHazard : MonoBehaviour
{
    //Create a timer that allows damage from hazard every 2 seconds
    private static Timer timer;
    bool allowDamage = true;
    int damage = 10;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer(2000);
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void causeDamage(Object source, ElapsedEventArgs e)
    {
        allowDamage = true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(allowDamage && collision.collider.tag == "Player")
        {
            allowDamage = false;
            collision.collider.SendMessage("takeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}
