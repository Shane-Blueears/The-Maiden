using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    //Public Variables
	public GameObject spawnPoint;
    public Rigidbody2D rb;
    public LayerMask mask;
    private Scoreboard score;

    public Slider healthBar;
    public int startingHealth = 100;

    public int numOfJumps;

    [Header("Movment Properties")]
    public float maxSpeed = 10f;
    public float acceleration = 2f;
    [Range(0f, 1f)]
    public float frictionCoefficient = 0.85f;
    public float massCoeficcient = 0.85f;
    public Vector3 movement;
    public float speed = 10f;

    public float dash = 10f;

    [Header("Jump Properties")]
    [Range(0f, 1f)]
    public float airControl = 0.85f;
    public float jumpHeight = 5f;
    public float gravityMod = 2.5f;
    public float maxVelocity = 20f;
    private Vector3 _characterSpeed = Vector3.zero;

    public bool onGround;
    public bool stopDash;
    public bool isLeftDirection;

    //For Animation
    public Animator animator;

    //Events for SFX
    public UnityEvent TakesDamage;
    public UnityEvent PlayerDash;
    public UnityEvent PlayerDeath;
    public UnityEvent PlayerJump;

    //Private Variables
    private int currentHealth;
    private Vector3 force = Vector3.zero;
    private static Timer timer;
    private IEnumerator coRoutine;
    private Vector3 jumpDirection;
    private int limitDashReset = 0;

    private Vector3 lastVelocity;

    // Start is called before the first frame update
    void Start()
	{
		currentHealth = startingHealth;
        numOfJumps = 0;
        isLeftDirection = true;
        onGround = true;
        stopDash = false;
        score = GetComponent<Scoreboard>();
	}

	//Update is called once per frame
	void Update()
	{

        animator.SetBool("IsGrounded", onGround);

        //Animation
        animator.SetFloat("Speed", Mathf.Abs(movement.x));

        //movement setup
        movement = Vector3.zero;
        movement.x = Input.GetAxisRaw("Horizontal") * 2f;
        //jump setup
        if (Input.GetKeyDown(KeyCode.Space)&&onGround)
		{
            Jump();
		}
        //Check if the player jesty felkl of a ledge
        if (this.GetComponent<Rigidbody2D>().velocity.y < -0.1)
        {
            onGround = false;
        }
        else
        {
            onGround = true;
        }

        //Check if the player is on the ground
        RaycastHit2D hitGround = Physics2D.Raycast(this.transform.position, -Vector2.up, 3f, mask);
        if(hitGround.collider != null )
        { 
            // Added "&& numOfJumps > 2" to allow single jumps. All works well but every once in a blue moon you can do 1 too many jumps ~Preston
            if (hitGround.collider.CompareTag("Ground") && numOfJumps > 1)
            {
                resetMechanics();
                numOfJumps++;
                _characterSpeed.y = 0;
                if (stopDash && limitDashReset==0)
                {
                    limitDashReset++;
                    Invoke("resetDash", 1.2f);
                }
            }
        }

        //Limit the number of jumps to 2
        //Changed to 1 for the forest level ~Preston
        if (numOfJumps >= 1)
        {
            onGround = false;
        }

        //Apply gravity to player, otherwise player becomes floaty
        //What should I do if the player just falls of a ledge WITHOUT jumping?
        //How to let player movement continue iafter hitting the floor-
        if(!onGround)
        {
            if (_characterSpeed.y> -maxVelocity)
            {
                _characterSpeed.y += gravityMod * Time.deltaTime * Physics.gravity.y;
            }
            //movement.x *= airControl;
        }
        else
        {
            _characterSpeed = Vector3.zero;
        }

        //Dash setup
        //Check which direction the player is looking at first
        if (Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isLeftDirection = true;
            this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
        else if (Input.GetKeyDown(KeyCode.D)|| Input.GetKeyDown(KeyCode.RightArrow))
        {
            isLeftDirection = false;
            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
        //Ignore gravity for 0.5 seconds & dash
		if(movement.x!=0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !stopDash)
            {
                this.GetComponent<Rigidbody2D>().gravityScale = 0f;
                coRoutine = WaitAndChange(0.5f);
                StartCoroutine(coRoutine);
                if (isLeftDirection)
                {
                    movement.x = -dash;
                    movement.y = 0;
                }
                else
                {
                    movement.x = dash;
                    movement.y = 0;
                }
                //Event for dash SFX
                PlayerDash.Invoke();
                stopDash = true;
                GetComponent<PlayerAttackScript>().dash();
            }
        }
        //Move the player
        movement *= massCoeficcient;
        movement = movement + _characterSpeed;
        movement.x *= frictionCoefficient;
        //movement.x += gravityPull;
        this.GetComponent<Rigidbody2D>().AddForce(movement * speed);

        

    }

    public void FixedUpdate()
    {
        lastVelocity = rb.velocity;

        //print(rb.velocity.x);
    }

    private void Jump()
    {
        numOfJumps++;
        animator.SetBool("IsJumping", true);
        _characterSpeed.y = 0;
        movement.y = 5 * jumpHeight;
        //Event for jump SFX
        PlayerJump.Invoke();
    }
    /// <summary>
    /// A method that is called 2 seconds after the player clicks shift to reset enable gravity again
    /// </summary>
    private IEnumerator WaitAndChange(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        this.GetComponent<Rigidbody2D>().gravityScale = 2;
    }

    /// <summary>
    /// If the player is grounded, meaning she is on the floor, she can jump twice and dash once.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            animator.SetBool("IsJumping", false);
            //Have an if check to see if the other collision is an enemy
            for (int i = 0; i < collision.contactCount; i++)
            {
                if ((collision.GetContact(i).point.y < this.transform.localPosition.y-1.8))//this.transform.position.y-0.59)
                {
                    resetMechanics();
                    resetDash();
                    
                }
            }
            numOfJumps = 0;
        }
        //Else statement

    }

    public void resetMechanics()
    {
        onGround = true;
        numOfJumps = 0;
        this.GetComponent<Rigidbody2D>().gravityScale = 2f;
    }
    public void resetDash()
    {
        stopDash = false;
        limitDashReset = 0;
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
        healthBar.value = currentHealth;
        //Event for damage SFX
        TakesDamage.Invoke();
		// If the player has 0 health kill them
		if (currentHealth <= 0)
		{
			killPlayer();
		}
        //Subtract the player health when damaged
        score.damageTaken();
	}

	/// <summary>
	/// Kills the player when they run out of health.
	/// </summary>
	private void killPlayer()
	{
        // Respawn the player at the spawnpoint
        // this.transform.position = spawnPoint.transform.position;

        //Event for death SFX
        PlayerDeath.Invoke();
        System.Threading.Thread.Sleep(1000);

        // Restarts the level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
