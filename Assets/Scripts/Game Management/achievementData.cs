using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievements/Achievement")]
public class achievementData : ScriptableObject
{
    public string achievementID;
    public string achievementName;

    [TextArea]
    public string description;
}