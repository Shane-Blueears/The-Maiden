using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEnemyAIScript : MonoBehaviour
{
    public Transform parent;
    public Scoreboard score;

    public bool isFacingLeft = true;

    public int damage;
    
    public int startingHealth = 10;
    private int currentHealth;

    public float playerKnockback = 1.0f;

    // The length of path the flying enemy is following
    public Transform lPoint;
    public Transform rPoint;

    public float movementSpeed = 5f;
    public float diveSpeed = 20f;

    public float maximumDiveHeight = 10f;

    // The amount of time the enemy is required to wait whent he player is in range before it attacks (in seconds).
    public float attackDelayTime = 2f;

    public GameObject player;

    private float startingXPos;

    // Keeps track of whether or not the enemy is currently attacking
    public bool isAttacking;

    bool isDiving = false;

    private Rigidbody2D rb;

    Seeker seeker;
    // The path that the enemy is currently following
    Path path;
    // The waypoint on the path that the enemy is currently heading towards
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    // How close the enemy must get to the current waypoint in the path in order to move onto the next waypoint
    public float nextWaypointDistance = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Pathfinding
        seeker = GetComponent<Seeker>();

        rb = GetComponent<Rigidbody2D>();

        // Sets the enemy's starting health
        currentHealth = startingHealth;

        isAttacking = false;

        // Registers the enemy
        EnemyManager.Instance.registerEnemy(this.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Moves the enemy along the path if there is an active path
        if (path == null)
        {
            if (isPlayerInRange() && !isAttacking)
            {
                // Faces the boss towards the player
                if (this.transform.position.x - player.transform.position.x > 0f)
                {
                    this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
                    isFacingLeft = true;
                }
                else
                {
                    this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
                    isFacingLeft = false;
                }

                currentWaypoint = 0;
                seeker.StartPath(this.transform.position, player.transform.position, OnPathComplete);
                isAttacking = true;
                isDiving = true;
                rb.isKinematic = false;
            }
            else if (!isAttacking)
            {
                move();
            }
            return;
        }

        Debug.Log("b" + Time.fixedTime + "currentWaypoint: " + currentWaypoint + " pathcount: " + path.vectorPath.Count);
        // If the enemy has reached the end of the path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("a" + Time.fixedTime + "currentWaypoint: " + currentWaypoint + " pathcount: " + path.vectorPath.Count);
            path = null;
            if (isDiving)
            {
                isDiving = false;
                if (isFacingLeft)
                {
                    currentWaypoint = 0;
                    seeker.StartPath(this.transform.position, lPoint.transform.position, OnPathComplete);
                }
                else
                {
                    currentWaypoint = 0;
                    seeker.StartPath(this.transform.position, rPoint.transform.position, OnPathComplete);
                }
            }
            else
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
                isAttacking = false;
                rb.isKinematic = true;
            }
            return;
        }
        
        dive();

        // Checks if the player should start heading to the next waypoint on the path
        float waypointDistance = Vector2.Distance(this.transform.position, path.vectorPath[currentWaypoint]);
        if (waypointDistance <= nextWaypointDistance)
        {
            Debug.Log(Time.fixedTime + "waypointdist: " + waypointDistance);
            currentWaypoint++;
        }
    }

    /// <summary>
    /// Makes the flying enemy dive towards the player.
    /// </summary>
    private void dive()
    {
        // Moves the enemy towards the player
        Vector2 diff = ((Vector2)path.vectorPath[currentWaypoint] - rb.position);
        diff.Normalize();
        Vector2 movePos = diff * diveSpeed * Time.deltaTime;
        rb.AddForce(movePos);
    }

    /// <summary>
    /// Moves the fly8ing enemy every fixed update.
    /// </summary>
    private void move()
    {
        Vector2 movePos;

        // Moves the player according to the desired path
        if (isFacingLeft)
        {
            // If the enemy should change directions
            if (this.transform.position.x < lPoint.position.x) // Change directions
            {
                isFacingLeft = false;
                this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
            }
            movePos = new Vector2(-1f, 0f) * movementSpeed * Time.deltaTime;
        }
        else
        {
            // If the enemy should change directions
            if (this.transform.position.x > rPoint.position.x) // Change directions
            {
                isFacingLeft = true;
                this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
            }
            movePos = new Vector2(1f, 0f) * movementSpeed * Time.deltaTime;
        }
        rb.MovePosition(rb.position + movePos);
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
            currentWaypoint = 0;
        }
    }

    bool isPlayerInRange()
    {
        float dif = this.transform.position.y - player.transform.position.y;
        if (player.transform.position.x > lPoint.transform.position.x
            && player.transform.position.x < rPoint.transform.position.x
            && dif < maximumDiveHeight 
            && dif > 0f)
        {
            return true;
        }
        return false;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.Equals(player.GetComponent<Collider2D>())) // The collider belongs to the player
        {
            if (isAttacking)
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), true);

            // Damages the player
            player.GetComponent<PlayerScript>().takeDamage(damage);
            // Knockback
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Vector2 kb;
            if (this.transform.position.x - player.transform.position.x > 0f)
            {
                Debug.Log("MOVE");
                kb = new Vector2(-20f, 0f);//.normalized;
            }
            else
            {
                Debug.Log("MOVE");
                kb = new Vector2(20f, 0f);//.normalized;
            }
            kb *= playerKnockback;
            rb.velocity = kb;
        }
    }

    /// <summary>
    /// Gets the players current health value.
    /// </summary>
    public int getHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Updates the player's current health value.
    /// </summary>
    public void updateHealth(int health)
    {
        currentHealth = health;
    }

    /// <summary>
    /// Applies the specified amount of damage to the player.
    /// </summary>
    public void takeDamage(int damage)
    {
        // Decreases the enemy's health
        currentHealth -= damage;

        // If the enemy has 0 health kill them
        if (currentHealth <= 0)
        {
            killEnemy();
        }
    }

    /// <summary>
    /// Kills the enemy when they run out of health.
    /// </summary>
    private void killEnemy()
    {
        score.enemyKilled();
        // Destorys the enemy gameobject
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Draws the enemy's attack radius
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draws the enemy's path
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(lPoint.position, rPoint.position);
        //Gizmos.DrawLine(this.transform.position - new Vector3(pathLength / 2f, 0f), this.transform.position + new Vector3(pathLength / 2f, 0f));

        // Draws the maximum depth that the enemy will dive
        Gizmos.color = Color.white;
        Gizmos.DrawLine(this.transform.position, this.transform.position - new Vector3(0f, maximumDiveHeight, 0f));
    }
}
