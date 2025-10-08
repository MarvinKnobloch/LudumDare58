using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class IngameController : MonoBehaviour
{
    public static IngameController Instance;

    [Header("Menus")]
    public MenuController menuController;
    public PlayerUI playerUI;
    public FloatingNumberController floatingNumberController;

    [Header("Other")]
    [SerializeField] private bool disableCursorIngame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {

    }
    public void ActivateCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void DeactivateCursor()
    {
        if(disableCursorIngame == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
