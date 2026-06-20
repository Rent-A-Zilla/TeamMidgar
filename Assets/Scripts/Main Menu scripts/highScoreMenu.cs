using UnityEngine;
using TMPro;

public class highScoreMenu : MonoBehaviour
{
    [SerializeField] TMP_Text[] scoreTexts;
    [SerializeField] TMP_Text[] nameTexts;

    public void showScores(string leaderboardKey)
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            nameTexts[i].text = PlayerPrefs.GetString(
                leaderboardKey + "_HighScoreName_" + i,
                getDefaultName(i));

            scoreTexts[i].text = PlayerPrefs.GetInt(
                leaderboardKey + "_HighScore_" + i,
                getDefaultScore(i)).ToString();
        }
    }

    int getDefaultScore(int index)
    {
        int[] defaultScores =
        {
            2500, 2000, 1500, 1000, 500
        };

        return defaultScores[index];
    }
    string getDefaultName(int index)
    {
        string[] defaultNames =
        {
            "Razor",
            "Ghost",
            "Nova",
            "Viper",
            "Ace"
        };

        return defaultNames[index];
    }
}