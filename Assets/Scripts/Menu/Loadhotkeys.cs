using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Loadhotkeys : MonoBehaviour
{
    [SerializeField] private InputActionReference inputActionReference;
    [Range(0, 9)]
    [SerializeField] private int selectedBinding;
    [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;
    [SerializeField] private InputBinding inputBinding;
    private string actionname;
    private int bindingindex;

    private void OnEnable()
    {
        if (inputActionReference != null)
        {
            getbindinginfo();
            Keybindinputmanager.loadbindingsoverride(actionname);
        }
        gameObject.SetActive(false);
    }
    private void OnValidate()                                                           // Wird außerhalb vom Playmodusgecalled, immer dann wenn ein Wert im Inspector geändert wird
    {
        if (inputActionReference == null)
            return;
        getbindinginfo();
    }
    private void getbindinginfo()
    {
        if (inputActionReference.action != null)
        {
            actionname = inputActionReference.action.name;                              // durchsucht die Hotkeys/Actionen und setzt dann String 
        }
        if (inputActionReference.action.bindings.Count > selectedBinding)              // eher unwichtig, weil ich für jede Action nur ein hotkey habe
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingindex = selectedBinding;
        }
    }
}
