using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;
using System.Collections;

/// <summary>
/// This class adds a Shuffle function to Random.
/// Source: https://stackoverflow.com/a/110570/11242797
/// </summary>
static class ShuffleExtension
{
    public static void Shuffle<T>(this Random rng, T[] arr)
    {
        int n = arr.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (arr[n], arr[k]) = (arr[k], arr[n]);
        }
    }

    public static void Shuffle<T>(this Random rng, List<T> arr)
    {
        int n = arr.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (arr[n], arr[k]) = (arr[k], arr[n]);
        }
    }
}

/// <summary>
/// This script controls the Customize Avatar menu
/// </summary>
public class CustomizeAvatarMenu : MonoBehaviour
{
    [Tooltip("The number of times the user needs to press the key to get to the desirable attribute.")]
    public int IncrementCount = 100;

    public AvatarCustomization AvatarCustomization;
    public Slider AttributeSlider;

    //public TMP_Text AttributeText;
    public Image[] Arrows;

    public Image Level1Image;
    public Image Level2Image;
    public Image Level3Image;

    public Sprite Gender_Male;
    public Sprite Gender_Female;
    public Sprite Gender_NonBinary;

    public Sprite Weight_Fat;
    public Sprite Weight_Medium;
    public Sprite Weight_Thin;

    public Sprite Age_Young;
    public Sprite Age_Adult;
    public Sprite Age_Old;

    public Sprite Skin_Dark;
    public Sprite Skin_Light;
    public Sprite Skin_Medium;

    public Sprite Height_Short;
    public Sprite Height_Tall;
    public Sprite Height_Medium;

    private Attribute currentAttribute;

    // These are used to map the attribute values to the sprites.
    private Dictionary<Gender, Sprite> GenderSprites = new();
    private Dictionary<Weight, Sprite> WeightSprites = new();
    private Dictionary<Age, Sprite> AgeSprites = new();
    private Dictionary<SkinTone, Sprite> SkinSprites = new();
    private Dictionary<Height, Sprite> HeightSprites = new();

    public GameObject instructionPanel;
    public GameObject continueButton;
    Button continueB;
    Image continueIm;
    int attributesMetCriterium;
    Image rightKey;
    public TMP_Text[] attributeTexts;
    // these are the dictionaries used for logging, this can be rewritten shorter with a struct or class
    Dictionary<Attribute, bool> midPoint = new Dictionary<Attribute, bool>() {
        {Attribute.Gender, false},
        {Attribute.Weight, false},
        {Attribute.Age, false},
        {Attribute.SkinTone, false},
        {Attribute.Height, false}
    };
    Dictionary<Attribute, bool> finalPoint = new Dictionary<Attribute, bool>(){
        {Attribute.Gender, false},
        {Attribute.Weight, false},
        {Attribute.Age, false},
        {Attribute.SkinTone, false},
        {Attribute.Height, false}
    };
    Dictionary<Attribute, bool> checkedUnwanted = new Dictionary<Attribute, bool>(){
        {Attribute.Gender, false},
        {Attribute.Weight, false},
        {Attribute.Age, false},
        {Attribute.SkinTone, false},
        {Attribute.Height, false}
    };
    Dictionary<Attribute, float> timeSpent = new Dictionary<Attribute, float>(){
        {Attribute.Gender, 0},
        {Attribute.Weight, 0},
        {Attribute.Age, 0},
        {Attribute.SkinTone, 0},
        {Attribute.Height, 0}
    };
    Dictionary<Attribute, int> pressesMid = new Dictionary<Attribute, int>(){
        {Attribute.Gender, 0},
        {Attribute.Weight, 0},
        {Attribute.Age, 0},
        {Attribute.SkinTone, 0},
        {Attribute.Height, 0}
    };
    Dictionary<Attribute, int> pressesFinal = new Dictionary<Attribute, int>(){
        {Attribute.Gender, 0},
        {Attribute.Weight, 0},
        {Attribute.Age, 0},
        {Attribute.SkinTone, 0},
        {Attribute.Height, 0}
    };

    float startTime;
    bool instructing;
    public Toggle toggleNoChange;

