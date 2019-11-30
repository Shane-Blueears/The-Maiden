using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishLineScript : MonoBehaviour
{

    public Text winText;
    public string winMessage = "Supreme Victor!";

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            completeLevel();
            other.gameObject.GetComponent<Rigidbody2D>().mass = 5000f;
        }
    }

    /// <summary>
    /// This will be called when the player has completed the level
    /// </summary>
    private void completeLevel()
    {
        Invoke("restartLevel", 5f);
        winText.text = winMessage;
    }

    private void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}