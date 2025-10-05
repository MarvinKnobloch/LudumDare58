using System.Collections.Generic;
using UnityEngine;

namespace Tower
{
    public class TowerRecipe : ScriptableObject
    {
        public List<BodyObject> Recipe = new();
    }
}