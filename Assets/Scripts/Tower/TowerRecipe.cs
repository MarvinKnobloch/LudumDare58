using System.Collections.Generic;
using UnityEngine;

namespace Tower
{
    [CreateAssetMenu(menuName = "TowerRecipe")]
    public class TowerRecipe : ScriptableObject
    {
        public List<BodyObject> Recipe = new();
    }
}