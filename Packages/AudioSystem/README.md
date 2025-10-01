Create an empty Gameobject and add the AudioManager to it
Create two childs and add a AudioSources to it (one MusicSource, one SoundEffectSource)
Assign them to the AudioManager.

Ways to play sound:

1. Add an audioObj to your script and a AudioSource to your gameobject - AudioSource.PlayOneShot(audioObj.AudioClip, audioObj.Volume);
2. Add an audioObj to your script and call - AudioManager.Instance.PlayAudioObjOneShot(audioObj);
3. Use the Script PlayAudioOnSpawn
4. Add the sound the AudioManager and call it directly - AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilitySounds[(int)AudioManager.UtilitySounds.MenuSelect]);
