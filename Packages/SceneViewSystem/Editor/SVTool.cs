using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Marvin.SceneViewSystem
{
    public class SVTool : EditorWindow
    {
        const string menuPath = "Marvin/SceneViewBookmarks/";

        static int bookmarkIndex = 0;

        private static int createWindowWidth = 350;
        private static int createWindowHeight = 120;

        private string textFieldValue = "";

        private Rect buttonSize = new Rect(10, 50, 150, 65);

        private static SVToolbar currentToolbar;

        //[MenuItem(menuPath + "Add")]
        //public static void AddMenuItem()
        //{
        //    Scene scene = EditorSceneManager.GetActiveScene();
        //    SWDirectory directory = SWDirectory.Find(scene);

        //    if (directory == null)
        //    {
        //        directory = SWDirectory.Create(scene);
        //    }

        //    directory.AddBookmark(SWBookmark.CreateBookmark());
        //}    

        //[MenuItem(menuPath + "Add")]
        public static void AddMenuItem(SVToolbar toolbar)
        {
            SVTool window = EditorWindow.GetWindow<SVTool>("Scene View");
            window.maxSize = new Vector2(createWindowWidth, createWindowHeight);
            window.minSize = new Vector2(createWindowWidth, createWindowHeight);

            currentToolbar = toolbar;
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);

            textFieldValue = EditorGUILayout.TextField("Scene View Name", textFieldValue);

            if (GUI.Button(new Rect(buttonSize), "Confirm"))
            {
                if (string.IsNullOrWhiteSpace(textFieldValue) == false)
                {
                    Scene scene = EditorSceneManager.GetActiveScene();
                    SVDirectory directory = SVDirectory.Find(scene);

                    if (directory == null)
                    {
                        directory = SVDirectory.Create(scene);
                    }

                    directory.AddBookmark(SVBookmark.CreateBookmark(textFieldValue));
                    currentToolbar.RefreshPopup();

                    Close();
                }
            }

            if (GUI.Button(new Rect(buttonSize.x + 180, buttonSize.y, buttonSize.width, buttonSize.height), "Cancel"))
            {
                Close();
            }
        }


        [MenuItem(menuPath + "Switch _4")]
        public static void SwitchMenuItem()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            SVDirectory directory = SVDirectory.Find(scene);

            if (directory != null)
            {
                SVBookmark? bookmark = directory.GetBookmark(bookmarkIndex);
                if (bookmark.HasValue)
                {
                    bookmark.Value.SetSceneViewOrientation();

                    bookmarkIndex++;
                    if (bookmarkIndex >= directory.Count)
                        bookmarkIndex = 0;
                }
            }
        }
        [InitializeOnLoadMethod]
        static void Initilaze()
        {
            EditorSceneManager.sceneOpened += OnSceneOpen;
        }
        private static void OnSceneOpen(Scene scene, OpenSceneMode mode)
        {
            bookmarkIndex = 0;
        }
    }

    [Serializable]
    public struct SVBookmark
    {
        public string sceneViewName;
        public Vector3 pivot;
        public Quaternion rotation;
        public float size;
        public bool isOrthographic;

        public static SVBookmark CreateBookmark(string name)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            SVBookmark bookmark = new SVBookmark()
            {
                sceneViewName = name,
                pivot = sceneView.pivot,
                rotation = sceneView.rotation,
                size = sceneView.size,
                isOrthographic = sceneView.orthographic
            };
            return bookmark;
        }

        public void SetSceneViewOrientation()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            sceneView.pivot = pivot;
            sceneView.size = size;
            sceneView.orthographic = isOrthographic;
            if (sceneView.in2DMode == false) sceneView.rotation = rotation;
        }
    }
}