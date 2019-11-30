using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Jump towards player > spin > dazed
 * Health bar is locked when the boss can't be damaged
 * 
 * 
 */
public class ForestBoss : MonoBehaviour
{

    public GameObject player;

    Animator animator;
    float timer = 0.2f;

    [Header("Combat")]
    public int startingHealth = 200;
    [Tooltip("The amount of damage applied when the player touches the enemy")]
    public int tapDamage = 10;
    [Tooltip("The amount of damage applied when the player is kicked by the enemy")]
    public int kickDamage = 20;
    [Tooltip("The amount of damage that must be dealt to the boss in order to knock them out of the dazed state")]
    public int damageToUndazeBoss = 10;

    public int currentHealth;
    int damageTakenDuringLoop = 0;

    [Header("Movement")]

    public float jumpBoost = 15f;
    public LayerMask groundMask;
    [Tooltip("The amount of knockback applied when the player touches the enemy")]
    public float tapKnockback = 10f;
    [Tooltip("The amount of knockback applied when the player is kicked by the enemy")]
    public float kickKnockback = 30f;

    bool isFacingLeft = true;
    Rigidbody2D rb;
    bool recentlyJumped;
    float spriteHeight;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = startingHealth;

        animator = GetComponent<Animator>();

        // Gets half the height of the sprite + 0.5 to extend past the height of the boss
        spriteHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2f + 0.5f;

        // Uncomment out if the player should be able to walk through the enemy
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), true);
        
        // Registers the enemy
        EnemyManager.Instance.registerEnemy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Faces the boss towards the player
        if (this.transform.position.x - player.transform.position.x > 0f)
        {
            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        {
            this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }

        GetComponent<BoxCollider2D>().enabled = animator.GetBool("isSpinning");

        // Wait 1 second before listening for the boss to hit the ground
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }

        if (recentlyJumped)
        {
            // Detects if the boss is touching the ground
            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.down, spriteHeight, groundMask);
            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                // Exit jumping state since the boss hit the ground
                animator.SetBool("isJumping", false);
                animator.SetBool("isInJumpingState", false);
                //Invoke("updateRecentlyJumped", 0.1f);
                recentlyJumped = false;
            }
        }
        else
        {
            // Jump if the boss is still in the jumping state and is not currently jumping
            if (animator.GetBool("isInJumpingState"))
            {
                recentlyJumped = true;

                float distance = player.transform.position.x - this.transform.position.x;
                rb.velocity = new Vector2(distance * (rb.gravityScale / 3f), jumpBoost);

                timer = 0.2f;
            }
        }
    }

    private void updateRecentlyJumped()
    {
        recentlyJumped = false;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // If the collider belongs to the player and the boss isn't dazed
        if (col.collider.Equals(player.GetComponent<Collider2D>()) && !animator.GetBool("isDazed"))
        {
            damagePlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // If the collider belongs to the player and the boss isn't dazed
        if (col.Equals(player.GetComponent<Collider2D>()) && !animator.GetBool("isDazed"))
        {
            damagePlayer();
        }
    }

    void damagePlayer()
    {
        // Knockback
        Rigidbody2D rbPlayer = player.GetComponent<Rigidbody2D>();
        Vector2 kb;
        if (this.transform.position.x - player.transform.position.x > 0f)
        {
            kb = new Vector2(-1f, 0.5f);
        }
        else
        {
            kb = new Vector2(1f, 0.5f);
        }
        if (GetComponent<Animator>().GetBool("spinAttack"))
        {
            kb *= kickKnockback;

            // Damages the player
            player.GetComponent<PlayerScript>().takeDamage(kickDamage);
        }
        else
        {
            kb *= tapKnockback;

            // Damages the player
            player.GetComponent<PlayerScript>().takeDamage(tapDamage);
        }
        rbPlayer.velocity = kb;
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
        // Boss should only take damage if they are dazed
        if (!animator.GetBool("isDazed"))
            return;

        // Should the boss leave the dazed state?
        damageTakenDuringLoop += damage;
        if (damageTakenDuringLoop >= damageToUndazeBoss)
        {
            animator.SetBool("isDazed", false);
            damageTakenDuringLoop = 0;
        }

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
}
