using UnityEngine;

public class achievementManager : MonoBehaviour
{
    public static achievementManager instance;

    void Awake()
    {
        instance = this;
    }

    public void unlockAchievement(achievementData achievement)
    {
        if (achievement == null)
            return;

        string key = "Achievement_" + achievement.achievementID;

        if (PlayerPrefs.GetInt(key, 0) == 1)
            return;

        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();

        Debug.Log("Achievement Unlocked: " + achievement.achievementName);
    }

    public bool isUnlocked(achievementData achievement)
    {
        if (achievement == null)
            return false;

        string key = "Achievement_" + achievement.achievementID;
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
}