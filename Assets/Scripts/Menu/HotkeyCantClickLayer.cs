using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HotkeyCantClickLayer : MonoBehaviour
{
    public GameObject cantClickLayer;

    private float disableTime = 0.05f;
    private float disableTimer;

    private DateTime startDate;
    private DateTime currentDate;
    private float seconds;
    private void OnEnable()
    {
        Keybindinputmanager.disableCantClickLayer += startCountdown;
    }
    private void OnDisable()
    {
        Keybindinputmanager.disableCantClickLayer -= startCountdown;
    }

    private void startCountdown()
    {
        StartCoroutine("disableLayer");
    }
    IEnumerator disableLayer()
    {
        startDate = DateTime.Now;
        disableTimer = 0f;
        while (disableTimer < disableTime)
        {
            currentDate = DateTime.Now;
            seconds = currentDate.Ticks - startDate.Ticks;
            disableTimer = seconds * 0.0000001f;
            yield return null;
        }
        cantClickLayer.SetActive(false);
    }
}
