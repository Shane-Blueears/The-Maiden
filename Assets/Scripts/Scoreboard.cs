using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    private int health;
    public Text scoreScript;

    private float initialTime;
    private float timeElapsed;
    private float threshold;
    private int score;
    private int opponentKilled = 0;
    //DIsplay health remaining
    //Display Enemies killed
    //Display Time remaining
    //Display final score

    // Start is called before the first frame update
    void Start()
    {
        initialTime = Time.time;
        score = 1000;
        InvokeRepeating("CalcScore", 60, 60);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        scoreScript.text = "Score: " + score;
    }
    public void CalcScore()
    {
        timeElapsed = Time.time - initialTime;
        int minutes = (int)timeElapsed / 60;
        switch(minutes)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                score -= 0;
                break;
            case 4:
            case 5:
            case 6:
                score -= 250;
                break;
            case 7:
            case 8:
            case 9:
                score -= 500;
                break;
            default:
                score -= 750;
                break;
        }
    }
    public void enemyKilled()
    {
        score += 100;
    }
    public void damageTaken()
    {
        score -= 50;
    }
}
