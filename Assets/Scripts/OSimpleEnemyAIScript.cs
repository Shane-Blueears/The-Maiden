using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class OSimpleEnemyAIScript : MonoBehaviour
{
    [Header("Movement")]

    public float speed = 10f;

    public float attackRange = 2f;

    [Header("Pathfinding")]

    public Transform target;
    public bool isTargetVisible = false;

    // How close the enemy must get to the current waypoint in the path in order to move onto the next waypoint
    public float nextWaypointDistance = 5f;

    // The maximum distance that the enemy can detect the player
    public float viewDistance = 10f;

    // If the enemy should be able to see the enemy through walls.
    // public bool seeThroughWalls = true;

    // The path that the enemy is currently following
    Path path;
    // The waypoint on the path that the enemy is currently heading towards
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rigidBody;

    /*
     TEMP NOTES ON HOW TO IMPLEMENT:
     I will use moveposition to move the player so they don't slide extra far but they also use colliders so they can
     walk up and down inclines easily. Actually I might have to do some hacky work in order to make going down inclines work.
     Maybe not hacky but maybe just the regular velocity method. Or just use moveposition at a downwards angle is probably best/easiest.
     Also only move player left and right (or downwards slightly in that case) and when it points directly up (or if there is a wall but
     at the point it should be pointing directly upwards) the enemy will jump.
     */

    // Start is called before the first frame update
    void Start()
    {
        
        seeker = GetComponent<Seeker>();
        rigidBody = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    /// <summary>
    /// Updates the path leading from the enemy to the target if the target is within view distance of the enemy.
    /// </summary>
    void UpdatePath()
    {
        if (isTargetVisible)
            seeker.StartPath(this.transform.position, target.position, OnPathComplete);
    }

    /// <summary>
    /// Called when a path was created by the seeker.
    /// </summary>
    /// <param name="p">The path that was created</param>
    void OnPathComplete(Path p)
    {
        // If the path was successfully made
        if (!p.error)
        {
            path = p;
            currentWaypoint = 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Vector2.Distance(this.transform.position, target.position);
        isTargetVisible = distance <= viewDistance;
        
        if (distance <= attackRange) // If the enemy is within attack range of the target
        {
            // TODO: Attack in the direction of the player
        }
        else if (!isTargetVisible) // If the terget is not within view distance
        {
            path = null;
        }


        // Moves the enemy along the path if there is an active path
        if (path == null)
            return;

        // If the enemy has reached the end of the path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        // Moves the player on the x axis towards the player
        Vector2 xDiff = ((Vector2)path.vectorPath[currentWaypoint] - rigidBody.position) * new Vector2(1f, 0f);
        xDiff.Normalize();
        Vector2 movePos = xDiff * speed * Time.deltaTime;
        //Debug.Log(movePos.x + " " + movePos.y + "   " + rigidBody.position.x + " " + rigidBody.position.y + "     " + xDiff.x + " " + xDiff.y + "     " + Time.deltaTime);
        rigidBody.MovePosition(rigidBody.position + movePos);

        // Checks if the player should start heading to the next waypoint on the path
        float waypointDistance = Vector2.Distance(this.rigidBody.position, path.vectorPath[currentWaypoint]);
        if (waypointDistance <= nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    /// <summary>
    /// Draws the enemy's attack radius
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draws the enemy's view distance
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, viewDistance);

        // Draws the enemy's attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }
}
