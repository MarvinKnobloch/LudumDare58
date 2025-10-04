using UnityEngine;

namespace Tower
{
    public class BodyObject : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; } // probably not necessary
        [field: SerializeField] public BodyPart Part { get; private set; }
        [field: SerializeField] public float AttackSpeedModifier {get; private set; }
        [field: SerializeField] public float DamageModifier {get; private set; }
        [field: SerializeField] public float RangeModifier {get; private set; }
        [field: SerializeField] public float AoeRadius { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        
        [field: SerializeField] public Sprite ProjectileSprite { get; private set; }
        [field: SerializeField] public WeaponType Weapon { get; private set; } 
    }
}