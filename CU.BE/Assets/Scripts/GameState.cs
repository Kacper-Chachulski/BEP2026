using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public enum Attribute
{
    Gender,
    Weight,
    Age,
    SkinTone,
    Height
}
public enum Gender
{
    Male,
    Female,
    NonBinary
}
public enum Weight
{
    Fit,
    Average,
    Large
}
public enum Age
{
    Young,
    Adult,
    Old
}
public enum SkinTone
{
    Fair,
    Brown,
    Dark
}
public enum Height
{
    Short,
    Average,
    Tall
}

/// <summary>
/// The GameState class.
/// </summary>
public static class GameState
{
    //new Page BOF framework
    [DllImport("__Internal")]
    public static extern void RedirectBOF();

    /// <summary>
    /// Player's name.
    /// </summary>
    public static string Name { get; set; }

    /// <summary>
    /// The unique player identifier.
    /// </summary>
    public static int PID { get; set; }

    /// <summary>
    /// The player's current score.
    /// </summary>
    public static int Score { get; set; }
    public static int BarrierHits { get; set; }
    // Total possible coins (collectables present in the level), regardless of pickup
    public static int CoinsPossible { get; set; }
    public static float TimePlayed { get; set; }
    public static int Segment { get; set; }
    public static int LaneChanges { get; set; }
    public static bool GameStarted { get; set; }
    public static bool GameEnded { get; set; }

    public static int Condition { get; set; }
    public static string Answers { get; set; }

    public static int[] selectedAttributes = new int[] { 0, 0, 0, 0, 0 };


    /// <summary>
    /// Whether this is the first time the user is playing the game.
    /// (Determines which screen to show after completing the level).
    /// </summary>
    public static bool FistTime { get; set; } = true;

    /// <summary>
    /// Age sorted by preference.
    /// </summary>
    public static (Age age, int preference)[] AgePreference = {
        ( Age.Young, 0 ),
        ( Age.Adult, 0 ),
        ( Age.Old, 0 ),
    };

    public static void AddAge(Age age, int preference)
    {
        List<(Age age, int preference)> list = new(AgePreference) { (age, preference) };
        var sortedList = list.OrderBy(a => a.preference);
        AgePreference = sortedList.Skip(1).ToArray();
    }

    /// <summary>
    /// Gender sorted by preference.
    /// </summary>
    public static (Gender gender, int preference)[] GenderPreference = {
        (Gender.Male, 0),
        (Gender.Female, 0),
        (Gender.NonBinary, 0)
    };

    public static void AddGender(Gender gender, int preference)
    {
        List<(Gender gender, int preference)> list = new(GenderPreference) { (gender, preference) };
        var sortedList = list.OrderBy(a => a.preference);
        GenderPreference = sortedList.Skip(1).ToArray();
    }

    /// <summary>
    /// Height sorted by preference.
    /// </summary>
    public static (Height height, int preference)[] HeightPreference = {
        (Height.Average, 0),
        (Height.Short, 0),
        (Height.Tall, 0)
    };

    public static void AddHeight(Height height, int preference)
    {
        List<(Height height, int preference)> list = new(HeightPreference) { (height, preference) };
        var sortedList = list.OrderBy(a => a.preference);
        HeightPreference = sortedList.Skip(1).ToArray();
    }

    /// <summary>
    /// Weight sorted by preference.
    /// </summary>
    public static (Weight weight, int preference)[] WeightPreference = {
        (Weight.Average, 0),
        (Weight.Fit, 0),
        (Weight.Large, 0)
    };

    public static void AddWeight(Weight weight, int preference)
    {
        List<(Weight weight, int preference)> list = new(WeightPreference) { (weight, preference) };
        var sortedList = list.OrderBy(a => a.preference);
        WeightPreference = sortedList.Skip(1).ToArray();
    }

    /// <summary>
    /// Skin tone sorted by preference.
    /// </summary>
    public static (SkinTone skinTone, int preference)[] SkinTonePreference = {
        (SkinTone.Brown, 0),
        (SkinTone.Dark, 0),
        (SkinTone.Fair, 0)
    };

