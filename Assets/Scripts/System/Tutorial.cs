using Unity.VisualScripting;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialHints;
    [SerializeField] private GameObject skipTutorialButton;
    [SerializeField] private GameObject startRoundButton;
    private int currrentHint;

    private void Start()
    {
        if (GameManager.Instance.showTutorial == true)
        {
            GameManager.Instance.showTutorial = false;
            StartTutorial();
        }
    }
    private void StartTutorial()
    {
        startRoundButton.SetActive(false);
        skipTutorialButton.SetActive(true);
        tutorialHints[currrentHint].SetActive(true);
    }
    public void NextHint()
    {
        tutorialHints[currrentHint].SetActive(false);
        currrentHint++;
        if(currrentHint > tutorialHints.Length - 1)
        {
            skipTutorialButton.SetActive(false);
            startRoundButton.SetActive(true);
        }
        else
        {
            tutorialHints[currrentHint].SetActive(true);
        }
    }
    public void SkipTutorial()
    {
        skipTutorialButton.SetActive(false);
        tutorialHints[currrentHint].SetActive(false);
        startRoundButton.SetActive(true);
    }
}
