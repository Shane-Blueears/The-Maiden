using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject Player;
    public float cameraDelay = 0.01f;

    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = (this.transform.position - Player.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Player.transform.position + offset;
        //transform.position = Vector3.Lerp(transform.position, Player.transform.position+offset, cameraDelay);
    }
}
