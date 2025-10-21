using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private RectTransform uiRectTransform;
    [SerializeField] private RectTransform showPositionTransform;
    [SerializeField] private RectTransform hidePositionTransform;
    [SerializeField] private float moveTime;
    private RectTransform endPosition;
    private float timer;

    public void ShowButton()
    {
        StopAllCoroutines();

        endPosition = showPositionTransform;
        timer = 0;
        nextLevelButton.enabled = true;
        StartCoroutine(Show());
    }
    IEnumerator Show()
    {
        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            uiRectTransform.anchoredPosition = Vector2.Lerp(uiRectTransform.anchoredPosition, endPosition.anchoredPosition, timer / moveTime);
            yield return null;
        }
    }

    public void HideButton()
    {
        StopAllCoroutines();

        endPosition = hidePositionTransform;
        timer = 0;
        nextLevelButton.enabled = false;
        StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            uiRectTransform.anchoredPosition = Vector2.Lerp(uiRectTransform.anchoredPosition, endPosition.anchoredPosition, timer / moveTime);
            yield return null;
        }
    }
}

