using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

	public GameObject spawnPoint;

	public int startingHealth = 100;

	private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {

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
        // Decreases the player's damage
		currentHealth -= damage;

        // If the player has 0 health kill them
		if (currentHealth <= 0)
		{
			killPlayer();
		}
	}

    /// <summary>
    /// Kills the player when they run out of health.
    /// </summary>
	private void killPlayer()
	{
        // Respawn the player at the spawnpoint
        this.transform.position = spawnPoint.transform.position;
	}
}
