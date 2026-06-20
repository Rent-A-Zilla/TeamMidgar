using UnityEngine;
using TMPro;

public class highScoreMenu : MonoBehaviour
{
    [SerializeField] TMP_Text[] scoreTexts;
    [SerializeField] TMP_Text[] nameTexts;

    public void showScores(leaderboardData leaderboardData)
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            nameTexts[i].text = PlayerPrefs.GetString(
                leaderboardData.leaderboardKey + "_HighScoreName_" + i,
                leaderboardData.defaultNames[i]);

            scoreTexts[i].text = PlayerPrefs.GetInt(
                leaderboardData.leaderboardKey + "_HighScore_" + i,
                leaderboardData.defaultScores[i]).ToString();
        }
    }
    void OnEnable()
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            nameTexts[i].text = "";
            scoreTexts[i].text = "";
        }
    }
}