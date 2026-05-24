using System.Collections.Generic;
using UnityEngine;
using System.Collections;
//using Random = System.Random;

/// <summary>
/// This class is responsible for spawning in new levels based on a list of level prefabs.
/// </summary>
public class LevelSpawner : MonoBehaviour
{

    // Track all active spawner instances (some scenes may have multiple spawners)
    public static System.Collections.Generic.List<LevelSpawner> Instances { get; } = new System.Collections.Generic.List<LevelSpawner>();

    [Tooltip("The level prefabs to spawn (levels are spawned randomly to create variation).")]
    public GameObject[] LevelPrefabs;
    public GameObject[] LevelPrefabsSeg0;
    public GameObject[] LevelPrefabsSeg2;


    List<GameObject> currentLevels =  new List<GameObject>();
    bool waiting = false;
    GameObject chosenLevel;
    GameObject spawnedLevel;
    int lengthPrefabs;
    int randomLevel;
    float zValue = 250;
    float zIncrement = 500;
    public GameObject endButton;

    void Start()
    {
        Instances.Add(this);
        // Reset total possible coins counter for this segment
        GameState.CoinsPossible = 0;
        //player can end the game manually at the last segment
        if (GameState.Segment == 2)
        {
            endButton.SetActive(true);
        }
        else
        {
            endButton.SetActive(false);
        }
        lengthPrefabs = LevelPrefabsSeg2.Length;
        // Spawn entire environment at start of first 2 environments
        switch (GameState.Segment)
        {
            case 0:
                StartCoroutine(SpawnLevelFixed(zValue, 0));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 1));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 2));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 0));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 1));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 2));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 0));
                zValue += zIncrement;
                break;
            case 1:
                StartCoroutine(SpawnLevelFixed(zValue, 0));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 1));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 2));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 0));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 1));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 2));
                zValue += zIncrement;
                StartCoroutine(SpawnLevelFixed(zValue, 0));
                zValue += zIncrement;
                break;
            //spawn 3 environments at the start of the last segment
            case 2:
                for (int i = 0; i < lengthPrefabs; ++i)
                {
                    StartCoroutine(SpawnLevel(zValue));
                    zValue += zIncrement;
                }
                break;
        }
    }

    void OnDestroy()
    {
        Instances.Remove(this);
    }
    //spawn fixed level, not a random one
    IEnumerator SpawnLevelFixed(float z, int levelSpawned)
    {
        randomLevel = levelSpawned ;
        switch (GameState.Segment)
        {
            case 0:
                chosenLevel = LevelPrefabsSeg0[randomLevel];
                break;
            default:
                chosenLevel = LevelPrefabs[randomLevel];
                break;
        }

        spawnedLevel = Instantiate(chosenLevel, new Vector3(0, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)));

        // Record initial collectable count for this spawned level (only positive-score coins)
        var allCollectables = spawnedLevel.GetComponentsInChildren<Collectable>();
        int initialCoinCount = 0;
        if (allCollectables != null)
        {
            foreach (var c in allCollectables)
            {
                if (c != null && c.Score > 0)
                    initialCoinCount += 1;
            }
        }
        var meta = spawnedLevel.AddComponent<LevelMeta>();
        meta.initialCollectables = initialCoinCount;
        Debug.Log($"[LevelSpawner] Spawned fixed level '{chosenLevel.name}' at z={z} initialCoins={initialCoinCount}");

        currentLevels.Add(spawnedLevel);
        yield return null;
    }
    //spawn random level
    IEnumerator SpawnLevel(float z)
    {
        randomLevel = Random.Range(0, lengthPrefabs);
        switch (GameState.Segment)
        {
            case 2:
                chosenLevel = LevelPrefabsSeg2[randomLevel];
                break;
            default:
                chosenLevel = LevelPrefabs[randomLevel];
                break;
        }
        spawnedLevel = Instantiate(chosenLevel, new Vector3(0, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)));

        // Record initial collectable count for this spawned level (only positive-score coins)
        var allCollectables2 = spawnedLevel.GetComponentsInChildren<Collectable>();
        int initialCoinCount2 = 0;
        if (allCollectables2 != null)
        {
            foreach (var c in allCollectables2)
            {
                if (c != null && c.Score > 0)
                    initialCoinCount2 += 1;
            }
        }
        var meta2 = spawnedLevel.AddComponent<LevelMeta>();
        meta2.initialCollectables = initialCoinCount2;
        Debug.Log($"[LevelSpawner] Spawned level '{chosenLevel.name}' at z={z} initialCoins={initialCoinCount2}");

        currentLevels.Add(spawnedLevel);
        yield return null;
    }
    //remove level which the player already passed
    IEnumerator RemoveLevel()
    {
        // Use the recorded initial collectable count for the level the player just passed
        var levelToRemove = currentLevels[0];
        if (levelToRemove != null)
        {
            var meta = levelToRemove.GetComponent<LevelMeta>();
            if (meta != null)
            {
                Debug.Log($"[LevelSpawner] Removing level at z={levelToRemove.transform.position.z} initialCoins={meta.initialCollectables} counted={meta.counted}");
                if (!meta.counted)
                {
                    GameState.CoinsPossible += meta.initialCollectables;
                    meta.counted = true;
                    Debug.Log($"[LevelSpawner] Added {meta.initialCollectables} to CoinsPossible -> {GameState.CoinsPossible}");
                }
            }
            else
            {
                // Fallback: count remaining positive-score collectable children
                var collectables = levelToRemove.GetComponentsInChildren<Collectable>();
                if (collectables != null)
                {
                    int cnt = 0;
                    foreach (var c in collectables)
                    {
                        if (c != null && c.Score > 0)
                            cnt += 1;
                    }
                    GameState.CoinsPossible += cnt;
                }
            }
        }

        Destroy(levelToRemove);
        currentLevels.RemoveAt(0);
        yield return null;
    }

    IEnumerator Waiting()
    {
        yield return new WaitForSeconds(1.0f);
        waiting = false;
    }

    // Force-count any spawned levels that the player has passed but which
    // haven't yet been removed by the trigger (e.g., on EndGame).
    public void FinalizePassedLevels()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float playerZ = player.transform.position.z;
        // safety margin to consider level passed
        const float passedMargin = 10f;

        // Process while the first spawned level is behind the player
        while (currentLevels.Count > 0)
        {
            var first = currentLevels[0];
            if (first == null)
            {
                currentLevels.RemoveAt(0);
                continue;
            }

            if (first.transform.position.z + passedMargin < playerZ)
            {
                var meta = first.GetComponent<LevelMeta>();
                if (meta != null)
                {
                    Debug.Log($"[LevelSpawner] Finalize passed level z={first.transform.position.z} initialCoins={meta.initialCollectables} counted={meta.counted}");
                    if (!meta.counted)
                    {
                        GameState.CoinsPossible += meta.initialCollectables;
                        meta.counted = true;
                        Debug.Log($"[LevelSpawner] Finalize added {meta.initialCollectables} -> CoinsPossible={GameState.CoinsPossible}");
                    }
                }
                else
                {
                    var collectables = first.GetComponentsInChildren<Collectable>();
                    if (collectables != null)
                    {
                        int cnt = 0;
                        foreach (var c in collectables)
                        {
                            if (c != null && c.Score > 0)
                                cnt += 1;
                        }
                        GameState.CoinsPossible += cnt;
                    }
                }

                Destroy(first);
                currentLevels.RemoveAt(0);
            }
            else
            {
                break;
            }
        }
    }

    // Aggregate helpers across all spawners
    public static void FinalizeAllPassedLevels()
    {
        foreach (var s in Instances.ToArray())
        {
            if (s == null) continue;
            s.FinalizePassedLevels();
        }
    }

    public static int GetTotalSpawnedLevelsCount()
    {
        int sum = 0;
        foreach (var s in Instances)
            sum += s.GetSpawnedLevelsCount();
        return sum;
    }

    public static int GetTotalSpawnedInitialCoinsSum()
    {
        int sum = 0;
        foreach (var s in Instances)
            sum += s.GetSpawnedInitialCoinsSum();
        return sum;
    }

    // Debug helpers
    public int GetSpawnedLevelsCount()
    {
        return currentLevels.Count;
    }

    public int GetSpawnedInitialCoinsSum()
    {
        int sum = 0;
        foreach (var lvl in currentLevels)
        {
            if (lvl == null) continue;
            var meta = lvl.GetComponent<LevelMeta>();
            if (meta != null) sum += meta.initialCollectables;
        }
        return sum;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !waiting && GameState.Segment == 2)
        {
            //using waiting delay to ensure this function is not triggered twice
            waiting = true;
            StartCoroutine(Waiting());
            //spawn next environment
            StartCoroutine(SpawnLevel(zValue));
            //remove passed environment
            StartCoroutine(RemoveLevel());
            zValue += zIncrement;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + zIncrement);
        }
    }
    //ran when player is done with the game level
    public void EndGame()
    {
        GameState.EndGame();
    }
}
