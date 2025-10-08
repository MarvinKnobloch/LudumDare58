using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResetPlayerPrefs))]
public class ResetPlayerPrefsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ResetPlayerPrefs resetPlayerPrefs = (ResetPlayerPrefs)target;
        if (GUILayout.Button("ResetRecipePrefs"))
        {
            resetPlayerPrefs.ResetPrefs();
        }
    }
}
