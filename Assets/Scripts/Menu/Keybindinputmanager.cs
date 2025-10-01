using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using TMPro;

public class Keybindinputmanager : MonoBehaviour
{
    private static Controls inputActions;

    public static event Action keyrebindfinished;                                    //f�r udatebinding UI
    public static event Action keyrebindcanceled;                                    //f�r udatebinding UI
    public static event Action disableCantClickLayer;

    public static Text keyrebindtext;

    public static Controls Controls
    {
        get
        {
            if (inputActions == null)
            {
                inputActions = new Controls();
            }
            return inputActions;
        }
    }

    public static void startrebind(string actionname, int bindingindex, TextMeshProUGUI textwhilerebind)
    {
        InputAction action = inputActions.asset.FindAction(actionname);
        if (action == null || action.bindings.Count <= bindingindex)
        {
            disableCantClickLayer?.Invoke();
            Debug.Log("action nicht vorhanden");
            return;
        }
        //if (action.bindings[bindingindex].isComposite)                                          // Composite = W/A/S/D mehrere hotkeys die gebindet werden m�ssen
        //{
        //    var firstpartindex = bindingindex + 1;
        //    if (firstpartindex < action.bindings.Count && action.bindings[firstpartindex].isPartOfComposite)
        //    {
        //        dorebind(action, firstpartindex, textwhilerebind, true);
        //    }
        //}
        //else
        {
            dorebind(action, bindingindex, textwhilerebind, false);

        }

    }
    private static void dorebind(InputAction actiontorebind, int bindingindex, TextMeshProUGUI textwhilerebind, bool isacomposite)
    {
        if (actiontorebind == null || bindingindex < 0)
            return;
        textwhilerebind.color = Color.green;
        textwhilerebind.text = "Choose button";                          // "Press a {actiontorebind.expectedControlType}";
        actiontorebind.Disable();                                       // der Hotkey wird disabled, wieso auch immer

        var rebind = actiontorebind.PerformInteractiveRebinding(bindingindex);              //ist eine Inputsystem function, startet den rebind prozess

        rebind.OnComplete(functionisruning =>                                             // wegen memoryleak
        {
            actiontorebind.Enable();
            functionisruning.Dispose();                                                   //wegen memoryleak

            InputBinding newbinding = actiontorebind.bindings[bindingindex];
            foreach (InputBinding binding in actiontorebind.actionMap.bindings)
            {
                if (binding.action == newbinding.action)
                {
                    inputActions.asset.FindBinding(binding, out InputAction action);
                    int newbindingindex = action.GetBindingIndex(binding);
                    inputActions.asset.FindBinding(newbinding, out InputAction oldaction);
                    int oldbindingindex = oldaction.GetBindingIndex(newbinding);
                    if (newbindingindex == oldbindingindex)
                    {
                        continue;
                    }
                }
                if (newbinding.effectivePath == binding.effectivePath)
                {
                    string overwritenbinding = binding.action;
                    if (binding.isPartOfComposite)
                    {
                        inputActions.asset.FindBinding(binding, out InputAction newaction);
                        int newbindingindex = newaction.GetBindingIndex(binding);
                        newaction.ApplyBindingOverride(newbindingindex, "");
                        savebindingsoverride(newaction);

                    }
                    else
                    {
                        bindingindex = 0;
                        InputAction newaction = inputActions.asset.FindAction(overwritenbinding);
                        newaction.ApplyBindingOverride(0, "");
                        savebindingsoverride(newaction);
                    }
                }
            }
            savebindingsoverride(actiontorebind);
            keyrebindfinished?.Invoke();
            disableCantClickLayer?.Invoke();
        });

        rebind.OnCancel(functionisruning =>                                             // wegen memoryleak
        {
            actiontorebind.Enable();
            functionisruning.Dispose();                                                   //l�scht alles was in der Funktion passiert damit keine speicherfehler im hintergrund entstehen

            keyrebindcanceled?.Invoke();
            disableCantClickLayer?.Invoke();
        });

        rebind.WithCancelingThrough("<Keyboard>/escape");

        rebind.Start();                                                               // startet die funktion rebind/macht das die rebind funktion st�ndig durchl�uft
    }
    public static string getbindingname(string actionname, int bindingindex)                           // f�r textupdate in spielmodus
    {
        if (inputActions == null)
        {
            inputActions = new Controls();
        }
        InputAction action = inputActions.asset.FindAction(actionname);
        return action.GetBindingDisplayString(bindingindex);
    }
    private static void savebindingsoverride(InputAction action)
    {
        var rebinds = action.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(action.name, rebinds);
    }
    public static void loadbindingsoverride(string actionname)
    {
        if (inputActions == null)                                           //falls start methode noch nicht gecalled worden ist
        {
            inputActions = new Controls();
        }
        InputAction action = inputActions.asset.FindAction(actionname);
        var rebinds = PlayerPrefs.GetString(actionname);
        action.LoadBindingOverridesFromJson(rebinds);
    }
    public static void resetbinding(string actionname, int bindingindex)
    {
        InputAction action = inputActions.asset.FindAction(actionname);

        if (action == null || action.bindings.Count <= bindingindex)
        {
            Debug.Log("reset binding fail");
            return;
        }
        if (action.bindings[bindingindex].isComposite)
        {
            for (int i = bindingindex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingindex);
        }
        InputBinding newbinding = action.bindings[bindingindex];
        foreach (InputBinding binding in action.actionMap.bindings)
        {
            if (binding.action == newbinding.action)
            {
                inputActions.asset.FindBinding(binding, out InputAction newaction);
                int newbindingindex = newaction.GetBindingIndex(binding);
                inputActions.asset.FindBinding(newbinding, out InputAction oldaction);
                int oldbindingindex = oldaction.GetBindingIndex(newbinding);
                if (newbindingindex == oldbindingindex)
                {
                    continue;
                }
            }
            if (newbinding.effectivePath == binding.effectivePath)
            {
                string overwritenbinding = binding.action;
                if (binding.isPartOfComposite)
                {
                    inputActions.asset.FindBinding(binding, out InputAction newaction);
                    int newbindingindex = newaction.GetBindingIndex(binding);
                    newaction.ApplyBindingOverride(newbindingindex, "");
                    savebindingsoverride(newaction);
                }
                else
                {
                    bindingindex = 0;
                    InputAction newaction = inputActions.asset.FindAction(overwritenbinding);
                    newaction.ApplyBindingOverride(0, "");
                    savebindingsoverride(newaction);
                }
            }
        }
        savebindingsoverride(action);
        keyrebindfinished?.Invoke();
    }
}
