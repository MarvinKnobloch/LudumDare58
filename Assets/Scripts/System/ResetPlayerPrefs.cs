using Tower;
using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
    public void ResetPrefs()
    {
        for (int i = 0; i < GetComponent<Player>().towerRecipes.Length; i++)
        {
            PlayerPrefs.SetInt(GetComponent<Player>().towerRecipes[i].towerName, 0);
        }
    }
}
