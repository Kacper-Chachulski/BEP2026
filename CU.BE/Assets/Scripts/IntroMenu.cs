using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the behaviour of the Intro menu screen that appears
/// when the game first starts.
/// </summary>
public class IntroMenu : MonoBehaviour
{
    public Canvas Canvas;
    public Button StartButton;
    public GameObject MovePanel;
    public GameObject RollingPanel;
    GameObject Player;
    void Awake()
    {
        Player = GameObject.Find("Player");
        GameState.GameStarted = false;
        //if its the first time, start the tutorial
        if (GameState.FistTime)
        {
            StartCoroutine(RollTutorial());
        }
        else
        {            Time.timeScale = 1;
            GameState.GameStarted = true;
        }
    }
    //handles tutorial logic
    IEnumerator RollTutorial()
    {
        yield return new WaitForSeconds(0.1f);
        //show moving forward tutorial
        MovePanel.SetActive(true);
        Time.timeScale = 0;
        // continue when W is pressed to learn the player to move
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                break;
            }
            yield return null;
        }
        MovePanel.SetActive(false);
        Time.timeScale = 1;
        while (Player.transform.position.z < 60)
        {
            yield return null; 
        }
        //show rolling tutorial
        RollingPanel.SetActive(true);
        Time.timeScale = 0;
        //continue when player presses space to roll
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                break;
            }
            yield return null;
        }
        Time.timeScale = 1;
        RollingPanel.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        // show tutorial changing lanes 
        Time.timeScale = 0;
        Canvas.enabled = true;
        //continue when player presses one of the buttons to change lanes
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                break;
            }
            yield return null;
        }
        Canvas.enabled = false;
        Time.timeScale = 1;
        GameState.GameStarted = true;
    }
}
