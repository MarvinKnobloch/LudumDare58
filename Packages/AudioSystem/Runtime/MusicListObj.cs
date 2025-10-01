using UnityEngine;

namespace Marvin.AudioSystem
{
    [CreateAssetMenu(fileName = "MusicList", menuName = "ScriptableObject/MusicList")]
    public class MusicListObj : ScriptableObject
    {
        public AudioObj[] songs;
    }
}
