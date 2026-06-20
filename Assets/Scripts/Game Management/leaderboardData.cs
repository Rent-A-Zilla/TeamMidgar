using UnityEngine;

[CreateAssetMenu(fileName = "New Leaderboard", menuName = "Leaderboards/Leaderboard")]
public class leaderboardData : ScriptableObject
{
    public string leaderboardKey;
    public int[] defaultScores = new int[5];
    public string[] defaultNames = new string[5];
}