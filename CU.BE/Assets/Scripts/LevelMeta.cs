using UnityEngine;

public class LevelMeta : MonoBehaviour
{
    // initial number of collectables present in the level when spawned
    public int initialCollectables = 0;
    // whether this level's coins have already been counted into GameState.CoinsPossible
    public bool counted = false;
}
