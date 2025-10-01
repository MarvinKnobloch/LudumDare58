using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public static class TakeScreenshot
{
    public const string editorPref = "CaptureScreenshot";
    public const string windowName = "Marvin/TakeScreenshot";

    [MenuItem(windowName + "_F11")]
    public static void CreateScreenshot()
    {
        string path = EditorPrefs.GetString(editorPref);
        if (string.IsNullOrWhiteSpace(path))
        {
            path = GetDefaultPath();
        }

        if(Directory.Exists(path) == false)
        {
            Debug.Log("Can�t save screenshot, path doesnt exist");
        }
        else
        {
            string filename = Path.Combine(path, string.Format("{0}_{1}.png", Application.productName, DateTime.Now.ToString("yyyymmddhhmmss")));
            ScreenCapture.CaptureScreenshot(filename, 1);

            Debug.Log("Captured screenshot at " + path);
        }
    }

    public static string GetDefaultPath()
    {
        string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        return defaultPath;
    }
}

public class CaptureScreenshotSettings : SettingsProvider
{
    public CaptureScreenshotSettings(string path, SettingsScope scope = SettingsScope.User) : base(path, scope)
    { }
    public override void OnGUI(string searchContext)
    {
        base.OnGUI(searchContext);

        string path = EditorPrefs.GetString(TakeScreenshot.editorPref);

        if (string.IsNullOrWhiteSpace(path))
        {
            path = TakeScreenshot.GetDefaultPath();
        }

        string changedPath = EditorGUILayout.TextField(path);
        if (string.Compare(path, changedPath) != 0)
        {
            EditorPrefs.SetString(TakeScreenshot.editorPref, changedPath);
        }

        if (GUILayout.Button("Reset to Default", GUILayout.Width(200)))
        {
            EditorPrefs.DeleteKey(TakeScreenshot.editorPref);
            Repaint();
        }
    }
    [SettingsProvider]
    public static SettingsProvider CreateScreenshotSettings()
    {
        CaptureScreenshotSettings captureScreenshotSettings = new CaptureScreenshotSettings(TakeScreenshot.windowName);
        return captureScreenshotSettings;
    }
}