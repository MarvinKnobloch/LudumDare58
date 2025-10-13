using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    [SerializeField] private Image gameOverScreen;
    [SerializeField] private float fadeInTime;
    [SerializeField] private Button[] gameOverButtons;
    [SerializeField] private TextMeshProUGUI gameOverText;
    private float timer;
    private bool gameOverStarted;
    private float targetAlpha = 0.8f;

    private void Start()
    {
        foreach (Button button in gameOverButtons)
        {
            button.gameObject.SetActive(false);
        }
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
        IngameController.Instance.menuController.gameIsPaused = true;
        IngameController.Instance.SetGameOver();

        Color color = gameOverScreen.color;
        color.a = 0;
        gameOverScreen.color = color;
        gameOverScreen.gameObject.SetActive(true);

        while (gameOverScreen.color.a < targetAlpha)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeInTime);
            color = gameOverScreen.color;
            color.a = alpha;
            gameOverScreen.color = color;
            yield return null;
        }

        color = gameOverScreen.color;
        color.a = targetAlpha;
        gameOverScreen.color = color;

        foreach (Button button in gameOverButtons)
        {
            button.gameObject.SetActive(true);
        }

        Time.timeScale = 0;
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}