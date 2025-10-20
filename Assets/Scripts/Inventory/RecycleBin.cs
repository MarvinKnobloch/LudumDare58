using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecycleBin : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image finalImage;
    [SerializeField] private Sprite[] images;
    [SerializeField] private float speed;
    private bool countUp;
    private int currentFrame;
    private bool end;

    private void Awake()
    {
        finalImage = GetComponent<Image>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateAnimation(true);
        IngameController.Instance.playerUI.inventory.canRecycle = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateAnimation(false);
        IngameController.Instance.playerUI.inventory.canRecycle = false;
    }

    private void UpdateAnimation(bool direction)
    {
        StopAllCoroutines();

        countUp = direction;
        StartCoroutine(Animate());
    }
    IEnumerator Animate()
    {
        end = false;
        int targetFrame = 0;
        if (countUp == true) targetFrame = images.Length - 1;
        while (end == false)
        {
            if (countUp == false)
            {
                currentFrame--;
                if (currentFrame >= 0)
                {
                    finalImage.sprite = images[currentFrame];
                }
                if (currentFrame <= targetFrame)
                {
                    end = true;
                }
            }
            else
            {
                currentFrame++; 
                if(currentFrame <= targetFrame)
                {
                    finalImage.sprite = images[currentFrame];
                }
                if (currentFrame >= targetFrame)
                {
                    end = true;
                }
            }
            if (end == true) break;

            yield return new WaitForSeconds(speed);
        }
    }
}
