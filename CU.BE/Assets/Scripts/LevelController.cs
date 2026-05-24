using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

/// <summary>
/// Controls logic level ending and general level behaviour
/// </summary>
public class LevelController : MonoBehaviour
{
    //playing time per round in seconds
    int playTime = 20;
    public Canvas Canvas;
    GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 40;
        Player = GameObject.Find("Player");
        //Reset segment variables every segment
        GameState.BarrierHits = 0;
        GameState.Score = 0;
        GameState.LaneChanges = 0;
        GameState.GameEnded = false;
        GameState.FistTime = false;
        //log avatar at start of game scene
        GameState.PostAvatar();
        Canvas.enabled = false;
        // Run timed game for all segments (20s configured)
        StartCoroutine(GameDuration());

    }
    //end game when the player reached a certain distance
    public IEnumerator GameDuration()
    {
        // end game after a fixed amount of real seconds (playTime)
        float startRealTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startRealTime < playTime)
        {
            yield return null;
        }
        // record elapsed real time for the segment
        GameState.TimePlayed = Time.realtimeSinceStartup - startRealTime;
        EndSegment();
    }
    //handles logic ending the game
    void EndSegment()
    {
        // If GameDuration already set TimePlayed, keep it; otherwise fall back to level time.
        if (GameState.TimePlayed <= 0f)
        {
            GameState.TimePlayed = Time.timeSinceLevelLoad;
        }
        //post segment data after every segment
        GameState.PostInputSegment();
        // Pause game.
        GameState.GameEnded = true;
        Time.timeScale = 0;
        // Show the canvas.
        Canvas.enabled = true;
    }

    //handles logic continuebutton end of scene
    public void CustomizeAvatarButton_OnClick()
    {
        // If this was not the last game segment, advance to the next game segment.
        if (GameState.Segment < 2)
        {
            GameState.Segment += 1;
            // Skip avatar customization entirely and continue to the next game scene.
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            // Final segment finished — end the experiment and redirect to the next page.
            GameState.EndGame();
        }

    }
}
