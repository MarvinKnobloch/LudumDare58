using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialHints;
    [SerializeField] private GameObject skipTutorialButton;
    [SerializeField] private GameObject startRoundButton;


    [SerializeField] private GameObject BuildButton;

    [SerializeField] private GameObject arrowCue;
    [SerializeField] private Vector2[] arrowPositions;

    

    private int currentHint;
    public Vector2 towerPos;


    private void Start()
    {
        if (GameManager.Instance.showTutorial)
        {
            StartTutorial();
        }

    }


    private void StartTutorial()
    {
        skipTutorialButton.SetActive(true);
        tutorialHints[currentHint].SetActive(true);
        StartCoroutine(HideNextRoundButton());


        ShowArrowForCurrentHint();
    }

    IEnumerator HideNextRoundButton()
    {
        yield return null;
        startRoundButton.SetActive(false);
    }

    public void NextHint()
    {
        tutorialHints[currentHint].SetActive(false);
        currentHint++;

        if (currentHint > tutorialHints.Length - 1)
        {
            GameManager.Instance.showTutorial = false;

            skipTutorialButton.SetActive(false);
            startRoundButton.SetActive(true);

            arrowCue.SetActive(false);
        }
        else
        {
            tutorialHints[currentHint].SetActive(true);
            ShowArrowForCurrentHint();
        }
    }

    public void SkipTutorial()
    {
        skipTutorialButton.SetActive(false);
        tutorialHints[currentHint].SetActive(false);
        startRoundButton.SetActive(true);
        arrowCue.SetActive(false);
    }




    public void TryAdvanceHint(int expectedHint)
    {
        if (currentHint == expectedHint)
            NextHint();
    }







    private void ShowArrowForCurrentHint()
    {
        if (GameManager.Instance.showTutorial == false) return;

        arrowCue.SetActive(true);
        RectTransform arrowRect = arrowCue.GetComponent<RectTransform>();


        if (currentHint == 0)
        {
            arrowRect.anchoredPosition = new Vector2(-612f, -240f);
        }


        else if (currentHint == 1)
        {
       
            arrowRect.anchoredPosition = new Vector2(101.1f, 61.4f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, 15.6f);
        }

        //Tower Select
        else if (currentHint == 2)
        {

            arrowRect.transform.position = towerPos + new Vector2(170f, 100f);


        }

        //DragItem to Craft
        else if (currentHint == 3)
        {

            arrowRect.anchoredPosition = new Vector2(-641.5f, 467.5f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, 6.8f);

        }

        else if (currentHint == 4)
        {

            arrowRect.anchoredPosition = new Vector2(573.84f, 500.7f);
        }


        else if (currentHint == 5)
        {

            arrowRect.anchoredPosition = new Vector2(573.84f, 500.7f);
        }


        else if (currentHint == 6)
        {

            arrowRect.anchoredPosition = new Vector2(573.84f, 500.7f);
        }


        else if (currentHint == 7)
        {
            arrowRect.anchoredPosition = new Vector2(573.84f, 500.7f);
        }


        else if (currentHint == 8)
        {
            arrowRect.anchoredPosition = new Vector2(573.84f, 500.7f);
        }


    }
}
