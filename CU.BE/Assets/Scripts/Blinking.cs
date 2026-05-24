using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Blinking mechanic for certain buttons.
/// </summary>
public class Blinking : MonoBehaviour
{
    public Image nButton;
    public Image mButton;

    //start blinking when button is enabled
    void OnEnable()
    {
        StartCoroutine(Blink());
    }
    //infinite loop blinking from yellow to white
    IEnumerator Blink()
    { 
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            nButton.color = Color.yellow;
            yield return new WaitForSecondsRealtime(0.5f);
            nButton.color = Color.white;
            yield return new WaitForSecondsRealtime(0.2f);
            mButton.color = Color.yellow;
            yield return new WaitForSecondsRealtime(0.5f);
            mButton.color = Color.white;
            yield return new WaitForSecondsRealtime(1.0f);
        }
    }
}
