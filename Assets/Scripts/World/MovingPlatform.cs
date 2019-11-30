using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    public GameObject platform;
    [Tooltip("There MUST be at least 2 points")]
    public GameObject[] points;
    [Tooltip("If this is checked the moving platform will travel back to the first point in the array once it reaches the end instead of going back through the points backwards.")]
    public bool loop = false;
    public float speed = 1f;

    bool reverse = false;
    int currentPoint = 1;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = platform.GetComponent<Rigidbody2D>();
        rb.position = points[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Updates the current point if necessary
        if (Vector2.Distance(platform.transform.position, points[currentPoint].transform.position) <= 0.1f)
        {
            if (reverse)
            {
                // If the platform has reached the final point
                if (--currentPoint < 0)
                {
                    currentPoint = 1;
                    reverse = false;
                }
            }
            else
            {
                // If the platform has reached the final point
                if (++currentPoint >= points.Length)
                {
                    // If loop is true platform heads towards 0, otherwise it approaches the previous point
                    if (loop)
                    {
                        currentPoint = 0;
                    }
                    else
                    {
                        currentPoint = currentPoint - 2;
                        reverse = true;
                    }
                }
            }
        }

        // Finds the vector pointing towards the player
        Vector2 pos = new Vector2(points[currentPoint].transform.position.x - rb.position.x,
                                  points[currentPoint].transform.position.y - rb.position.y).normalized;
        pos *= speed * 1f;
        rb.velocity = pos;
    }

    /// <summary>
    /// Draws the attack radius for all of the player's attacks
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Platform paths
        Gizmos.color = Color.gray;
        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.DrawLine(points[i - 1].transform.position, points[i].transform.position);
        }

        if (loop)
            Gizmos.DrawLine(points[0].transform.position, points[points.Length - 1].transform.position);
    }

}
