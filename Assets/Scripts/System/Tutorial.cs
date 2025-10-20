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

    [SerializeField] private GameObject arrowCueHealth;
    [SerializeField] private GameObject arrowCueSoul;
    

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
        GameManager.Instance.showTutorial = false;


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

        //First Arrow 
        if (currentHint == 0)
        {
            arrowRect.anchoredPosition = new Vector2(-612f, -240f);
        }

        // Von FirstArrow durch ButtonRelease zum Item
        else if (currentHint == 1)
        {

            arrowRect.anchoredPosition = new Vector2(101.1f, 61.4f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, 15.6f);
        }

        //Von Item durch SelectObjectItem zum currentTower
        else if (currentHint == 2)
        {
            arrowRect.transform.position = towerPos + new Vector2(170f, 100f);


        }

        //Von currentTower durch SelectObjectTower zum Inventar   ------ hier könnte Arrow anim machen 
        else if (currentHint == 3)
        {

            arrowRect.anchoredPosition = new Vector2(-658.5f, 441.5f);
            arrowRect.localRotation = Quaternion.Euler(0f, 0f, -6.8f);
            
        }

        //Von Inventar durch ItemWurdeInsCraftedGelegt zum Modifying 
        else if (currentHint == 4)
        {

            arrowRect.anchoredPosition = new Vector2(-189.8f, 10f);

        }

        //Von Modifying durch GotIt! zur EnergyBar
        else if (currentHint == 5)
        {

            arrowRect.anchoredPosition = new Vector2(-308.2f, 90f);
        }

        //Von Energybar durch GotIt! zur Health und Souls Bar

        else if (currentHint == 6)
        {

            arrowCue.SetActive(false);


            arrowCueSoul.SetActive(true);
            RectTransform arrowSoulRect = arrowCueSoul.GetComponent<RectTransform>();
            arrowSoulRect.anchoredPosition = new Vector2(-663.5f, 112.8f); 

            arrowCueHealth.SetActive(true);
            RectTransform arrowHealthRect = arrowCueHealth.GetComponent<RectTransform>();
            arrowHealthRect.anchoredPosition = new Vector2(-189.2f, -181f);

        }

        //Von Health und Souls durch GotIt! zum Recipe
        else if (currentHint == 7)
        {
            arrowCueSoul.SetActive(false);
            arrowCueHealth.SetActive(false);

            arrowRect.anchoredPosition = new Vector2(711, 464.2f);
            arrowRect.localScale = new Vector3(-1, 1, 1);


        }


        else if (currentHint == 8)
        {

            arrowCue.SetActive(false);

        }


    }
}


