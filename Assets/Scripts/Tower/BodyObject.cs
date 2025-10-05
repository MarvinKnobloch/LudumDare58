using UnityEngine;

namespace Tower
{
    [CreateAssetMenu(menuName = "BodyPart")]
    public class BodyObject : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }

        [field: SerializeField, Tooltip("Maybe for UI tooltips later on.")]
        public string Description { get; private set; }

        [field: SerializeField] public BodyPart Part { get; private set; }
        [field: SerializeField] public BodyType Type { get; private set; } = BodyType.Default;
        [field: SerializeField] public float AttackSpeedModifier { get; private set; }
        [field: SerializeField] public float DamageModifier { get; private set; }
        [field: SerializeField] public float RangeModifier { get; private set; }



        [field: SerializeField, Tooltip("The UI Sprite to use.")]
        public Sprite Sprite { get; private set; }

        [field: SerializeField] public Sprite ProjectileSprite { get; private set; }
        
        [field: SerializeField, Header("Fill if part is weapon")] public WeaponType Weapon { get; private set; }
        [field: SerializeField] public float WeaponDamage { get; private set; }
        [field: SerializeField] public float WeaponRange { get; private set; }
        [field: SerializeField] public float WeaponSpeed { get; private set; }
        [field: SerializeField, Tooltip("Must be 0 for single target attacks.")]
        public float AoeRadius { get; private set; }
        
    }
}