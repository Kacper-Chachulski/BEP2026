using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Platinio.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles logic of the questionnaire scene
/// </summary>
public class Questionnaire : MonoBehaviour
{
    public CanvasGroup choice1;
    public CanvasGroup choice2;
    public CanvasGroup endQScreen;
    TMP_Text choice1Text;
    TMP_Text choice2Text;
    Button choice1Button;
    Button choice2Button;

    float startTime;
    bool gameStarted;

    public CanvasGroup instruction;

    public float fadeDuration = 0.5f;
    bool toInteract;

    //hardcoded questionnaire
    List<string> choices = new List<string> {"Male Gender", "Female Gender", "Male Gender", "Fluid Gender", "Male Gender", "Young Age", "Male Gender", "Adult Age", "Male Gender", "Old Age", "Male Gender", "Short", "Male Gender", "Average Height", "Male Gender", "Tall", "Male Gender", "Thin", "Male Gender", "Average Weight", "Male Gender", "Overweight", "Male Gender", "Fair Skin", "Male Gender", "Brown Skin", "Male Gender", "Dark Skin",
                                            "Female Gender", "Fluid Gender", "Female Gender", "Young Age", "Female Gender", "Adult Age", "Female Gender", "Old Age", "Female Gender", "Short", "Female Gender", "Average Height", "Female Gender", "Tall", "Female Gender", "Thin", "Female Gender", "Average Weight", "Female Gender", "Overweight", "Female Gender", "Fair Skin", "Female Gender", "Brown Skin", "Female Gender", "Dark Skin",
                                            "Fluid Gender", "Young Age", "Fluid Gender", "Adult Age", "Fluid Gender", "Old Age", "Fluid Gender", "Short", "Fluid Gender", "Average Height", "Fluid Gender", "Tall", "Fluid Gender", "Thin", "Fluid Gender", "Average Weight", "Fluid Gender", "Overweight", "Fluid Gender", "Fair Skin", "Fluid Gender", "Brown Skin", "Fluid Gender", "Dark Skin",
                                            "Young Age", "Adult Age", "Young Age", "Old Age", "Young Age", "Short", "Young Age", "Average Height", "Young Age", "Tall", "Young Age", "Thin", "Young Age", "Average Weight", "Young Age", "Overweight", "Young Age", "Fair Skin", "Young Age", "Brown Skin", "Young Age", "Dark Skin",
                                            "Adult Age", "Old Age", "Adult Age", "Short", "Adult Age", "Average Height", "Adult Age", "Tall", "Adult Age", "Thin", "Adult Age", "Average Weight", "Adult Age", "Overweight", "Adult Age", "Fair Skin", "Adult Age", "Brown Skin", "Adult Age", "Dark Skin",
                                             "Old Age", "Short", "Old Age", "Average Height", "Old Age", "Tall", "Old Age", "Thin", "Old Age", "Average Weight", "Old Age", "Overweight", "Old Age", "Fair Skin", "Old Age", "Brown Skin", "Old Age", "Dark Skin",
                                              "Short", "Average Height", "Short", "Tall", "Short", "Thin", "Short", "Average Weight", "Short", "Overweight", "Short", "Fair Skin", "Short", "Brown Skin", "Short", "Dark Skin",
                                              "Average Height", "Tall", "Average Height", "Thin", "Average Height", "Average Weight", "Average Height", "Overweight", "Average Height", "Fair Skin", "Average Height", "Brown Skin", "Average Height", "Dark Skin",
                                              "Tall", "Thin", "Tall", "Average Weight", "Tall", "Overweight", "Tall", "Fair Skin", "Tall", "Brown Skin", "Tall", "Dark Skin",
                                              "Thin", "Average Weight", "Thin", "Overweight", "Thin", "Fair Skin", "Thin", "Brown Skin", "Thin", "Dark Skin",
                                               "Average Weight", "Overweight", "Average Weight", "Fair Skin", "Average Weight", "Brown Skin", "Average Weight", "Dark Skin",
                                                "Overweight", "Fair Skin", "Overweight", "Brown Skin", "Overweight", "Dark Skin",
                                                 "Fair Skin", "Brown Skin", "Fair Skin", "Dark Skin",
                                                  "Brown Skin", "Dark Skin",

    };
    int loopChoices;
    int randomOrder;

    //weight matrix
    IDictionary<string,int> featureWeights = new Dictionary<string, int> {
        {"Male Gender", 0},
        {"Female Gender", 0},
        {"Fluid Gender", 0},

        {"Young Age", 0},
        {"Adult Age", 0},
        {"Old Age", 0},

        {"Short", 0},
        {"Average Height", 0},
        {"Tall", 0},

        {"Thin", 0},
        {"Average Weight", 0},
        {"Overweight", 0},

        {"Fair Skin", 0},
        {"Brown Skin", 0},
        {"Dark Skin", 0},
    };


    void Start()
    {

        GameState.Segment = 0;

        endQScreen.alpha = 0;
        endQScreen.gameObject.SetActive(false);
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);
        