    /// <summary>
    /// How close the slider has to be to the value before
    /// updating the attribute.
    /// </summary>
    public float SliderTolerance = 0.001f;

    // The list of available attributes.
    private Attribute[] attributes =
    {
        Attribute.Gender,
        Attribute.Weight,
        Attribute.Age,
        Attribute.SkinTone,
        Attribute.Height,
    };

    // The current attribute value.
    private int currentAttributeValue;
    // The current index of the attribute in the attributes array.
    private int currentAttributeIndex;

    private Random rng = new((int)DateTime.Now.Ticks & 0x0000FFFF);

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        GenderSprites[Gender.Female] = Gender_Female;
        GenderSprites[Gender.NonBinary] = Gender_NonBinary;
        GenderSprites[Gender.Male] = Gender_Male;

        WeightSprites[Weight.Fit] = Weight_Thin;
        WeightSprites[Weight.Average] = Weight_Medium;
        WeightSprites[Weight.Large] = Weight_Fat;

        AgeSprites[Age.Young] = Age_Young;
        AgeSprites[Age.Adult] = Age_Adult;
        AgeSprites[Age.Old] = Age_Old;

        SkinSprites[SkinTone.Fair] = Skin_Light;
        SkinSprites[SkinTone.Brown] = Skin_Medium;
        SkinSprites[SkinTone.Dark] = Skin_Dark;

