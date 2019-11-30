using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyAIScript : MonoBehaviour
{

    public PlayerScript playerScript;
    public Scoreboard score;

    [Header("Movement")]
    public bool isFacingLeft;

    public bool standStill = true;

    public float movementSpeed = 5f;

    public float pathlength = 5f;

    private float startingXPos;

    [Header("Attack")]
    public int damage;

    public int startingHealth = 10;
    private int currentHealth;

    public float attackRadius;

    // The amount of time the enemy is required to wait whent he player is in range before it attacks (in seconds).
    public float attackDelayTime = 2f;

    // Keeps track of whether or not the enemy is currently attacking
    public bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        // The default direction the enemy sprite is facing
        isFacingLeft = true;
        
        currentHealth = startingHealth;

        // Gets the starting X position the enemy will use to walk left and right
        startingXPos = this.transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If the player is not currently attacking
        if (!isAttacking)
        {

        }
    }

    /// <summary>
    /// Applies the specified amount of damage to the enemy.
    /// </summary>
    /// <param name="damage">The amount of damage applied to the player</param>
    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        // TODO: Add some epic particle effects and other cool stuff here

        if (currentHealth <= 0)
        {
            killEnemy();
        }
    }

    /// <summary>
    /// Kills the enemy.
    /// </summary>
    public void killEnemy()
    {
        score.enemyKilled();
        Destroy(this.gameObject);
    }
}
