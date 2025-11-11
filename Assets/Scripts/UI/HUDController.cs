using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboScoreText;
    [SerializeField] private TextMeshProUGUI FinalText;
    [SerializeField] private TextMeshProUGUI IntroText;
    [SerializeField] private TextMeshProUGUI TutoText;
    [SerializeField] private Image fillImage;

    private int displayedScore = 0;
    private int targetScore = 0;
    private Coroutine scoreCoroutine;

    [SerializeField] private float smoothSpeed = 5f;
    private Coroutine lerpStaminaRoutine;

    public void Start()
    {
        gameTimerText.text = "0:00";
        scoreText.text = displayedScore.ToString();
        comboScoreText.text = "0";
    }

    public void SetGameTimer(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);
        gameTimerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void SetTargetScore(int newScore)
    {
        targetScore = newScore;

        if (scoreCoroutine != null)
            StopCoroutine(scoreCoroutine);

        scoreCoroutine = StartCoroutine(IncrementScore());
    }

    private IEnumerator IncrementScore()
    {
        while (displayedScore < targetScore)
        {
            // increment displayedScore based on the difference to targetScore
            int difference = targetScore - displayedScore;
            displayedScore += Mathf.CeilToInt(difference / 10f);
            if (displayedScore > targetScore)
                displayedScore = targetScore;
            scoreText.text = displayedScore.ToString();
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetComboScore(int newScore)
    {
        comboScoreText.text = newScore.ToString();
    }

    public void SetFinalText(int newMaxCombo)
    {
        FinalText.text = "You reached your grave in " + gameTimerText.text + " with a maximum combo of " + newMaxCombo.ToString() + " and a total score of " + targetScore.ToString() + "!\nCongratulations!";
    }

    public void HideIntroText()
    {
        IntroText.gameObject.SetActive(false);
        TutoText.gameObject.SetActive(false);
    }

    public void SetStamina(float current, float max)
    {
        float targetFill = Mathf.Clamp01(current / max);

        if (lerpStaminaRoutine != null)
            StopCoroutine(lerpStaminaRoutine);

        lerpStaminaRoutine = StartCoroutine(SmoothFill(targetFill));
    }

    private IEnumerator SmoothFill(float target)
    {
        while (!Mathf.Approximately(fillImage.fillAmount, target))
        {
            fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, target, Time.deltaTime * smoothSpeed);
            yield return null;
        }

        fillImage.fillAmount = target;
        lerpStaminaRoutine = null;
    }
}