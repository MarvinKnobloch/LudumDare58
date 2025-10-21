using UnityEngine;
using UnityEngine.UI;

public class SetBackgroundRenderer : MonoBehaviour
{
    void Start()
    {
        if(LevelManager.Instance != null) LevelManager.Instance.backgroundRenderer = GetComponent<SpriteRenderer>();
    }
}
