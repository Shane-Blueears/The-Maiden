using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemyAIScript : MonoBehaviour
{
    //public GameObject tempRollObj;
    //public GameObject tempIdleObj;

    [Header("Attack")]

    public int startingHealth = 10;
    public int currentHealth;
    public Scoreboard score;

    public int damage = 5;

    public float playerKnockback = 20f;

    public GameObject player;

    public float attackCooldown = 3f;
    private float currentAttackCooldown = 0f;

    public Animator spriteAnimator;

    // If the enemy is currently attacking
    private bool isAttacking = false;

    // If the enemy already hit the player in it's current attack
    public bool hitPlayer = false;

    [Header("Movement")]

    public Transform target;

    public float rollSpeed = 20f;
    
    public float walkSpeed = 2f;

    // The points that tracks whether or not the enemy is about to walk off the edge
    public Transform groundPt;
    public LayerMask tilemapLayer;
    public Collider2D tileMapCollider;

    public Transform lForwardPt;
    public Transform uForwardPt;

    public BoxCollider2D idleCollider;
    public CircleCollider2D rollCollider;

    // The length of the animation going to and from the rolling state
    public float toRollAnimationTime = 0.5f;
    public float toIdleAnimationTime = 0.5f;

    // The maximum distance that the enemy will roll after the player
    public float attackRange = 10f;

    public bool isFacingLeft = true;

    private bool isRolling = false;

    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        currentHealth = startingHealth;

        // The enemy starts with the attack area trigger disabled
        rollCollider.enabled = false;

        // Registers the enemy
        EnemyManager.Instance.registerEnemy(this.gameObject);

        // TEMPORARY FOR TESTING PURPOSES
        //tempIdleObj.SetActive(true);
        //tempRollObj.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // The distance away from the target
        float distance = Vector2.Distance(this.transform.position, target.position);

        // Attack cooldown
        if (currentAttackCooldown > 0f)
        {
            currentAttackCooldown -= Time.deltaTime;
            return;
        }

        // Attack if target is within attackDistance
        if (!isAttacking && distance <= attackRange)
        {
            beginAttack();
        }

        if (isRolling)
        {
            move();
        }
    }

    /// <summary>
    /// Puts this enemy into attack mode towards the player.
    /// </summary>
    private void beginAttack()
    {
        isAttacking = true;

        //rigidBody.isKinematic = false;

        // Faces the enemy towards the player
        // Moves the player on the x axis towards the target
        Vector2 xDir = new Vector2(target.position.x - this.transform.position.x, 0f).normalized;
        // Face the enemy in the direction they are moving
        this.transform.localScale = new Vector3(-xDir.x * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        // Updates isFacingLeft
        isFacingLeft = xDir.x < 0f;

        spriteAnimator.SetBool("isRolling", true);
        Invoke("attack", toRollAnimationTime);

        // Swaps the the rolling collider
        idleCollider.enabled = false;
        rollCollider.enabled = true;

        // TEMPORARY FOR TESTING PURPOSES
        //tempIdleObj.SetActive(false);
        //tempRollObj.SetActive(true);
    }

    /// <summary>
    /// Makes the enemy start to roll after the player.
    /// </summary>
    private void attack()
    {
        isRolling = true;
        hitPlayer = false;
    }

    /// <summary>
    /// Moves the enemy towards the target.
    /// </summary>
    private void move()
    {
        // Moves the player on the x axis towards the target
        Vector2 xDir;
        if (isFacingLeft)
        {
            xDir = new Vector2(-1f, 0f);
        }
        else
        {
            xDir = new Vector2(1f, 0f);
        }

        // Ensures the enemy won't walk off a cliff by shooting a ray of length 4(2 tiles) downwards
        RaycastHit2D groundInfo = Physics2D.Raycast(groundPt.position, Vector2.down, 4f, tilemapLayer);
        //Debug.DrawLine(groundPt.position, new Vector3(groundPt.position.x, groundPt.position.y - 4f, groundPt.position.z), Color.green, 0.1f);

        if (groundInfo.collider == false || groundInfo.collider != tileMapCollider)
        {
            Debug.Log("Lost ground");
            stopAttack();
            return;
        }

        Vector2 movePos;

        // Handle walking up/down blocks...
        RaycastHit2D lForwardPtInfo = Physics2D.Raycast(lForwardPt.position, Vector2.down, 0.2f, tilemapLayer);
        groundInfo = Physics2D.Raycast(groundPt.position, Vector2.down, 0.2f, tilemapLayer);
        if (lForwardPtInfo.collider == tileMapCollider) // If the enemy has hit a wall or incline
        {
            Debug.Log("lforward " + this.transform.position);
            RaycastHit2D uForwardPtInfo = Physics2D.Raycast(uForwardPt.position, Vector2.up, 1f, tilemapLayer);
            if (uForwardPtInfo.collider == tileMapCollider) // The enemy ran into a wall
            {
                stopAttack();
                return;
            }
            else // The enemy is walking up an incline
            {
                Debug.Log("incline " + this.transform.position);
                xDir += new Vector2(0f, 1f);
                movePos = xDir * rollSpeed * Time.deltaTime;
            }
        }
        else if (groundInfo.collider == false || groundInfo.collider != tileMapCollider)
        {
            movePos = (xDir * rollSpeed + Physics2D.gravity) * Time.deltaTime;
        }
        else
        {
            movePos = xDir * rollSpeed * Time.deltaTime;
        }

        // Move the enemy
        rigidBody.MovePosition(rigidBody.position + movePos);
    }

    private void stopAttack()
    {
        isRolling = false;
        isAttacking = false;

        //rigidBody.isKinematic = true;

        // Swaps to the idle collider
        rollCollider.enabled = false;
        idleCollider.enabled = true;

        spriteAnimator.SetBool("isRolling", false);
        currentAttackCooldown = attackCooldown;

        // Allows the player to walk through the enemy
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);

        hitPlayer = false;

        // TEMPORARY FOR TESTING PURPOSES
        //tempIdleObj.SetActive(true);
        //tempRollObj.SetActive(false);
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
        score.enemyKilled();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Something has been hit by the attack.
    /// </summary>
    void OnCollisionEnter2D(Collision2D col)
    {
        // If the enemy already hit the player with the current attack
        if (hitPlayer)
            return;

        if (!isAttacking)
        {
            // Damages the player
            player.GetComponent<PlayerScript>().takeDamage(damage);

            // Knockback
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Vector2 kb;
            if (player.GetComponent<PlayerScript>().isLeftDirection)
            {
                kb = new Vector2(2f, 2f).normalized;
            }
            else
            {
                kb = new Vector2(-2f, 2f).normalized;
            }
            kb *= playerKnockback;
            rb.velocity = kb;

            return;
        }

        if (col.collider.Equals(player.GetComponent<Collider2D>())) // The collider belongs to the player
        {
            hitPlayer = true;
            
            // Damages the player
            player.GetComponent<PlayerScript>().takeDamage(damage);

            // Knockback
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Vector2 kb;
            if (this.transform.position.x - player.transform.position.x > 0f)
            {
                kb = new Vector2(-1f, 0.8f).normalized;
            }
            else
            {
                kb = new Vector2(1f, 0.8f).normalized;
            }
            kb *= playerKnockback;
            rb.velocity = kb;

            // Allows the player to walk through the enemy
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), true);
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
