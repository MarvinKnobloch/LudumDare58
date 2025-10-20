using System.Collections.Generic;
using UnityEngine;

namespace Tower
{
    [CreateAssetMenu(menuName = "TowerRecipe")]
    public class TowerRecipe : ScriptableObject
    {
        public string towerName;
        public Sprite towerIcon;
        public List<BodyObject> Recipe = new();
        public GameObject recipeTowerPrefab;
        public int shownPartsIfNotUnlocked;
    }
}