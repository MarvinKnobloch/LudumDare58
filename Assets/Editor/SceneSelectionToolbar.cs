using UnityEngine;
using UnityEditor.Overlays;
using UnityEditor;
using UnityEditor.Toolbars;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[Overlay(typeof(SceneView), "Scene Selection")]
//[Icon(icon)]
public class SceneSelectionToolbar : ToolbarOverlay
{
    //public const string icon = "Assets/Editor/Icons/WhiteTile.png";

    SceneSelectionToolbar() : base(SceneDropdownBar.id) { }


    [EditorToolbarElement(id, typeof(SceneView))]
    class SceneDropdownBar : EditorToolbarDropdownToggle, IAccessContainerWindow
    {
        public const string id = "SceneSelectionOverlay/SceneDropdownToggle";
        public EditorWindow containerWindow { get; set; }

        SceneDropdownBar()
        {
            text = "Scenes";
            tooltip = "Select a scene to load";
            //icon = AssetDatabase.LoadAssetAtPath<Texture2D>(SceneSelectionToolbar.icon);

            dropdownClicked += ShowSceneMenu;
        }

        private void ShowSceneMenu()
        {
            GenericMenu menu = new GenericMenu();

            Scene currentScene = EditorSceneManager.GetActiveScene();

            string[] allScenes = AssetDatabase.FindAssets("t:scene", new[] { "Assets/Scenes" });

            for (int i = 0; i < allScenes.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allScenes[i]);

                string name = Path.GetFileNameWithoutExtension(path);

                menu.AddItem(new GUIContent(name), string.Compare(currentScene.name, name) == 0, () => OpenScene(currentScene, path));
            }
            menu.ShowAsContext();    
        }

        private void OpenScene(Scene currentScene, string path)
        {
            if (currentScene.isDirty)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(path);
                }
            }
            else EditorSceneManager.OpenScene(path);
        }
    }
}