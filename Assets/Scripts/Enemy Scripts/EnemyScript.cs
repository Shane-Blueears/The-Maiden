using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    public bool isFacingLeft;

    public int damage;

    public int startingHealth = 10;
    private int currentHealth;
    
    public float attackRadius;

    public float pathlength;

    public float movementSpeed;
    
    // The amount of time the enemy is required to wait whent he player is in range before it attacks (in seconds).
    public float attackDelayTime = 2f;

    public PlayerScript playerScript;

    private float startingXPos;

    // Keeps track of whether or not the enemy is currently attacking
    public bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the enemy's starting health
        currentHealth = startingHealth;

        // Gets the enemy's starting x position for movement
        startingXPos = this.transform.position.x;

        isAttacking = false;

        isFacingLeft = true;
    }

    // Update is called once per frame
    void Update()
    {
        // If the enemy isn't already attacking, check if the player is within attack range
        if (!isAttacking)
        {
            // Moves the player according to the desired path
            if (isFacingLeft)
            {
                // If the enemy should change directions
                if (startingXPos - this.transform.position.x > pathlength / 2f) // Change directions
                {
                    isFacingLeft = false;
                    this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
                }
                // Moves the enemy
                GetComponent<Rigidbody2D>().AddForce(new Vector2(-1f, 0f) * movementSpeed);
            }
            else
            {
                // If the enemy should change directions
                if (startingXPos - this.transform.position.x < -pathlength / 2f) // Change directions
                {
                    isFacingLeft = true;
                    this.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
                }
                GetComponent<Rigidbody2D>().AddForce(new Vector2(1f, 0f) * movementSpeed);
            }
            
            // Checks if a player is within attack range
            Collider2D[] playerCol = Physics2D.OverlapCircleAll(this.transform.position, attackRadius);
            foreach (Collider2D player in playerCol)
            {
                if (player.CompareTag("Player"))
                {
                    // Make the enemy face the player
                    if (this.transform.position.x < player.transform.position.x) // Player is to the right of the enemy
                    {
                        isFacingLeft = false;
                        this.transform.localScale = new Vector2(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
                    }
                    else // Player is to the left of the enemy
                    {
                        isFacingLeft = true;
                        this.transform.localScale = new Vector2(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y);
                    }

                    // Attacks the player after the attackDelayTime
                    Invoke("attack", attackDelayTime);
                    isAttacking = true;
                }
            }
        }
        else
        {
            //GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Attacks on the left side of the enemy.
    /// </summary>
    private void attack()
    {
        // Commented out parts will will be used when the player is turned into a sprite
        Collider2D[] playerCol = Physics2D.OverlapCircleAll(this.transform.position, attackRadius);
        foreach (Collider2D player in playerCol)
        {
            // If the collider belongs to the player
            if (player.CompareTag("Player"))
            {
                playerScript.takeDamage(damage);
            }
        }
        isAttacking = false;
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
        // Destorys the enemy gameobject
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Draws the enemy's attack radius
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draws the enemy attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, attackRadius);

        // Draws the enemy's path
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(this.transform.position - new Vector3(pathlength / 2f, 0f), this.transform.position + new Vector3(pathlength / 2f, 0f));
    }
}
