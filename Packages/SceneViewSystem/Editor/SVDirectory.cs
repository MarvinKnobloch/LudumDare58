using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Marvin.SceneViewSystem
{
    public class SVDirectory : ScriptableObject
    {
        public string sceneGuid = null;
        public SVBookmark[] bookmarks = null;

        public int Count
        {
            get { return bookmarks == null ? 0 : bookmarks.Length; }
        }

        public static SVDirectory Create(Scene scene)
        {
            SVDirectory directory = ScriptableObject.CreateInstance<SVDirectory>();

            string sceneGuid = AssetDatabase.AssetPathToGUID(scene.path);
            directory.sceneGuid = sceneGuid;

            string sceneName = Path.GetFileNameWithoutExtension(scene.path);
            string path = scene.path.Substring(0, scene.path.Length - Path.GetFileName(scene.path).Length);
            path = Path.Combine(path, sceneName + "Bookmarks.asset");

            AssetDatabase.CreateAsset(directory, path);

            return directory;
        }

        public static SVDirectory Find(Scene scene)
        {
            string sceneGuid = AssetDatabase.AssetPathToGUID(scene.path);

            foreach (string directoryGuid in AssetDatabase.FindAssets("t:SVDirectory"))
            {
                string pathToAsset = AssetDatabase.GUIDToAssetPath(directoryGuid);
                SVDirectory directory = AssetDatabase.LoadAssetAtPath<SVDirectory>(pathToAsset);
                if (directory.sceneGuid == sceneGuid)
                {
                    return directory;
                }
            }
            return null;
        }

        public SVBookmark? GetBookmark(int index)
        {
            if (bookmarks == null || bookmarks.Length <= index) return null;

            return bookmarks[index];
        }

        public void AddBookmark(SVBookmark bookmark)
        {
            if (bookmarks == null)
            {
                bookmarks = new SVBookmark[1];
                bookmarks[0] = bookmark;
            }
            else
            {
                ArrayUtility.Add<SVBookmark>(ref bookmarks, bookmark);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }
}