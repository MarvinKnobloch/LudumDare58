using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
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
#endif
