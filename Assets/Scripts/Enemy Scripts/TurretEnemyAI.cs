using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemyAI : MonoBehaviour
{
    [Tooltip("The position from which the projectiles will be launched from")]
    public Transform projectileSource;
    public GameObject projectilePrefab;
    public GameObject player;
    public Scoreboard score;

    public float viewDistance = 10f;
    public float cooldown = 1f;
    float timer = 0f;

    int currentHealth;
    public int startingHealth = 10;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;

        EnemyManager.Instance.registerEnemy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Cooldown
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }

        // Checks if the player is within attack range
        float distance = Vector2.Distance(this.transform.position, player.transform.position);
        if (distance <= viewDistance)
        {
            launchProjectile();
            timer = cooldown;
        }
    }

    /// <summary>
    /// Launches a projectile towards the player
    /// </summary>
    void launchProjectile()
    {
        GameObject proj = Instantiate(projectilePrefab, projectileSource.position, Quaternion .identity, this.transform);
        // Allows the player to walk through the enemy
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), proj.GetComponent<Collider2D>(), true);
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
    /// Draws the attack radius for all of the player's attacks
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draws the jab attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, viewDistance);
    }
}
