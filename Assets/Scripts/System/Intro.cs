using System;
using System.Collections;
using JetBrains.Annotations;
using Marvin.AudioSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] private Dialog[] dialog;
    private int currentDialogLine;

    [Header("DialogUI")]
    [SerializeField] private GameObject dialogWindow;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Image introImage;
    [SerializeField] private float fadeOutTime;

    private float timer;

    private void Start()
    {
        if (GameManager.Instance.showIntro == true)
        {
            dialogWindow.SetActive(true);
            introImage.gameObject.SetActive(true);

            //Becaue of first sound
            dialogText.text = dialog[currentDialogLine].dialogText;
            currentDialogLine++;
            //NextDialog();
        }
    }

    public void NextDialog()
    {
        if (currentDialogLine < dialog.Length)
        {
            dialogText.text = dialog[currentDialogLine].dialogText;
            introImage.sprite = dialog[currentDialogLine].imageToDisplay;
            currentDialogLine++;
            AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilitySounds[(int)AudioManager.UtilitySounds.MenuSelect]);
        }
        else
        {
            GameManager.Instance.showIntro = false;
            AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilitySounds[(int)AudioManager.UtilitySounds.MenuSelect]);
            dialogWindow.SetActive(false);
            StartCoroutine(FadeOutBlackscreen());
        }
    }
    IEnumerator FadeOutBlackscreen()
    {
        introImage.raycastTarget = false;
        Color color = introImage.color;
        color.a = 1;
        introImage.color = color;
        introImage.gameObject.SetActive(true);
        timer = fadeOutTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float alpha = Mathf.Clamp01(timer /fadeOutTime);
            color = introImage.color;
            color.a = alpha;
            introImage.color = color;
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
        public Sprite imageToDisplay;
    }
}
