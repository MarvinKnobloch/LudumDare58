using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] private Dialog[] dialog;
    private int currentDialogLine;

    [Header("DialogUI")]
    [SerializeField] private GameObject dialogWindow;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Image blackScreen;
    [SerializeField] private float fadeOutTime;

    private float timer;

    private void Start()
    {
        if (GameManager.Instance.showIntro == true)
        {
            GameManager.Instance.showIntro = false;
            dialogWindow.SetActive(true);
            blackScreen.gameObject.SetActive(true);
            NextDialog();
        }
    }

    public void NextDialog()
    {
        if (currentDialogLine < dialog.Length)
        {
            characterNameText.text = dialog[currentDialogLine].characterName;
            dialogText.text = dialog[currentDialogLine].dialogText;
            currentDialogLine++;
        }
        else
        {
            dialogWindow.SetActive(false);
            StartCoroutine(FadeOutBlackscreen());
        }
    }
    IEnumerator FadeOutBlackscreen()
    {
        blackScreen.raycastTarget = false;
        Color color = blackScreen.color;
        color.a = 1;
        blackScreen.color = color;
        blackScreen.gameObject.SetActive(true);
        timer = fadeOutTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float alpha = Mathf.Clamp01(timer /fadeOutTime);
            color = blackScreen.color;
            color.a = alpha;
            blackScreen.color = color;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void SkipDialog()
    {
        currentDialogLine = dialog.Length;
        NextDialog();
    }

    [Serializable]
    public struct Dialog
    {
        public string characterName;
        [TextArea (4, 4)] public string dialogText;
    }
}
