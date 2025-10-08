using UnityEngine;

public class FloatingNumberController : MonoBehaviour
{
    [SerializeField] private GameObject floatingNumber;

    public void displaynumber(Vector3 spawnPosition, int numberToDisplay, Color colorToDisplay)
    {
        if (transform.GetChild(0).gameObject.activeSelf == false)
        {
            transform.GetChild(0).gameObject.GetComponent<FloatingNumber>().SetFloatingNumber(spawnPosition, numberToDisplay, colorToDisplay);
        }
        else
        {
            FloatingNumber newFloatingNumber =
               Instantiate(floatingNumber, spawnPosition + Vector3.up * 2, Quaternion.identity, transform).GetComponent<FloatingNumber>();
            newFloatingNumber.SetFloatingNumber(spawnPosition, numberToDisplay, colorToDisplay);
        }
    }
}