    public static void AddSkinTone(SkinTone skinTone, int preference)
    {
        List<(SkinTone skinTone, int preference)> list = new(SkinTonePreference) { (skinTone, preference) };
        var sortedList = list.OrderBy(a => a.preference);
        SkinTonePreference = sortedList.Skip(1).ToArray();
    }
    public static void EndGame()
    {
        TimePlayed = Time.timeSinceLevelLoad;
        // Only post once: if the segment is already ended, don't post again.
        if (!GameEnded)
        {
            PostInputSegment();
            GameEnded = true;
        }
        RedirectBOF();
    }
    //load the condition
    public static IEnumerator LoadCondition()
    {
        string url = "/fetch_condition";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                string response = request.downloadHandler.text;

                if (response.Length == 0)
                {
                    Debug.Log("Unable to load condition! Does the participant have a valid session?");
                    //testing purposes
                    Condition = 1;
                }
                else
                {
                    Condition = int.Parse(response);
                    //if condition is 5 or 6, normative avatar is assigned to the player
                    if (Condition == 5|| Condition ==6)
                    {
                        (GenderPreference[0], GenderPreference[1]) = (GenderPreference[1],GenderPreference[0]);
                        (AgePreference[0], AgePreference[1]) = (AgePreference[1], AgePreference[0]);
                        (WeightPreference[0],WeightPreference[1]) = (WeightPreference[1], WeightPreference[0]);
                        (HeightPreference[0], HeightPreference[1]) = (HeightPreference[1], HeightPreference[0]);
                        (SkinTonePreference[0], SkinTonePreference[1]) = (SkinTonePreference[1], SkinTonePreference[0]);
                    }
                    //if condition is 3 or 4, desirable avatar is assigned to the player
                    else if (Condition == 3 || Condition == 4)
                    {
                        (GenderPreference[0], GenderPreference[1], GenderPreference[2]) = (GenderPreference[2], GenderPreference[0], GenderPreference[1]);
                        (AgePreference[0], AgePreference[1], AgePreference[1]) = (AgePreference[2], AgePreference[0], AgePreference[1]);
                        (WeightPreference[0], WeightPreference[1], WeightPreference[2]) = (WeightPreference[2], WeightPreference[0], WeightPreference[1]);
                        (HeightPreference[0], HeightPreference[1], HeightPreference[2]) = (HeightPreference[2], HeightPreference[0], HeightPreference[1]);
                        (SkinTonePreference[0], SkinTonePreference[1], SkinTonePreference[2]) = (SkinTonePreference[2], SkinTonePreference[0], SkinTonePreference[1]);
                    }
                    //otherwise condition is 1 or 2 and undesirable avatar is assigned to the player

                }
            }
        }
    }
    //log button inputs during the game scene
    public static void LiveInput(string currEvent)
    {

        if (!GameEnded && GameStarted)
        {
            WWWForm frm = new WWWForm();
            //Game Statistics
            frm.AddField("segment", Segment);
            frm.AddField("eventKey", currEvent);

            try
            {
                var request = UnityWebRequest.Post("#", frm);
                request.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.Log("Error in PostInput(): " + ex.Message);
            }
        }
    }
    //log n and m button inputs
    public static void NMInput(string currEvent)
    {

        if (!GameEnded && GameStarted)
        {
            WWWForm frm = new WWWForm();
            //Game Statistics
            frm.AddField("segment", Segment);
            frm.AddField("key", currEvent);

            try
            {
                var request = UnityWebRequest.Post("#", frm);
                request.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.Log("Error in PostInput(): " + ex.Message);
            }
        }
    }
    //log summary after game behaviour
    public static void PostInputSegment()
    {
        // Ensure any levels already passed but not yet removed are counted
        try
        {
            LevelSpawner.FinalizeAllPassedLevels();
        }
        catch (Exception)
        {
            // best-effort; don't block posting
        }

        WWWForm frm = new WWWForm();

        Debug.Log($"[GameState] Posting segment={Segment} Score={Score} CoinsPossible={CoinsPossible} spawnedLevels={LevelSpawner.GetTotalSpawnedLevelsCount()} spawnedInitialCoins={LevelSpawner.GetTotalSpawnedInitialCoinsSum()}");

        frm.AddField("segment", Segment);
        // `Score` already includes penalty adjustments; post it directly.
        frm.AddField("coins", Score);
        // Include total possible coins passed in this segment (may differ from coins picked up)
        frm.AddField("coinsPassed", CoinsPossible);
        // Debug fields: spawned levels and sum of initial coins still tracked
        try
        {
            frm.AddField("spawnedLevels", LevelSpawner.GetTotalSpawnedLevelsCount());
            frm.AddField("spawnedInitialCoins", LevelSpawner.GetTotalSpawnedInitialCoinsSum());
        }
        catch (Exception)
        {
        }
        frm.AddField("barrierHits", BarrierHits);
        frm.AddField("timePlayed", TimePlayed.ToString());
        frm.AddField("laneChanges", LaneChanges);

        try
        {
            var request = UnityWebRequest.Post("#", frm);
            request.SendWebRequest();
        }
        catch (Exception ex)
        {
            Debug.Log("Error in PostInput(): " + ex.Message);
        }
    }
    //log questionnaire weights
    public static void PostQuestResults(IDictionary<string,int> featureWeights)
    {
        WWWForm frm = new WWWForm();
        frm.AddField("maleWeight", featureWeights["Male Gender"]);
        frm.AddField("femaleWeight", featureWeights["Female Gender"]);
        frm.AddField("fluidWeight", featureWeights["Fluid Gender"]);

        frm.AddField("youngWeight", featureWeights["Young Age"]);
        frm.AddField("adultWeight", featureWeights["Adult Age"]);
        frm.AddField("oldWeight", featureWeights["Old Age"]);

        frm.AddField("shortWeight", featureWeights["Short"]);
        frm.AddField("averageHeightWeight", featureWeights["Average Height"]);
        frm.AddField("tallWeight", featureWeights["Tall"]);

        frm.AddField("fitWeight", featureWeights["Thin"]);
        frm.AddField("averageWeightWeight", featureWeights["Average Weight"]);
        frm.AddField("overweightWeight", featureWeights["Overweight"]);

        frm.AddField("fairWeight", featureWeights["Fair Skin"]);
        frm.AddField("brownWeight", featureWeights["Brown Skin"]);
        frm.AddField("darkWeight", featureWeights["Dark Skin"]);

        try
        {
            var request = UnityWebRequest.Post("#", frm);
            request.SendWebRequest();
        }
        catch (Exception ex)
        {
            Debug.Log("Error in PostInput(): " + ex.Message);
        }
    }
    public static void PostAvatar()
    {
        WWWForm frm = new WWWForm();

        frm.AddField("segmentCI", Segment);
        frm.AddField("age", AgePreference[0].age.ToString());
        frm.AddField("gender", GenderPreference[0].gender.ToString());
        frm.AddField("weight", WeightPreference[0].weight.ToString());
        frm.AddField("skinTone", SkinTonePreference[0].skinTone.ToString());
        frm.AddField("height", HeightPreference[0].height.ToString());

        try
        {
            var request = UnityWebRequest.Post("#", frm);
            request.SendWebRequest();
        }
        catch (Exception ex)
        {
            Debug.Log("Error in PostInput(): " + ex.Message);
        }
    }
    //log questionnaire button presses
    public static void QuestionnaireLiveInput(string keyEvent, string choice1, string choice2, float timeSpent)
    {
        WWWForm frm = new WWWForm();
        //Game Statistics
        frm.AddField("event", keyEvent);
        frm.AddField("leftChoice", choice1);
        frm.AddField("rightChoice", choice2);
        frm.AddField("timeSpent", timeSpent.ToString());

        try
        {
            var request = UnityWebRequest.Post("#", frm);
            request.SendWebRequest();
        }
        catch (Exception ex)
        {
            Debug.Log("Error in PostInput(): " + ex.Message);
        }
    }
    //log customizer button presses
    public static void CustomizerInput(string keyEvent, float position, string attribute, float exactTime)
    {

        position = Mathf.Round(position * 1000f) / 1000f;
        WWWForm frm = new WWWForm();

        frm.AddField("segmentCI", Segment);
        frm.AddField("attribute", attribute);
        frm.AddField("event", keyEvent);
        frm.AddField("position", position.ToString());
        frm.AddField("exactTime", exactTime.ToString());

        try
        {
            var request = UnityWebRequest.Post("#", frm);
            request.SendWebRequest();
        }
        catch (Exception ex)
        {
            Debug.Log("Error in PostInput(): " + ex.Message);
        }
    }
    //log summary player behaviour in the customization screen for each attribute
    public static void AttributeInput(string attribute, string startAttribute, string midAttribute, string targetAttribute, bool achieved_midpoint, bool achieved_target, bool checkedUnwanted, int keyPressesMid, int keyPressesTotal, float timeSpent)
    {
        WWWForm frm = new WWWForm();

        frm.AddField("segmentCI", Segment);
        frm.AddField("attribute", attribute);
        frm.AddField("startAttribute", startAttribute);
        frm.AddField("midAttribute", midAttribute);
        frm.AddField("tarAttribute", targetAttribute);
        frm.AddField("achMid", achieved_midpoint.ToString());
        frm.AddField("achTar", achieved_target.ToString());
        frm.AddField("checkedNoChange", checkedUnwanted.ToString());
        frm.AddField("keyMid", keyPressesMid);
        frm.AddField("keyTar", keyPressesTotal);
        frm.AddField("timeSpentAtt", timeSpent.ToString());

        try
        {
            var request = UnityWebRequest.Post("#", frm);
            request.SendWebRequest();
        }
        catch (Exception ex)
        {
            Debug.Log("Error in PostInput(): " + ex.Message);
        }
    }

}
