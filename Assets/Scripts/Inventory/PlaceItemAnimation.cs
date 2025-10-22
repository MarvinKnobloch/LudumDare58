using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class PlaceItemAnimation : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] images;
    [SerializeField] private float speed;
    private int currentFrame;

    private void OnEnable()
    {
        StopAllCoroutines();

        currentFrame = 0;
        image.sprite = images[currentFrame];

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        while (currentFrame < images.Length - 1)
        {
            yield return new WaitForSeconds(speed);
            currentFrame++;
            image.sprite = images[currentFrame];
        }
        StopAllCoroutines();

        gameObject.SetActive(false);
    }
}

