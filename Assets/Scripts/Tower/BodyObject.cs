using JetBrains.Annotations;
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
        [field: SerializeField] public float AoeRadius { get; private set; } = 0;
        [field: SerializeField] public Sprite Sprite { get; private set; }

        [field: SerializeField] public AttackAnimationType AnimationType { get; private set; } = AttackAnimationType.None;  // Which hit animation to use
        [field: SerializeField] public AttackAnimationElement AnimationElement { get; private set; } // Which hit animation after effect / layered effect to use
        [field: SerializeField] [CanBeNull] public Sprite ProjectileSprite { get; private set; } = null;
        [field: SerializeField] public WeaponType Weapon { get; private set; } 
    }
}