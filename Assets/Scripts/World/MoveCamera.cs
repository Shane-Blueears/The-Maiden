using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject Player;
    //public float cameraDelay = 0.01f;
    public Vector3 cameraMovement;

    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = (this.transform.position - Player.transform.position);
        offset = new Vector3(offset.x,5.0f,0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*cameraMovement.x = Player.transform.position.x + offset.x;
        cameraMovement.y = this.transform.position.y;
        cameraMovement.z = this.transform.position.z;
        this.transform.position = cameraMovement;*/
        this.transform.position = Player.transform.position + offset;
    }
}
