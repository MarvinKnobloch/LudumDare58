using UnityEditor.Overlays;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Marvin.SceneViewSystem;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Marvin.SceneViewSystem
{
    [Overlay(typeof(SceneView), "SceneView")]
    public class SVToolbar : Overlay
    {
        private int baseHeight = 67;
        private int addedElements;
        public override VisualElement CreatePanelContent()
        {
            addedElements = 0;
            VisualElement root = new VisualElement();
            root.style.width = 150;

            Label label = new Label("SceneViews");
            root.Add(label);

            Scene scene = EditorSceneManager.GetActiveScene();
            SVDirectory directory = SVDirectory.Find(scene);

            if (directory != null)
            {
                for (int i = 0; i < directory.bookmarks.Length; i++)
                {
                    string name = directory.bookmarks[i].sceneViewName;

                    SVBookmark? bookmark = directory.GetBookmark(i);
                    if (bookmark.HasValue && string.IsNullOrEmpty(name) == false)
                    {
                        Button viewButton = new Button();
                        viewButton.style.scale = new Vector3(1, 1, 1);
                        viewButton.text = name;
                        viewButton.clickable.clicked += () => SwitchSceneView(bookmark);
                        addedElements++;

                        root.Add(viewButton);
                    }
                }
            }

            //addButton
            Button addButton = new Button();
            addButton.style.scale = new Vector3(1, 1, 1);
            addButton.text = ("Add View");
            addButton.style.translate = new Vector3(0, 30, 0);
            addButton.clickable.clicked += () => SVTool.AddMenuItem(this);

            root.Add(addButton);

            root.style.height = baseHeight + (addedElements * 20.9f);

            return root;
        }
        private void SwitchSceneView(SVBookmark? bookmark)
        {
            if (bookmark.HasValue)
            {
                bookmark.Value.SetSceneViewOrientation();
            }
        }
    }
}