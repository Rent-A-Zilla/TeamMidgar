using UnityEngine;
using TMPro;

public class fallingPlatformTimerUI : MonoBehaviour
{
    public TMP_Text timerParentText;
    public TMP_Text timerNumberText;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Start()
    {
        HideTimer();
    }

    public void ShowTimer(float timeLeft)
    {
        if (timerParentText != null)
        {
            timerParentText.text = "Platform falling in:";
        }

        if (timerNumberText != null)
        {
            timerNumberText.text = timeLeft.ToString("F1");
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void HideTimer()
    {
        if (timerNumberText != null)
        {
            timerNumberText.text = "";
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}