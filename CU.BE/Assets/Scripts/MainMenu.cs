using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.WebRequestMethods;
using static UnityEngine.Rendering.DebugUI;
using Button = UnityEngine.UI.Button;

/// <summary>
/// Controls the behaviour of the main menu screen.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public Button PlayButton;

    struct AttributeByWeight
    {
        public string name; // Name of the attribute.
        public int weight; // Attribute weight.
    }

    void InitGame(string parameters)
    {
        string[] arguments = parameters.Split(';', '=');
        ParseArguments(arguments);
    }

    /// <summary>
    /// Parse the arguments passed in the array in order to configure the attribute preferences.
    /// </summary>
    /// <param name="arguments">An array of name/value pairs that represent the attribute and weight (preference).</param>
    void ParseArguments(string[] arguments)
    {
        List<AttributeByWeight> attributes = new List<AttributeByWeight>();

        // Parse the attributes
        for (int i = 0; i < arguments.Length; i += 2)
        {
            try
            {
                Debug.Log($"{arguments[i]}={arguments[i + 1]}");

                switch (arguments[i].ToLower())
                {
                    case "pid":
                        GameState.PID = int.Parse(arguments[i + 1]);
                        break;
                    case "name":
                        GameState.Name = HttpUtility.UrlDecode(arguments[i + 1]);
                        break;
                    default:
                        AttributeByWeight attrib = new()
                        {
                            name = arguments[i],
                            weight = int.Parse(arguments[i + 1])
                        };
                        attributes.Add(attrib);
                        break;
                }
            }
            catch (FormatException)
            {
                Debug.LogError($"Unexpected argument: {arguments[i]} = {arguments[i + 1]}");
            }
        }

        // Sort attributes by weight
        var attributeByWeights = attributes.OrderByDescending(a => a.weight);

        // Assign attributes based on the order they appear in the sorted list.
        // Undesirable attributes will be set last and overwrite any previously set values.
        foreach (AttributeByWeight attrib in attributeByWeights)
        {
            // Attribute names should have the form "Type_Value".
            // For example:
            // Gender_Male, Gender_Female, Gender_NonBinary,
            // Weight_Fit, Weight_Average, Weight_Large,
            // Age_Young, Age_Adult, Age_Old,
            // Skin_Fair, Skin_Brown, Skin_Dark
            // Height_Short, Height_Average, Height_Tall
            var groups = Regex.Match(attrib.name, @"(\w+)_(\w+)").Groups; // Split the name by Type/Value
            var type = groups[1].Value;
            var value = groups[2].Value;

            Debug.Log($"{type}_{value}: {attrib.weight}");

            switch (type.ToLower())
            {
                case "gender":
                    switch (value.ToLower())
                    {
                        case "male":
                            GameState.AddGender(Gender.Male, attrib.weight);
                            break;
                        case "female":
                            GameState.AddGender(Gender.Female, attrib.weight);
                            break;
                        case "nonbinary":
                            GameState.AddGender(Gender.NonBinary, attrib.weight);
                            break;
                    }
                    break;
                case "weight":
                    switch (value.ToLower())
                    {
                        case "fit":
                            GameState.AddWeight(Weight.Fit, attrib.weight);
                            break;
                        case "average":
                            GameState.AddWeight(Weight.Average, attrib.weight);
                            break;
                        case "large":
                            GameState.AddWeight(Weight.Large, attrib.weight);
                            break;
                    }
                    break;
                case "age":
                    switch (value.ToLower())
                    {
                        case "young":
                            GameState.AddAge(Age.Young, attrib.weight);
                            break;
                        case "adult":
                            GameState.AddAge(Age.Adult, attrib.weight);
                            break;
                        case "old":
                            GameState.AddAge(Age.Old, attrib.weight);
                            break;
                    }
                    break;
                case "skin":
                    switch (value.ToLower())
                    {
                        case "fair":
                            GameState.AddSkinTone(SkinTone.Fair, attrib.weight);
                            break;
                        case "brown":
                            GameState.AddSkinTone(SkinTone.Brown, attrib.weight);
                            break;
                        case "dark":
                            GameState.AddSkinTone(SkinTone.Dark, attrib.weight);
                            break;
                    }
                    break;
                case "height":
                    switch (value.ToLower())
                    {
                        case "short":
                            GameState.AddHeight(Height.Short, attrib.weight);
                            break;
                        case "average":
                            GameState.AddHeight(Height.Average, attrib.weight);
                            break;
                        case "tall":
                            GameState.AddHeight(Height.Tall, attrib.weight);
                            break;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Parse the URL parameters and configure the Avatar accordingly.
    /// </summary>
    void ParseURLParameters()
    {
#if UNITY_WEBGL
        try
        {
#if UNITY_EDITOR
            string url = "http://localhost:8000/?PID=1&Name=Jeremiah%20van%20Oosten&Gender_Male=1&Gender_Female=2&Gender_NonBinary=3&Weight_Fit=4&Weight_Average=5&Weight_Large=6&Age_Young=7&Age_Adult=8&Age_Old=9&Skin_Fair=10&Skin_Brown=11&Skin_Dark=12&Height_Short=13&Height_Average=14&Height_Tall=15";
#else
            string url = Application.absoluteURL;
#endif
            string parameters = url.Substring(url.LastIndexOf("?", StringComparison.Ordinal) + 1);
            string[] arguments = parameters.Split('&', '=');

            ParseArguments(arguments);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
#endif

    }

    public void Play_OnClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    void Update()
    {
        // Allow the player to start the game by pressing "Enter"
        // on the Keyboard.
        if (PlayButton.interactable && Input.GetKeyUp(KeyCode.Return))
        {
            PlayButton.onClick.Invoke();
        }
    }
}
