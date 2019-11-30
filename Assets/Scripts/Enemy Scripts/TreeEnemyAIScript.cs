using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
// STOP ENEMY FROM WALKING OFF CLIFFS IN THE MORMING
public class TreeEnemyAIScript : MonoBehaviour
{
    [Header("Attack")]

    public int startingHealth = 10;
    public int currentHealth;

    public int damage = 5;

    public GameObject player;
    public Scoreboard score;

    public float attackCooldown = 5f;
    private float currentAttackCooldown = 0f;

    public float attackRange = 2f;

    // The knockback multiplier applied to the player when they get hit by the tree enemy's attack
    public float playerKnockback = 2f;

    public Animator spriteAnimator;

    // The BoxCollider2D that represents the attack damage area
    public BoxCollider2D attackAreaTrigger;

    // The length of the attack area trigger animation
    public float attackAreaTriggerAnimationTime = 1f;

    // The animater that animates the attack area trigger box collider
    public Animator attackAreaTriggerAnimator;

    // If the enemy is currently attacking
    private bool isAttacking = false;

    [Header("Movement")]

    public Transform target;

    /*
    THIS ENEMY NO LONGER MOVES
    public float speed = 2f;

    // The points that tracks whether or not the enemy is about to walk off the edge
    // public Transform groundPt;
    // public Collider2D tileMapCollider;
    */

    // The maximum distance that the enemy can detect the player
    public float viewDistance = 10f;

    public bool isFacingLeft = true;
    
    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        currentHealth = startingHealth;

        // Uncomment out if the player should be able to walk through the enemy
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), true);

        // The enemy starts with the attack area trigger disabled
        attackAreaTrigger.enabled = false;

        if (!isFacingLeft)
        {
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        }

        // Registers the enemy
        EnemyManager.Instance.registerEnemy(this.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Don't do anything if the enemy is attacking
        if (isAttacking)
            return;
        
        // The distance away from the target
        float distance = Vector2.Distance(this.transform.position, target.position);

        if (currentAttackCooldown > 0f)
        {
            currentAttackCooldown -= Time.deltaTime;
        }

        if (distance <= attackRange) // Terget is within attack range
        {
            if (currentAttackCooldown <= 0f)
            {
                attack();
            }
        }
        else if (distance <= viewDistance) // Target is within view distance
        {
            // move();
        }
    }

    /// <summary>
    /// Moves the enemy towards the target.
    /// </summary>
    private void move()
    {
        // Moves the player on the x axis towards the target
        Vector2 xDir = new Vector2(target.position.x - this.transform.position.x, 0f).normalized;
        
        // Face the enemy in the direction they are moving
        // this.transform.localScale = new Vector3(-xDir.x * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        /*
        THIS ENEMY NO LONGER MOVES
        // Ensures the enemy won't walk off a cliff by shooting a ray of length 5 downwards
        RaycastHit2D groundInfo = Physics2D.Raycast(groundPt.position, Vector2.down, 10f);
        if (groundInfo.collider == false || groundInfo.collider != tileMapCollider)
        {
            return;
        }

        // Move the enemy
        Vector2 movePos = xDir * speed * Time.deltaTime;
        rigidBody.MovePosition(rigidBody.position + movePos);
        // Trigger walking animation
        */
    }

    /// <summary>
    /// Initiates the enemy's attack.
    /// </summary>
    private void attack()
    {
        isAttacking = true;
        attackAreaTrigger.enabled = true;
        attackAreaTriggerAnimator.SetTrigger("AttackTrigger");
        // Calls when the attack animation is completed
        Invoke("endAttack", attackAreaTriggerAnimationTime);
        currentAttackCooldown = attackCooldown + attackAreaTriggerAnimationTime;

        // Trigger attack animation
        spriteAnimator.SetTrigger("Attack");
    }

    /// <summary>
    /// Invoked when the attack animation is completed.
    /// </summary>
    private void endAttack()
    {
        isAttacking = false;
        attackAreaTrigger.enabled = false;
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            killEnemy();
        }
    }

    public void killEnemy()
    {
        EnemyManager.Instance.removeEnemy(this.gameObject);
        score.enemyKilled();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Something has been hit by the attack.
    /// </summary>
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.Equals(player.GetComponent<Collider2D>())) // The collider belongs to the player
        {
            // Damages the player
            player.GetComponent<PlayerScript>().takeDamage(damage);

            // Knockback
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Vector2 kb;
            if (isFacingLeft)
            {
                kb = new Vector2(-2f, 2f).normalized;
            }
            else
            {
                kb = new Vector2(2f, 2f).normalized;
            }
            kb *= playerKnockback;
            rb.velocity = kb;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag=="Player")
        {
            // Knockback
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Vector2 kb;
            if (this.transform.position.x - player.transform.position.x > 0f)
            {
                kb = new Vector2(-1.5f, 1.5f).normalized;
            }
            else
            {
                kb = new Vector2(1.5f, 1.5f).normalized;
            }
            kb *= playerKnockback;
            rb.velocity = kb;
        }
    }

    /// <summary>
    /// Draws the enemy's attack radius
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draws the enemy's attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }
}
