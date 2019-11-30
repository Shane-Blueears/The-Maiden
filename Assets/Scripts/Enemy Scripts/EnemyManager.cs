using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    // If collisions should be ignored when enemies collide into each other
    public bool ignoreEnemyCollisions = false;

    public UnityEvent TakesDamage;

    private static EnemyManager instance;


    private List<GameObject> livingEnemies = new List<GameObject>();

    public static EnemyManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void damageEnemy(GameObject enemy, int damage)
    {
        //Event for Damage SFX
        TakesDamage.Invoke();
        enemy.SendMessage("takeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    public void registerEnemy(GameObject enemy)
    {
        livingEnemies.Add(enemy);

        // Allows the enemies to walk through each other
        if (ignoreEnemyCollisions)
        {
            foreach (GameObject e in livingEnemies)
            {
                Physics2D.IgnoreCollision(enemy.GetComponent<Collider2D>(), e.GetComponent<Collider2D>(), true);
            }
        }
    }

    /// <summary>
    /// Removes the enemy from the list of living enemies.
    /// </summary>
    public void removeEnemy(GameObject enemy)
    {
        livingEnemies.Remove(enemy);
    }
}
