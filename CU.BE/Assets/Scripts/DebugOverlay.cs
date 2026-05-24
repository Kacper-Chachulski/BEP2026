using UnityEngine;

public class DebugOverlay : MonoBehaviour
{
    void OnGUI()
    {
        // Small overlay showing key counters for debugging
        string[] lines = new string[] {
            $"Segment: {GameState.Segment}",
            $"Score: {GameState.Score}",
            $"CoinsPossible: {GameState.CoinsPossible}",
            $"SpawnedLevels: {LevelSpawner.GetTotalSpawnedLevelsCount()}",
            $"SpawnedInitialCoins: {LevelSpawner.GetTotalSpawnedInitialCoinsSum()}"
        };

        int x = 10, y = 10, w = 360, h = 18;
        GUI.Box(new Rect(x - 4, y - 4, w + 8, lines.Length * h + 12), "Debug");
        for (int i = 0; i < lines.Length; i++)
        {
            GUI.Label(new Rect(x, y + i * h, w, h), lines[i]);
        }
    }
}
