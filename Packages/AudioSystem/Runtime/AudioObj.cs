using UnityEngine;

namespace Marvin.AudioSystem
{
    [CreateAssetMenu(fileName = "AudioValues", menuName = "ScriptableObject/AudioValue")]
    public class AudioObj : ScriptableObject
    {
        public string Name;
        public AudioClip AudioClip;
        [Range(0f, 1f)]
        public float Volume = 0.3f;
        [Range(0f, 0.3f)]
        public float randomPitch;

        public void PlaySound(AudioSource audioSource)
        {
            float randomNumber = Random.Range(-randomPitch, randomPitch);
            audioSource.pitch = 1 + randomNumber;
            audioSource.PlayOneShot(AudioClip, Volume);
        }
    }
}

