using UnityEngine;

namespace Marvin.AudioSystem
{
    public class SceneMusic : MonoBehaviour
    {
        [SerializeField] private MusicListObj musicList;


        [SerializeField] [Range(0.001f, 5)] private float fadeOutSpeed = 0.1f;
        [SerializeField] [Range(0.001f, 5)] private float fadeInSpeed = 1;

        public AudioManager.MusicLoopType loopType;

        void Start()
        {
            if (loopType == AudioManager.MusicLoopType.StopMusic)
            {
                AudioManager.Instance.StopMusic(fadeOutSpeed);
            }
            else
            {
                AudioManager.Instance.SetMusic(musicList, fadeOutSpeed, fadeInSpeed, loopType);
            }
        }
    }
}
