using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Victory : MonoBehaviour
{
    [SerializeField] private Image victoryScreen;
    [SerializeField] private float fadeInTime;
    [SerializeField] private TextMeshProUGUI gameOverText;
    private float timer;
    private bool gameOverStarted;
    private float targetAlpha = 0.8f;

    private void Start()
    {
        StartGameOver();
    }
    public void StartGameOver()
    {
        if (gameOverStarted) return;

        StartCoroutine(FadeInGameOver());
    }
    IEnumerator FadeInGameOver()
    {
        gameOverStarted = true;

        Color color = victoryScreen.color;
        color.a = 0;
        victoryScreen.color = color;
        victoryScreen.gameObject.SetActive(true);

        while (victoryScreen.color.a < targetAlpha)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeInTime);
            color = victoryScreen.color;
            color.a = alpha;
            victoryScreen.color = color;
            yield return null;
        }

        color = victoryScreen.color;
        color.a = targetAlpha;
        victoryScreen.color = color;
    }
}
