using UnityEngine;
using UnityEditor;
using System.IO;

public static class CreateScriptMenu 
{

    private static string savePathString = "/Editor/ScriptTemplates";

    [MenuItem("Assets/Scripts/Create MonoBehaviour")]
    private static void CreateMonoBehaviour()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Mono Behaviour", GetCurrentPath(), "NewMonoBehaviour", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/MonoBehaviourTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/Create ScriptableObject")]
    private static void CreateScriptableObject()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Scriptable Object", GetCurrentPath(), "NewScriptableObject", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/ScriptableObjectTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/Create Interface")]
    private static void CreateInterface()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Interface", GetCurrentPath(), "Interface", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/InterfaceTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/BasicClass/Create HealthScript")]
    private static void CreateHealthScript()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Value Script", GetCurrentPath(), "Value", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/HealthTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/BasicClass/Create UtilityClass")]
    private static void CreateUtilityClass()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Utility Class", GetCurrentPath(), "Utility", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/UtilityTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/BasicClass/Create GameOver")]
    private static void CreateGameOver()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Game Over", GetCurrentPath(), "GameOver", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/GameOverTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/BasicClass/Create InteractionField")]
    private static void CreateInteractionField()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Interaction Field", GetCurrentPath(), "InteractionField", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/InteractionFieldTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/BasicClass/Create TakeScreenshot")]
    private static void CreateTakeScreenshot()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Take Screenshot", GetCurrentPath(), "TakeScreenshot", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/TakeScreenshotTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/BasicClass/Create MoveComponentTool")]
    private static void CreateMoveComponentTool()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create MoveComponent Tool", GetCurrentPath(), "MoveComponentTool", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/MoveComponentToolTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    [MenuItem("Assets/Scripts/BasicClass/Create SceneSelectionToolbar")]
    private static void CreateSceneSelectionToolbar()
    {
        string pathToNewFile = EditorUtility.SaveFilePanel("Create Scene Selection Toolbar", GetCurrentPath(), "SceneSelectionToolbar", "cs");
        string pathToTemplate = Application.dataPath + savePathString + "/SceneSelectionToolbarTemplate.txt";

        CreateScriptFromTemplate(pathToNewFile, pathToTemplate);
    }

    private static void CreateScriptFromTemplate(string pathToNewFile, string pathToTemplate)
    {
        if (string.IsNullOrEmpty(pathToNewFile) == false)
        {
            FileInfo fileInfo = new FileInfo(pathToNewFile);
            string nameOfScript = Path.GetFileNameWithoutExtension(fileInfo.Name);

            string text = File.ReadAllText(pathToTemplate);

            //Basic Overrides
            text = text.Replace("#SCRIPTNAME#", nameOfScript);

            //ScriptableObject Overrides
            text = text.Replace("#FILENAME#", nameOfScript);
            text = text.Replace("#SCRIPTABLEOBJECTTYPE#", nameOfScript);

            File.WriteAllText(pathToNewFile, text);
            AssetDatabase.Refresh();
        }
    }
    private static string GetCurrentPath()
    {
        string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        if (path.Contains("."))
        {
            int index = path.LastIndexOf("/");
            path = path.Substring(0, index);
        }
        return path;
    }
}
