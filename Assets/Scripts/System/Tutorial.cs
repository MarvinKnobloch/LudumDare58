using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialHints;
    [SerializeField] private GameObject skipTutorialButton;
    [SerializeField] private NextLevelButton startRoundButton;


    [SerializeField] private GameObject BuildButton;

    [SerializeField] private GameObject arrowCue;
    

    [SerializeField] private GameObject arrowCueHealth;
    [SerializeField] private GameObject arrowCueSoul;
    

    public int currentHint;
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

        ShowArrowForCurrentHint();
    }

    public void NextHint()
    {
        tutorialHints[currentHint].SetActive(false);
        currentHint++;

        if (currentHint > tutorialHints.Length - 1)
        {
            GameManager.Instance.showTutorial = false;

            skipTutorialButton.SetActive(false);
            startRoundButton.ShowButton();

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
        GameManager.Instance.showTutorial = false;


        skipTutorialButton.SetActive(false);
        tutorialHints[currentHint].SetActive(false);
        startRoundButton.ShowButton();
        arrowCue.SetActive(false);
        arrowCueHealth.SetActive(false);
        arrowCueSoul.SetActive(false);
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
            Debug.Log("Hint 0");
            arrowCue.SetActive(false);
        }

        if (currentHint == 1)
        {
            Debug.Log("Hint 1");
            arrowCue.SetActive(true);
            arrowRect.anchoredPosition = new Vector2(-649.3f, -287.9f);
        }

        else if (currentHint == 2)
        {
            Debug.Log("Hint 2");

            arrowRect.anchoredPosition = new Vector2(100.1f, 71.4f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, 30.6f);
        }

        else if (currentHint == 3)
        {

            Debug.Log("Hint 3");
            arrowRect.transform.position = towerPos + new Vector2(170f, 100f);


        }

        else if (currentHint == 4)
        {

            arrowRect.anchoredPosition = new Vector2(-658.5f, 441.5f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, -6.8f);

        }

        else if (currentHint == 5)
        {

            arrowRect.anchoredPosition = new Vector2(-189.8f, 10f);

        }

        else if (currentHint == 6)
        {

            arrowRect.anchoredPosition = new Vector2(-308.2f, 90f);
        }


        else if (currentHint ==7)
        {

            arrowCue.SetActive(true);

            arrowRect.anchoredPosition = new Vector2(711, 464.2f);
            arrowRect.localScale = new Vector3(-1, 1, 1);


        }

       
        else if (currentHint == 8)
        {
       
            arrowCue.SetActive(false);
            arrowCueHealth.SetActive(true);
            RectTransform arrowHealthRect = arrowCueHealth.GetComponent<RectTransform>();
            arrowHealthRect.anchoredPosition = new Vector2(-189.2f, -181f);

        }

        else if (currentHint == 9)
        {


            arrowCue.SetActive(false);
            arrowCueHealth.SetActive(false);


            arrowCueSoul.SetActive(true);
            RectTransform arrowSoulRect = arrowCueSoul.GetComponent<RectTransform>();
            arrowSoulRect.anchoredPosition = new Vector2(-670f, 185.6f);
        }



        else if (currentHint == 10)
        {

            arrowCueSoul.SetActive(false);
            arrowCueHealth.SetActive(false);
            arrowCue.SetActive(false);

        }

         


    }
}


