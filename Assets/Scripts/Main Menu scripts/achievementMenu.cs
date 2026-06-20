using UnityEngine;
using TMPro;

public class achievementMenu : MonoBehaviour
{
    [SerializeField] achievementData[] achievements;
    [SerializeField] TMP_Text[] nameTexts;
    [SerializeField] TMP_Text[] descriptionTexts;
    [SerializeField] Color unlockedColor = new Color(1f, 0.84f, 0f);
    [SerializeField] Color lockedColor = Color.white;

    public void showAchievements()
    {
        for (int i = 0; i < achievements.Length; i++)
        {
            bool unlocked = achievementManager.instance.isUnlocked(achievements[i]);

            nameTexts[i].text =
                (unlocked ? "[UNLOCKED] " : "[LOCKED] ") + achievements[i].achievementName;

            descriptionTexts[i].text = achievements[i].description;

            if (unlocked)
            {
                nameTexts[i].color = unlockedColor;
                descriptionTexts[i].color = unlockedColor;
            }
            else
            {
                nameTexts[i].color = lockedColor;
                descriptionTexts[i].color = lockedColor;
            }
        }
    }
}