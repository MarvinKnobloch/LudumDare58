using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class FloatingNumber : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float uptime = 1;
    [SerializeField] private float heightOffset = 2;
    [SerializeField] private float randomOffset = 1f;
    [SerializeField] private TextMeshPro numberText;

    private void OnEnable()
    {
        Invoke("turnoff", uptime);
    }
    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }
    private void turnoff()
    {
        gameObject.transform.SetAsFirstSibling();
        gameObject.SetActive(false);
    }
    public void SetFloatingNumber(Vector3 spawnPosition, int numberToDisplay, Color colorToDisplay)
    {
        transform.position = spawnPosition + Vector3.up * heightOffset;
        transform.rotation = Quaternion.identity;
        transform.localPosition += new Vector3(Random.Range(-randomOffset, randomOffset), Random.Range(-randomOffset, randomOffset), 0);
        //transform.Rotate(0, 0, 0);
        numberText.text = numberToDisplay.ToString();
        numberText.color = colorToDisplay;
        gameObject.SetActive(true);
        gameObject.transform.SetAsLastSibling();
    }
}