        choice1Button = choice1.GetComponent<Button>();
        choice2Button = choice2.GetComponent<Button>();
        choice1Text = choice1.GetComponentInChildren<TMP_Text>();
        choice2Text = choice2.GetComponentInChildren<TMP_Text>();
    }

    public void Continue()
    {
        SceneManager.LoadScene("GameScene");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)&& gameStarted)
        {
            //log button press
            GameState.QuestionnaireLiveInput("Left", choice1Text.text, choice2Text.text,Time.time-startTime);
            //reset start time
            startTime = Time.time;
            choice1Button.onClick.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.D)&& gameStarted)
        {
            //log button press
            GameState.QuestionnaireLiveInput("Right", choice1Text.text, choice2Text.text, Time.time - startTime);
            //reset start time
            startTime = Time.time;
            choice2Button.onClick.Invoke();
        }
    }
    //start questionnaire after player understood instructions
    public void Understood()
    {
        instruction.gameObject.SetActive(false);
        choice1.alpha = 0;
        choice2.alpha = 0;

        choice1.gameObject.SetActive(true);
        choice2.gameObject.SetActive(true);
        choice1.FadeIn(fadeDuration);
        choice2.FadeIn(fadeDuration);

        startTime = Time.time;
        StartCoroutine(NextQuestion());
    }

    //show the next two choices
    IEnumerator NextQuestion()
    {
        yield return null;

        loopChoices = Random.Range(0, choices.Count);
        if (loopChoices % 2 == 1)
        {
            loopChoices -= 1;
        }
        //randomize whether choice is left or right
        randomOrder = Random.Range(0, 2);
        
        if (randomOrder == 0)
        {
            choice1Text.text = choices[loopChoices];
            choice2Text.text = choices[loopChoices + 1];
            choices.RemoveAt(loopChoices);
            choices.RemoveAt(loopChoices);
        }
        else
        {
            choice1Text.text = choices[loopChoices+1];
            choice2Text.text = choices[loopChoices];
            choices.RemoveAt(loopChoices);
            choices.RemoveAt(loopChoices);
        }
        gameStarted = true;
        toInteract = true;
    }
    //handles logic when questionnaire ends
    IEnumerator EndQuestionnaire()
    {
        gameStarted = false;
        choice1.FadeOut(fadeDuration);
        choice2.FadeOut(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);
        UpdateFeatureLists();
        endQScreen.alpha = 0;
        endQScreen.gameObject.SetActive(true);
        endQScreen.FadeIn(fadeDuration);
        //log questionnaire answers summary
        GameState.PostQuestResults(featureWeights);
        yield return null;

    }
    //apply questionnaire results
    void UpdateFeatureLists()
    {
        GameState.GenderPreference[0] = (Gender.Male, featureWeights["Male Gender"]);
        GameState.GenderPreference[1] = (Gender.Female, featureWeights["Female Gender"]);
        GameState.GenderPreference[2] = (Gender.NonBinary, featureWeights["Fluid Gender"]);

        GameState.AgePreference[0] = (Age.Young, featureWeights["Young Age"]);
        GameState.AgePreference[1] = (Age.Adult, featureWeights["Adult Age"]);
        GameState.AgePreference[2] = (Age.Old, featureWeights["Old Age"]);

        GameState.WeightPreference[0] = (Weight.Fit, featureWeights["Thin"]);
        GameState.WeightPreference[1] = (Weight.Average, featureWeights["Average Weight"]);
        GameState.WeightPreference[2] = (Weight.Large, featureWeights["Overweight"]);

        GameState.HeightPreference[0] = (Height.Short, featureWeights["Short"]);
        GameState.HeightPreference[1] = (Height.Average, featureWeights["Average Height"]);
        GameState.HeightPreference[2] = (Height.Tall, featureWeights["Tall"]);

        GameState.SkinTonePreference[0] = (SkinTone.Fair, featureWeights["Fair Skin"]);
        GameState.SkinTonePreference[1] = (SkinTone.Brown, featureWeights["Brown Skin"]);
        GameState.SkinTonePreference[2] = (SkinTone.Dark, featureWeights["Dark Skin"]);

        //attributes order by preference
        GameState.GenderPreference = GameState.GenderPreference.OrderBy(weight => weight.preference).ToArray();
        GameState.AgePreference = GameState.AgePreference.OrderBy(weight => weight.preference).ToArray();
        GameState.WeightPreference = GameState.WeightPreference.OrderBy(weight => weight.preference).ToArray();
        GameState.HeightPreference = GameState.HeightPreference.OrderBy(weight => weight.preference).ToArray();
        GameState.SkinTonePreference = GameState.SkinTonePreference.OrderBy(weight => weight.preference).ToArray();
        //load condition
        StartCoroutine(GameState.LoadCondition());

    }
    //stores choice depending on which button is true
    public void MakeChoice(Button button)
    {
        if(toInteract)
        {
            toInteract = false;

            if (button.gameObject.CompareTag("Choice1"))
            {
                featureWeights[choice1Text.text] += 1;
            }
            else
            {
                featureWeights[choice2Text.text] += 1;
            }
            if (choices.Count != 0)
            {
                StartCoroutine(NextQuestion());
            }
            else
            {
                StartCoroutine(EndQuestionnaire());
            }
        }    

    }
}
