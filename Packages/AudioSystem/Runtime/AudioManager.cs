using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Marvin.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource soundSource;

        [Space]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string masterVolume;
        [SerializeField] private string musicVolume;
        [SerializeField] private string soundVolume;

        //Music
        private float disableTimer;
        private DateTime startDate;
        private DateTime currentDate;
        private float seconds;
        private float musicIsRuningCheckInterval = 0.1f;
        private float minFadeOutInSpeed = 0.001f;

        private string currentMusicList;
        private List<AudioObj> currentSongs = new List<AudioObj>();
        private int currentSongIndex;
        private MusicLoopType currentLoopType;

        private Coroutine fadeOutCoroutine;
        private Coroutine fadeInCoroutine;
        private Coroutine checkForNextSong;
        private Coroutine tuningMusicDown;
        private Coroutine tuningMusicUp;

        private bool musicSourceTunedDown;
        //Volume if music source is tuned down
        private float mainMusicSourceTuneDownVolume;

        [SerializeField] private AudioFiles[] musicFiles;

        [Header("Utiltiy")]
        [SerializeField] public AudioFiles[] utilitySounds;

        [Header("Player")]
        [SerializeField] public AudioFiles[] playerSounds;

        [Header("Enemy")]
        [SerializeField] public AudioFiles[] enemySounds;

        public enum MusicSongs
        {
            Empty,
            Menu,
            Level,
        }
        public enum UtilitySounds
        {
            MenuSelect,
        }
        public enum PlayerSounds
        {
            PlayerJump,
        }
        public enum EnemySounds
        {
            Attack,
        }

        public enum MusicLoopType
        {
            LoopSongsWithFadeInAndOut,
            LoopLastOrSingleSong,
            LoopLastSongWithFadeInAndOut,
            StopMusic,
            //LoopRandom,
        }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            if (PlayerPrefs.GetInt("AudioHasBeenChange") == 0)
            {
                PlayerPrefs.SetFloat("SliderValue" + masterVolume, 0.8f);

                PlayerPrefs.SetFloat("SliderValue" + musicVolume, 0.8f);

                PlayerPrefs.SetFloat("SliderValue" + soundVolume, 2.4f);
            }

            SetDecibel(PlayerPrefs.GetFloat("SliderValue" + masterVolume), masterVolume, 0);
            SetDecibel(PlayerPrefs.GetFloat("SliderValue" + musicVolume), musicVolume, 0);
            SetDecibel(PlayerPrefs.GetFloat("SliderValue" + soundVolume), soundVolume, 10);

        }
        private void SetDecibel(float sliderValue, string audioString, int maxDecibel)
        {
            float decibel = Mathf.Log10(sliderValue) * 20;

            audioMixer.SetFloat(audioString, decibel);
            bool gotvalue = audioMixer.GetFloat(audioString, out float soundvalue);

            if (gotvalue == true)
            {
                if (soundvalue > maxDecibel)
                {
                    Debug.Log(soundvalue);
                    audioMixer.SetFloat(audioString, maxDecibel);
                }
            }
            PlayerPrefs.SetFloat(audioString, decibel);
        }
        private void OnValidate()
        {
            SetNames(musicFiles, Enum.GetNames(typeof(MusicSongs)));
            SetNames(utilitySounds, Enum.GetNames(typeof(UtilitySounds)));
            SetNames(playerSounds, Enum.GetNames(typeof(PlayerSounds)));
            SetNames(enemySounds, Enum.GetNames(typeof(EnemySounds)));
        }
        public void SetNames(AudioFiles[] audioFiles, string[] enumNames)
        {
            for (int i = 0; i < enumNames.Length; i++)
            {
                if (audioFiles.Length - 1 < i) break;

                audioFiles[i].Name = enumNames[i];

            }
        }

        //Should be call when there is no audioSource on the obj and you don´t want to add multiple sounds to the script
        public void PlayAudioFileOneShot(AudioFiles file)
        {
            soundSource.PlayOneShot(file.audioObj.AudioClip, file.audioObj.Volume);
        }
        //Should be called when there is no audioSource on the obj and you add a audioObj to the script
        public void PlayAudioObjOneShot(AudioObj audioObj)
        {
            soundSource.PlayOneShot(audioObj.AudioClip, audioObj.Volume);
        }
        public void PlayRandomOneShot(AudioFiles[] files)
        {
            int randomNumber = UnityEngine.Random.Range(0, files.Length);

            if (soundSource != null) soundSource.PlayOneShot(files[randomNumber].audioObj.AudioClip, files[randomNumber].audioObj.Volume);
        }


        //Music
        public void SetMusic(MusicListObj musicList, float fadeOutSpeed, float fadeInSpeed, MusicLoopType loopType, bool ignoreMusicChangeIfSameClip = true)
        {
            //Music list checks
            if (musicList == null)
            {
                Debug.Log("Missing music list");
                return;
            }
            if (musicList.songs.Length <= 0)
            {
                Debug.Log("Music list is empty");
                return;
            }

            //if the music list is the same do nothing
            if (string.Compare(currentMusicList, musicList.name) == 0)
            {
                Debug.Log("Same music list");
                return;
            }

            currentMusicList = musicList.name;

            CancelCheckForNextSong();
            musicSource.loop = false;
            currentSongs.Clear();

            if(musicList.songs.Length > 0)
            {
                //add songs
                for (int i = 0; i < musicList.songs.Length; i++)
                {
                    currentSongs.Add(musicList.songs[i]);
                }
                currentSongIndex = 0;
                currentLoopType = loopType;

                StartMusicFadeOut(currentSongs[0], fadeOutSpeed, fadeInSpeed, ignoreMusicChangeIfSameClip);

                //start loop
                if(loopType == MusicLoopType.LoopLastOrSingleSong && musicList.songs.Length <= 1)
                {
                    musicSource.loop = true;
                }
                else
                {
                    if (checkForNextSong == null)
                    {
                        checkForNextSong = StartCoroutine(CheckForNextSong(fadeOutSpeed, fadeInSpeed));
                    }
                }
            }
            else
            {
                StopMusic(fadeOutSpeed);
            }
        }
        public void StopMusic(float fadeOutSpeed)
        {
            musicSource.loop = false;
            currentMusicList = string.Empty;
            CancelCheckForNextSong();
            CancelFadeOutCoroutine();
            CancelFadeInCoroutine();

            if(fadeOutCoroutine == null)
            {
                fadeOutCoroutine = StartCoroutine(FadeOutMusic(fadeOutSpeed, 0, true));
            }
        }

        private void StartMusicFadeOut(AudioObj audioObj, float fadeOutSpeed, float fadeInSpeed, bool ignoreMusicChangeIfSameClip)
        {
            AudioClip newSong = audioObj.AudioClip;

            //dont change music if the songs are the same
            if (ignoreMusicChangeIfSameClip == true && musicSource.clip == newSong)
            {
                if (newSong == null) return;

                //checking if the fadeOut with the same song allready is happening. If yes the fadeout is canceled and a new one will start
                if (fadeOutCoroutine == null) return;
            }

            CancelFadeOutCoroutine();
            CancelFadeInCoroutine();
            if (fadeOutCoroutine == null)
            {
                fadeOutCoroutine = StartCoroutine(FadeOutMusic(fadeOutSpeed, fadeInSpeed));
            }
        }
        private IEnumerator FadeOutMusic(float fadeOutSpeed, float fadeInSpeed, bool stopMusic = false)
        {
            if (fadeOutSpeed <= 0) fadeOutSpeed = minFadeOutInSpeed;
            if (fadeInSpeed <= 0) fadeInSpeed = minFadeOutInSpeed;

            float start = musicSource.volume;
            float targetVolume = 0;
            float duration = fadeOutSpeed;
            startDate = DateTime.Now;
            disableTimer = 0f;
            while (disableTimer < duration)
            {
                currentDate = DateTime.Now;
                seconds = currentDate.Ticks - startDate.Ticks;
                disableTimer = seconds * 0.0000001f;
                musicSource.volume = Mathf.Lerp(start, targetVolume, disableTimer / duration);
                yield return null;
            }

            if(stopMusic == false)
            {
                if (currentSongs.Count > 0)
                {
                    musicSource.clip = currentSongs[currentSongIndex].AudioClip;
                    musicSource.Play();

                    if(fadeInCoroutine == null) fadeInCoroutine = StartCoroutine(FadeInMusic(fadeInSpeed, 0));
                }
                else
                {
                    Debug.Log("No song available. Can this even happen?");
                    musicSource.clip = null;
                    CancelCheckForNextSong();
                }
            }
            else
            {
                musicSource.clip = null;
                musicSource.Stop();
            }

            CancelFadeOutCoroutine();
        }

        private IEnumerator FadeInMusic(float fadeInSpeed, float startVolume)
        {
            if (fadeInSpeed <= 0) fadeInSpeed = minFadeOutInSpeed;

            float duration = fadeInSpeed;
            float start = startVolume;
            startDate = DateTime.Now;
            disableTimer = 0f;
            while (disableTimer < duration)
            {
                currentDate = DateTime.Now;
                seconds = currentDate.Ticks - startDate.Ticks;
                disableTimer = seconds * 0.0000001f;
                if (musicSourceTunedDown == false)
                {
                    musicSource.volume = Mathf.Lerp(start, currentSongs[currentSongIndex].Volume, disableTimer / duration);
                }
                else 
                {
                    musicSource.volume = Mathf.Lerp(start, mainMusicSourceTuneDownVolume, disableTimer / duration);
                }

                //if tuneUpMainMusic or tuneDownMainMusic is call the fadeInMusic will be canceled so they dont fight each other
                if (tuningMusicUp != null || tuningMusicDown != null)
                {
                    disableTimer = duration;
                }
                yield return null;
            }
            CancelFadeInCoroutine();
            yield break;
        }

        private IEnumerator CheckForNextSong(float fadeOutSpeed, float fadeInSpeed)
        {
            while (true)
            {
                yield return new WaitForSeconds(musicIsRuningCheckInterval);

                if (musicSource.isPlaying == false)
                {
                    CancelFadeOutCoroutine();
                    CancelFadeInCoroutine();

                    SetCurrentMusicIndex();

                    if(fadeOutCoroutine == null) fadeOutCoroutine = StartCoroutine(FadeOutMusic(fadeOutSpeed, fadeInSpeed));
                }
            }
        }
        private void SetCurrentMusicIndex()
        {
            switch (currentLoopType)
            {
                case MusicLoopType.LoopSongsWithFadeInAndOut:
                    currentSongIndex++;
                    if (currentSongIndex > currentSongs.Count - 1)
                    {
                        currentSongIndex = 0;
                    }
                    break;

                case MusicLoopType.LoopLastOrSingleSong:
                    currentSongIndex++;
                    if (currentSongIndex >= currentSongs.Count - 1)
                    {
                        currentSongIndex = currentSongs.Count - 1;
                        musicSource.loop = true;
                        CancelCheckForNextSong();
                    }
                    break;

                case MusicLoopType.LoopLastSongWithFadeInAndOut:
                    if (currentSongIndex < currentSongs.Count - 1)
                    {
                        currentSongIndex++;
                    }
                    break;
            }
        }
        public void TuneDownMusicSource(float fadeOutSpeed, float musicSourceVolume)
        {
            if(tuningMusicDown == null && musicSourceTunedDown == false)
            {
                musicSourceTunedDown = true;
                mainMusicSourceTuneDownVolume = musicSourceVolume;
                tuningMusicDown = StartCoroutine(TuneDownMainMusicSource(fadeOutSpeed));
            }
        }
        IEnumerator TuneDownMainMusicSource(float fadeOutSpeed)
        {
            if (fadeOutSpeed <= 0) fadeOutSpeed = minFadeOutInSpeed;

            float duration = fadeOutSpeed;
            float start = musicSource.volume;
            float targetVolume = mainMusicSourceTuneDownVolume;
            startDate = DateTime.Now;
            disableTimer = 0f;
            while (disableTimer < duration)
            {
                currentDate = DateTime.Now;
                seconds = currentDate.Ticks - startDate.Ticks;
                disableTimer = seconds * 0.0000001f;

                if(fadeOutCoroutine == null) musicSource.volume = Mathf.Lerp(start, targetVolume, disableTimer / duration);

                yield return null;
            }
            CancelTuningMusicDown();
        }
        public void TuneUpMusicSource(float fadeInSpeed)
        {
            //needs to be reseted on gameover or switching scenes etc.

            CancelTuningMusicDown();
            CancelTuningMusicUp();

            if(tuningMusicUp == null) tuningMusicUp = StartCoroutine(TuneUpMainMusicSource(fadeInSpeed));
        }
        IEnumerator TuneUpMainMusicSource(float fadeInSpeed)
        {
            if (fadeInSpeed <= 0) fadeInSpeed = 0.001f;

            float duration = fadeInSpeed;
            float start = musicSource.volume;
            startDate = DateTime.Now;
            disableTimer = 0f;
            while (disableTimer < duration)
            {
                currentDate = DateTime.Now;
                seconds = currentDate.Ticks - startDate.Ticks;
                disableTimer = seconds * 0.0000001f;

                if (fadeOutCoroutine == null) musicSource.volume = Mathf.Lerp(start, currentSongs[currentSongIndex].Volume, disableTimer / duration);
                yield return null;
            }
            musicSourceTunedDown = false;
            CancelTuningMusicUp();
        }
        private void CancelCheckForNextSong()
        {
            if (checkForNextSong != null)
            {
                StopCoroutine(checkForNextSong);
                checkForNextSong = null;
            }
        }
        private void CancelFadeOutCoroutine()
        {
            if(fadeOutCoroutine != null)
            {
                StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = null;
            }
        }
        private void CancelFadeInCoroutine()
        {
            if (fadeInCoroutine != null)
            {
                StopCoroutine(fadeInCoroutine);
                fadeInCoroutine = null;
            }
        }
        private void CancelTuningMusicDown()
        {
            if(tuningMusicDown != null)
            {
                StopCoroutine(tuningMusicDown);
                tuningMusicDown = null;
            }
        }
        private void CancelTuningMusicUp()
        {
            if(tuningMusicUp != null)
            {
                StopCoroutine(tuningMusicUp);
                tuningMusicUp = null;
            }
        }
    }
    
    [Serializable]
    public struct AudioFiles
    {
        public string Name;
        public AudioObj audioObj;
    }
}
