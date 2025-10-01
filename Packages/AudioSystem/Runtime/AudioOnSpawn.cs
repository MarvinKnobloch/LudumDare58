using UnityEditor;
using UnityEngine;

namespace Marvin.AudioSystem
{
    public class AudioOnSpawn : MonoBehaviour
    {
        [SerializeField] private AudioObj audioObj;
        private AudioSource audioSource;

        public AudioPlayMode audioPlayMode;
        public enum AudioPlayMode
        {
            OnEnable,
            OnStart,
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        void Start()
        {
            if (audioPlayMode == AudioPlayMode.OnStart) PlayAudio();
        }
        private void OnEnable()
        {
            if (audioPlayMode == AudioPlayMode.OnEnable) PlayAudio();
        }

        private void PlayAudio()
        {
            if (audioSource != null)
            {
                float randomNumber = Random.Range(-audioObj.randomPitch, audioObj.randomPitch);
                audioSource.pitch = 1 + randomNumber;
                audioSource.PlayOneShot(audioObj.AudioClip, audioObj.Volume);
            }
            else
            {
                AudioManager.Instance.PlayAudioObjOneShot(audioObj);
            }
        }
    }
}
