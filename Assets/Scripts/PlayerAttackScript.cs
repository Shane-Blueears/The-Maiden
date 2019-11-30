using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAttackScript : MonoBehaviour
{

    public Transform jabAttackPos;
    public float jabAttackRadius;
    public Transform heavyAttackPos;
    public float heavyAttackRadius;
    public LayerMask whatIsEnemies;

    public int jabDamage;
    public int heavyDamage;
    
    // The amount of time the palyer is required to wait between attacks (in seconds)
    public float timeBetweenJabAttacks = 0.5f;
    public float timeBetweenHeavyAttacks = 1f;
    private float timeBetweenAttack;

    // The length of the attack animations (in seconds)
    public float jabAttackAnimationLength = 0.28f;
    public float heavyAttackAnimationLength = 0.35f;
    // The amount of time left in the current attack animation 
    private float jabAttackTime;
    private float heavyAttackTime;
    // The enemies that were already damaged in the current attack
    private List<Collider2D> attacked = new List<Collider2D>();

    public Animator playerAnimator;

    public Animator animator;

    public GameObject weapon;

    private PlayerScript playerScript;

    // Events for SFX
    public UnityEvent HalberdSwing;
    public UnityEvent HalberdJab;

    float dashTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timeBetweenAttack = 0f;
        playerScript = GetComponent<PlayerScript>();

        jabAttackTime = 0f;
        heavyAttackTime = 0f;

        weapon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        dashTimer -= Time.deltaTime;
        if (dashTimer > 0f)
        {
            return;
        }

        // If the player is currently attacking search for enemies in attack range
        if (jabAttackTime > 0) // Jab attack
        {
            jabAttackTime -= Time.deltaTime;


            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(jabAttackPos.position, jabAttackRadius, whatIsEnemies);
            foreach (Collider2D enemy in enemiesToDamage)
            {
                // Damage the enemy if they haven't already been damaged
                if (!attacked.Contains(enemy))
                {
                    // Fix for new system!!!!!
                    // enemy.GetComponent<EnemyScript>().takeDamage(jabDamage);
                    EnemyManager.Instance.damageEnemy(enemy.gameObject, jabDamage);
                    attacked.Add(enemy);
                }
            }
        }
        else if (heavyAttackTime > 0) // Heavy attack
        {
            heavyAttackTime -= Time.deltaTime;


            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(heavyAttackPos.position, heavyAttackRadius, whatIsEnemies);
            foreach (Collider2D enemy in enemiesToDamage)
            {
                // Damage the enemy if they haven't already been damaged
                if (!attacked.Contains(enemy))
                {
                    // enemy.GetComponent<EnemyScript>().takeDamage(heavyDamage);
                    EnemyManager.Instance.damageEnemy(enemy.gameObject, heavyDamage);
                    attacked.Add(enemy);
                }
            }
        }

        // If the attack refresh rate has been reached
        if (timeBetweenAttack <= 0)
        {
            if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Mouse0)) // Jab attack
            {
                // Show sword
                weapon.SetActive(true);
                Invoke("hideWeapon", jabAttackAnimationLength);

                // Creates a cooldown before the player can attack again
                timeBetweenAttack = timeBetweenJabAttacks;

                // Plays the jab animation
                animator.SetTrigger("JabAnimation");
                playerAnimator.SetBool("IsJab", true);
                playerAnimator.SetBool("IsJumping", false);
                playerAnimator.SetTrigger("Jab");

                //Event for jab SFX
                HalberdJab.Invoke();

                // Starts searching for enemies within the player's attack range
                jabAttackTime = jabAttackAnimationLength;
                attacked.Clear();

                //jabbing animation (Dylan)
               // animator.SetBool("isJabbing", true);
            }
            else if (Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Mouse1))
            {
                // Show sword
                weapon.SetActive(true);
                Invoke("hideWeapon", heavyAttackAnimationLength);

                // Creates a cooldown before the player can attack again
                timeBetweenAttack = timeBetweenHeavyAttacks;

                // Plays the heavy animation
                animator.SetTrigger("HeavyAnimation");
                playerAnimator.SetBool("IsHeavy", true);
                playerAnimator.SetBool("IsJumping", false);
                playerAnimator.SetTrigger("Heavy");

                //Event for swing SFX
                HalberdSwing.Invoke();

                // Starts searching for enemies within the player's attack range
                heavyAttackTime = heavyAttackAnimationLength;
                attacked.Clear();
            }
        }
        else
        {
            timeBetweenAttack -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Prevents the player from attacking for .5 second after dashing
    /// </summary>
    public void dash()
    {
        dashTimer = 0.5f;
    }

    private void hideWeapon()
    {
        // Hide weapon
        weapon.SetActive(false);
    }

    /// <summary>
    /// Draws the attack radius for all of the player's attacks
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draws the jab attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(jabAttackPos.position, jabAttackRadius);

        // Draws the heavy attack radius
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(heavyAttackPos.position, heavyAttackRadius);
    }

}
