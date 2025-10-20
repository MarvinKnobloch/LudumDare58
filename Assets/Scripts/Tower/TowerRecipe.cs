using System;
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
        [HideInInspector] public List<bool> partUnlocked = new();
        public List<bool> defaultUnlockState = new();
        public GameObject recipeTowerPrefab;
    }
}