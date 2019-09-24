using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

	public GameObject spawnPoint;

	public int startingHealth = 100;

    public float speed = 10f;

    public bool isLeftDirection;

    private int currentHealth;

	// Start is called before the first frame update
	void Start()
	{
		currentHealth = startingHealth;
        isLeftDirection = true;
	}

	//Update is called once per frame
	void Update()
	{
		//movement setup
		Vector3 movement = new Vector3();
		movement.x = Input.GetAxis("Horizontal");

		//jump setup
		if (Input.GetKeyDown(KeyCode.Space))
		{
			movement.y = 5 * speed;
		}

        //Dash setup
        //Check which direction the player is looking at first
        if (Input.GetKeyDown(KeyCode.A))
        {
            print("Hit A");
            isLeftDirection = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            isLeftDirection = false;
        }

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
            print(isLeftDirection);
            if (isLeftDirection)
            {
                print("Hit");
                movement.x = -10 * speed;
            }
            else
            {
                movement.x = 10 * speed;
            }
		}
		//Move the player
		this.GetComponent<Rigidbody>().AddForce(movement * speed);
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
