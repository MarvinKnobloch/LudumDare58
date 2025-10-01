using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
//using Marvin.AudioSystem;

public class Audioslider : MonoBehaviour
{
    public AudioMixer audioMixer;
    public string volumeString;
    private Slider slider;

    [SerializeField] private TextMeshProUGUI sliderText;

    private bool skipFirstSound;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        skipFirstSound = true;
    }
    private void OnEnable()
    {
        slider.value = PlayerPrefs.GetFloat("SliderValue" + volumeString);
        sliderText.text = Mathf.Round(slider.normalizedValue * 100).ToString();
    }
    public void MasterValuechange(float slidervalue)
    {
        float textvalue = slidervalue;
        sliderText.text = Mathf.Round(slider.normalizedValue * 100).ToString();

        SetDecibel(slidervalue, volumeString, 0);

        if (skipFirstSound == true)
        {
            skipFirstSound = false;
            return;
        }
        //else AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilitySounds[(int)AudioManager.UtilitySounds.MenuSelect]);
    }
    public void MusicValueChange(float slidervalue)
    {
        float textvalue = slidervalue;
        sliderText.text = Mathf.Round(slider.normalizedValue * 100).ToString();

        SetDecibel(slidervalue, volumeString, 0);
    }
    public void SoundEffectValueChange(float slidervalue)
    {
        float textvalue = slidervalue;
        sliderText.text = Mathf.Round(slider.normalizedValue * 100).ToString();

        SetDecibel(slidervalue, volumeString, 10);

        if(skipFirstSound == true)
        {
            skipFirstSound = false;
            return;
        }
        //else AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilitySounds[(int)AudioManager.UtilitySounds.MenuSelect]);
    }
    private void SetDecibel(float sliderValue, string audioString, int maxDecibel)
    {
        PlayerPrefs.SetInt("AudioHasBeenChange", 1);
        PlayerPrefs.SetFloat("SliderValue" + audioString, sliderValue);

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
}
