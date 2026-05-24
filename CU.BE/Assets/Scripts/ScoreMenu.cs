using TMPro;
using UnityEngine;

/// <summary>
/// Controls the behaviour of the ScoreMenu.
/// </summary>
public class ScoreMenu : MonoBehaviour
{
    public TextMeshProUGUI Score;

    // Start is called before the first frame update
    void Start()
    {
        // Reset score on start...
        GameState.Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the score text.
        Score.text = GameState.Score.ToString();
    }
}
