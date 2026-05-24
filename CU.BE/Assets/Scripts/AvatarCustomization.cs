using UnityEngine;

public class AvatarCustomization : MonoBehaviour
{

    [SerializeField] private Gender gender = Gender.Male;
    [SerializeField] private Weight weight = Weight.Fit;
    [SerializeField] private Age age = Age.Young;
    [SerializeField] private SkinTone skinTone = SkinTone.Fair;
    [SerializeField] private Height height = Height.Average;

    public Gender Gender
    {
        get => gender;
        set
        {
            gender = value;
            ShowGeometry(gender, weight);
        }
    }

    public Weight Weight
    {
        get => weight;
        set
        {
            weight = value;
            ShowGeometry(gender, weight);
        }
    }

    public Age Age
    {
        get => age;
        set
        {
            age = value;
            SetAge(age);
        }
    }

    public SkinTone SkinTone
    {
        get => skinTone; 
        set
        {
            skinTone = value;
            SetSkin(skinTone);
        }
    }

    public Height Height
    {
        get => height; 
        set
        {
            height = value;
            SetHeight(height);
        }
    }

    /// <summary>
    /// Root character (used for scaling).
    /// </summary>
    public GameObject Character;

    /// <summary>
    /// Female geometry.
    /// </summary>
    public GameObject Character_Female_Average;
    public GameObject Character_Female_Fit;
    public GameObject Character_Female_Large;

    /// <summary>
    /// Male geometry.
    /// </summary>
    public GameObject Character_Male_Average;
    public GameObject Character_Male_Fit;
    public GameObject Character_Male_Large;

    /// <summary>
    /// Non-binary geometry.
    /// </summary>
    public GameObject Character_NB_Average;
    public GameObject Character_NB_Fit;
    public GameObject Character_NB_Large;

    /// <summary>
    /// Primary animator on root character.
    /// </summary>
    public Animator animator;

    /// <summary>
    /// Animation controllers.
    /// </summary>
    public AnimatorOverrideController Anim_Adult;
    public AnimatorOverrideController Anim_Young;
    public AnimatorOverrideController Anim_Old;

    public Material SkinMaterial;
    public Material HairMaterial;

    public Color HairColor_Young;
    public Color HairColor_Adult;
    public Color HairColor_Old;

    public Color SkinTone_Fair;
    public Color SkinTone_Brown;
    public Color SkinTone_Dark;

    public float Height_Short = 0.9f;
    public float Height_Average = 1.0f;
    public float Height_Tall = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        Gender = GameState.GenderPreference[0].gender;
        Weight = GameState.WeightPreference[0].weight;
        Age = GameState.AgePreference[0].age;
        SkinTone = GameState.SkinTonePreference[0].skinTone;
        Height = GameState.HeightPreference[0].height;
    }

    private void ShowGeometry(Gender gender, Weight weight)
    {

        // First deactivate geometry.
        Character_Female_Average.SetActive(false);
        Character_Female_Fit.SetActive(false);
        Character_Female_Large.SetActive(false);

        Character_Male_Average.SetActive(false);
        Character_Male_Fit.SetActive(false);
        Character_Male_Large.SetActive(false);

        Character_NB_Average.SetActive(false);
        Character_NB_Fit.SetActive(false);
        Character_NB_Large.SetActive(false);

        // Activate only specific geometry.
        switch (gender)
        {
            case Gender.Male:
                switch (weight)
                {
                    case Weight.Average:
                        Character_Male_Average.SetActive(true);
                        break;
                    case Weight.Large:
                        Character_Male_Large.SetActive(true);
                        break;
                    case Weight.Fit:
                        Character_Male_Fit.SetActive(true);
                        break;
                }
                break;
            case Gender.Female:
                switch (weight)
                {
                    case Weight.Average:
                        Character_Female_Average.SetActive(true);
                        break;
                    case Weight.Large:
                        Character_Female_Large.SetActive(true);
                        break;
                    case Weight.Fit:
                        Character_Female_Fit.SetActive(true);
                        break;
                }
                break;
            case Gender.NonBinary:
                switch (weight)
                {
                    case Weight.Average:
                        Character_NB_Average.SetActive(true);
                        break;
                    case Weight.Large:
                        Character_NB_Large.SetActive(true);
                        break;
                    case Weight.Fit:
                        Character_NB_Fit.SetActive(true);
                        break;
                }
                break;
        }
    }

    private void SetAge(Age age)
    {

        switch (age)
        {
            case Age.Adult:
                animator.runtimeAnimatorController = Anim_Adult;
                HairMaterial.color = HairColor_Adult;
                break;
            case Age.Old:
                animator.runtimeAnimatorController = Anim_Old;
                HairMaterial.color = HairColor_Old;
                break;
            case Age.Young:
                animator.runtimeAnimatorController = Anim_Young;
                HairMaterial.color = HairColor_Young;
                break;
        }
    }

    private void SetSkin(SkinTone skinTone)
    {

        switch (skinTone)
        {
            case SkinTone.Brown:
                SkinMaterial.color = SkinTone_Brown;
                break;
            case SkinTone.Dark:
                SkinMaterial.color = SkinTone_Dark;
                break;
            case SkinTone.Fair:
                SkinMaterial.color = SkinTone_Fair;
                break;
        }
    }

    private void SetHeight(Height height)
    {

        switch (height)
        {
            case Height.Average:
                Character.transform.localScale = new Vector3(Height_Average, Height_Average, Height_Average);
                break;
            case Height.Short:
                Character.transform.localScale = new Vector3(Height_Short, Height_Short, Height_Short);
                break;
            case Height.Tall:
                Character.transform.localScale = new Vector3(Height_Tall, Height_Tall, Height_Tall);
                break;
        }
    }
}