        HeightSprites[Height.Short] = Height_Short;
        HeightSprites[Height.Average] = Height_Medium;
        HeightSprites[Height.Tall] = Height_Tall;
    }

    // Start is called before the first frame update
    void Start()
    {
        continueB = continueButton.GetComponent<Button>();
        continueIm = continueButton.GetComponent<Image>();

        rightKey = GameObject.Find("ArrowKey_Right").GetComponent<Image>();
        // Initially, hide all arrows.
        foreach (var arrow in Arrows)
        {
            arrow.enabled = false;
        }

        // Randomize the order of the attributes.
        rng.Shuffle(attributes);

        AvatarCustomization.Gender = GameState.GenderPreference[0].gender;
        AvatarCustomization.Weight = GameState.WeightPreference[0].weight;
        AvatarCustomization.Age = GameState.AgePreference[0].age;
        AvatarCustomization.SkinTone = GameState.SkinTonePreference[0].skinTone;
        AvatarCustomization.Height = GameState.HeightPreference[0].height;

        //show all attribute names in shuffled state
        int i = 0;
        foreach (var a in attributes)
        {
            //hardcoded so "skin tone" is visible instead of "skintone"
            switch (a)
            {
                case Attribute.SkinTone:
                    attributeTexts[i].text = "Skin Tone";
                    break;
                default:
                    attributeTexts[i].text = a.ToString();
                    break;
            }
            i += 1;
        }

        // Set the initial attribute.
        SetAttribute(attributes[currentAttributeIndex], currentAttributeIndex);
        currentAttributeValue = GetValue(AttributeSlider.value);

        Time.timeScale = 0;

    }

    void Update()
    {
        //criterium is met if the attribute's checkmark selected to not change the attribute or the indicator is moved at least once
        attributesMetCriterium = 0;
        int i = 0;
        //improvement would be to make this event based instead of running every frame
        //if attribute meets criterium to pass text is white, else gray
        foreach (var a in attributes)
        {
            if (pressesMid[a] > 0 || checkedUnwanted[a])
            {
                attributeTexts[i].color = Color.white;
                attributesMetCriterium += 1;
            }
            else
            {
                attributeTexts[i].color = Color.gray;
            }
            i += 1;
        }
        //if criterium is met for every single attribute, player may continue to game
        if (attributesMetCriterium == attributes.Length)
        {
            continueIm.color = Color.white;
            continueB.interactable = true;
        }
        else
        {
            continueIm.color = Color.gray;
            continueB.interactable = false;

        }

        float increment = (AttributeSlider.maxValue - AttributeSlider.minValue) / IncrementCount;

        //move to other attribute with arrow keys and when instruction panel is deactivated
        if (Input.GetKeyDown(KeyCode.UpArrow)&& !instructing)
        {
            ChangeAttribute(true);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)&& !instructing)
        {
            ChangeAttribute(false);
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow)&& !instructing)
        {
            //turn key yellow
            rightKey.color = Color.yellow;
            StopAllCoroutines();
            StartCoroutine(SwitchColour());
            //move slider
            AttributeSlider.value += increment;
            //if middle point is not yet achieved
            if (!midPoint[currentAttribute])
            {
                pressesMid[currentAttribute] += 1;
                //remove toggle when player pressed right at least one time
                toggleNoChange.gameObject.SetActive(false);
                if (pressesMid[currentAttribute] == 100)
                {
                    midPoint[currentAttribute] = true;
                }
            }
            else
            {
                pressesFinal[currentAttribute] += 1;
                if (pressesFinal[currentAttribute] == 100)
                {
                    finalPoint[currentAttribute] = true;
                }
            }
            //log when start pressing the button moving to the right
            GameState.CustomizerInput("Down", AttributeSlider.value / AttributeSlider.maxValue, currentAttribute.ToString(), Time.timeSinceLevelLoad);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow)&&!instructing)
        {
            rightKey.color = Color.white;
            //log when start pressing the button moving to the right
            GameState.CustomizerInput("Up", AttributeSlider.value / AttributeSlider.maxValue, currentAttribute.ToString(), Time.timeSinceLevelLoad);
        }
    }
    //switch right key back to 0 after a specific time
    IEnumerator SwitchColour()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        rightKey.color = Color.white;
    }
    /// <summary>
    /// Convert the floating-point value to a whole number, so we know which attribute to select
    /// </summary>
    int GetValue(float value)
    {
        if (value < 1-SliderTolerance)
        {
            currentAttributeValue = 0;
        }
        else if (value >= 1 -SliderTolerance && value < 2 - SliderTolerance)
        {
            currentAttributeValue = 1;
        }
        else if (value >= 2 - SliderTolerance)
        {
            currentAttributeValue = 2;
            finalPoint[currentAttribute] = true;
        }
        return currentAttributeValue;
    }

    public void ChangeAttribute(bool up)
    {
        timeSpent[currentAttribute] += (Time.time - startTime);
        if (!up)
        {
            if (currentAttributeIndex < attributes.Length - 1)
            {
                // Go to the next attribute.
                ++currentAttributeIndex;
                SetAttribute(attributes[currentAttributeIndex], currentAttributeIndex);
            }
        }
        else
        {
            if (currentAttributeIndex != 0)
            {
                // Go to the previous attribute.
                --currentAttributeIndex;
                SetAttribute(attributes[currentAttributeIndex], currentAttributeIndex);
            }
        }
    }

    //log all attribute info 
    public void ContinueGame()
    {
        timeSpent[currentAttribute] += (Time.time - startTime);
        //loop makes sure logs are posted in order player sees attributes
        foreach (var a in attributes)
        {
            switch (a)
            {
                case Attribute.Height:
                    GameState.AttributeInput(Attribute.Height.ToString(), GameState.HeightPreference[0].height.ToString()
                         , GameState.HeightPreference[1].height.ToString(), GameState.HeightPreference[2].height.ToString(),
                        midPoint[Attribute.Height], finalPoint[Attribute.Height], checkedUnwanted[Attribute.Height], pressesMid[Attribute.Height], pressesFinal[Attribute.Height],
                         timeSpent[Attribute.Height]);
                    break;
                case Attribute.Gender:
                    GameState.AttributeInput(Attribute.Gender.ToString(), GameState.GenderPreference[0].gender.ToString()
                         , GameState.GenderPreference[1].gender.ToString(), GameState.GenderPreference[2].gender.ToString(),
                        midPoint[Attribute.Gender], finalPoint[Attribute.Gender], checkedUnwanted[Attribute.Gender], pressesMid[Attribute.Gender], pressesFinal[Attribute.Gender],
                         timeSpent[Attribute.Gender]);
                    break;
                case Attribute.Age:
                    GameState.AttributeInput(Attribute.Age.ToString(), GameState.AgePreference[0].age.ToString()
                                 , GameState.AgePreference[1].age.ToString(), GameState.AgePreference[2].age.ToString(),
                                midPoint[Attribute.Age], finalPoint[Attribute.Age], checkedUnwanted[Attribute.Age], pressesMid[Attribute.Age], pressesFinal[Attribute.Age],
                                 timeSpent[Attribute.Age]);
                    break;
                case Attribute.Weight:
                    GameState.AttributeInput(Attribute.Weight.ToString(), GameState.WeightPreference[0].weight.ToString()
                         , GameState.WeightPreference[1].weight.ToString(), GameState.WeightPreference[2].weight.ToString(),
                        midPoint[Attribute.Weight], finalPoint[Attribute.Weight], checkedUnwanted[Attribute.Weight], pressesMid[Attribute.Weight], pressesFinal[Attribute.Weight],
                         timeSpent[Attribute.Weight]);
                    break;
                case Attribute.SkinTone:
                    GameState.AttributeInput(Attribute.SkinTone.ToString(), GameState.SkinTonePreference[0].skinTone.ToString()
                                 , GameState.SkinTonePreference[1].skinTone.ToString(), GameState.SkinTonePreference[2].skinTone.ToString(),
                                midPoint[Attribute.SkinTone], finalPoint[Attribute.SkinTone], checkedUnwanted[Attribute.SkinTone], pressesMid[Attribute.SkinTone], pressesFinal[Attribute.SkinTone],
                                 timeSpent[Attribute.SkinTone]);
                    break;
            
            }
        }
        //readjust positions of attribute icons so the current selected attribute is most left
        (GameState.AgePreference[0], GameState.AgePreference[GetValue((pressesMid[Attribute.Age] + pressesFinal[Attribute.Age]) / 100.0f)]) = (GameState.AgePreference[GetValue((pressesMid[Attribute.Age] + pressesFinal[Attribute.Age]) / 100.0f)], GameState.AgePreference[0]);
        (GameState.GenderPreference[0], GameState.GenderPreference[GetValue((pressesMid[Attribute.Gender] + pressesFinal[Attribute.Gender]) / 100.0f)]) = (GameState.GenderPreference[GetValue((pressesMid[Attribute.Gender] + pressesFinal[Attribute.Gender]) / 100.0f)], GameState.GenderPreference[0]);
        (GameState.WeightPreference[0], GameState.WeightPreference[GetValue((pressesMid[Attribute.Weight] + pressesFinal[Attribute.Weight]) / 100.0f)]) = (GameState.WeightPreference[GetValue((pressesMid[Attribute.Weight] + pressesFinal[Attribute.Weight]) / 100.0f)], GameState.WeightPreference[0]);
        (GameState.SkinTonePreference[0], GameState.SkinTonePreference[GetValue((pressesMid[Attribute.SkinTone] + pressesFinal[Attribute.SkinTone]) / 100.0f)]) = (GameState.SkinTonePreference[GetValue((pressesMid[Attribute.SkinTone] + pressesFinal[Attribute.SkinTone]) / 100.0f)], GameState.SkinTonePreference[0]);
        (GameState.HeightPreference[0], GameState.HeightPreference[GetValue((pressesMid[Attribute.Height] + pressesFinal[Attribute.Height]) / 100.0f)]) = (GameState.HeightPreference[GetValue((pressesMid[Attribute.Height] + pressesFinal[Attribute.Height]) / 100.0f)], GameState.HeightPreference[0]);
        //go to game
        SceneManager.LoadScene("GameScene");
    }

    public void SetAttribute(Attribute attribute, int i)
    {
        currentAttribute = attribute;

        //reset toggle visual
        if (pressesMid[currentAttribute] > 0)
        {
            toggleNoChange.gameObject.SetActive(false);
        }
        else
        {
            toggleNoChange.isOn = checkedUnwanted[currentAttribute];
            toggleNoChange.gameObject.SetActive(true);
        }

        //reset logging variables
        startTime = Time.time;
        Slider_OnValueChanged((pressesMid[attribute] + pressesFinal[attribute])/ 100.0f);

        // Reset slider value.
        AttributeSlider.value = (pressesMid[attribute] + pressesFinal[attribute]) / 100.0f;

        // Hide the previous and next arrow.
        if (i != 0)
        {
            Arrows[i - 1].enabled = false;
        }
        if (i != attributes.Length - 1)
        {
            Arrows[i + 1].enabled = false;
        }
        // Show current arrow.
        Arrows[i].enabled = true;

        // Set level images based on preferences.
        switch (attribute)
        {
            case Attribute.Gender:
                Level1Image.sprite = GenderSprites[GameState.GenderPreference[0].gender];
                Level2Image.sprite = GenderSprites[GameState.GenderPreference[1].gender];
                Level3Image.sprite = GenderSprites[GameState.GenderPreference[2].gender];
                break;
            case Attribute.Weight:
                Level1Image.sprite = WeightSprites[GameState.WeightPreference[0].weight];
                Level2Image.sprite = WeightSprites[GameState.WeightPreference[1].weight];
                Level3Image.sprite = WeightSprites[GameState.WeightPreference[2].weight];
                break;
            case Attribute.Age:
                Level1Image.sprite = AgeSprites[GameState.AgePreference[0].age];
                Level2Image.sprite = AgeSprites[GameState.AgePreference[1].age];
                Level3Image.sprite = AgeSprites[GameState.AgePreference[2].age];
                break;
            case Attribute.SkinTone:
                Level1Image.sprite = SkinSprites[GameState.SkinTonePreference[0].skinTone];
                Level2Image.sprite = SkinSprites[GameState.SkinTonePreference[1].skinTone];
                Level3Image.sprite = SkinSprites[GameState.SkinTonePreference[2].skinTone];
                break;
            case Attribute.Height:
                Level1Image.sprite = HeightSprites[GameState.HeightPreference[0].height];
                Level2Image.sprite = HeightSprites[GameState.HeightPreference[1].height];
                Level3Image.sprite = HeightSprites[GameState.HeightPreference[2].height];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null);
        }
    }
    //opens or closes the instruction panel when button is pressed
    public void OpenOrCloseInstructions()
    {
        instructionPanel.SetActive(!instructionPanel.activeSelf);
        if (instructionPanel.activeSelf)
        {
            instructing = true;
            toggleNoChange.interactable = false;
            //time scale is set to 0, so the logging of the spent time of an attribute is not affected
            Time.timeScale = 0;
        }
        else
        {
            instructing = false;
            toggleNoChange.interactable = true;
            Time.timeScale = 1;
        }
    }
    //handles logic of toggle which displays whether the player wants to change a certain attribute
    public void ToggleValueChanged(Toggle change)
    {
        if (toggleNoChange.isOn && !checkedUnwanted[currentAttribute] && !instructing)
        {
            //log check
            GameState.CustomizerInput("Check", 0, currentAttribute.ToString(), Time.timeSinceLevelLoad);
            //store that check is selected
            checkedUnwanted[currentAttribute] = true;
        }
        else if (!toggleNoChange.isOn && checkedUnwanted[currentAttribute]&& !instructing)
        {
            //log uncheck
            GameState.CustomizerInput("Uncheck", 0, currentAttribute.ToString(), Time.timeSinceLevelLoad);
            //store that check is unselected
            checkedUnwanted[currentAttribute] = false;
        }
    }
    //store changes in attribute
    public void Slider_OnValueChanged(float value)
    {

        switch (currentAttribute)
        {
            case Attribute.Gender:
                AvatarCustomization.Gender = GameState.GenderPreference[GetValue(value)].gender;
                break;
            case Attribute.Weight:
                AvatarCustomization.Weight = GameState.WeightPreference[GetValue(value)].weight;
                break;
            case Attribute.Age:
                AvatarCustomization.Age = GameState.AgePreference[GetValue(value)].age;
                break;
            case Attribute.SkinTone:
                AvatarCustomization.SkinTone = GameState.SkinTonePreference[GetValue(value)].skinTone;
                break;
            case Attribute.Height:
                AvatarCustomization.Height = GameState.HeightPreference[GetValue(value)].height;
                break;
        }
    }
}
