using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("FrameRate")]
    [SerializeField] private bool capFrameRate;
    [SerializeField] private int targetFrameRate;

    [Header("GameSettings")]
    public bool showIntro;
    public bool showTutorial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        QualitySettings.vSyncCount = 0;
        if (capFrameRate) Application.targetFrameRate = targetFrameRate;
    }
}
