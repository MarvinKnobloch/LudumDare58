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
        [field: SerializeField] public float BonusAttackSpeed { get; private set; }
        [field: SerializeField] public int BonusDamage { get; private set; }
        [field: SerializeField] public float BonusRange { get; private set; }

        [field: SerializeField, Tooltip("Must be 0 for single target attacks.")]
        public float BonusAoeRadius { get; private set; }

        [field: SerializeField, Tooltip("The UI Sprite to use.")]
        public Sprite Sprite { get; private set; }
        
        [field: SerializeField, Header("Fill if part is weapon")] public WeaponType Weapon { get; private set; }
        [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
        [field: SerializeField] public TargetType TargetType { get; private set; }
        [field: SerializeField] public bool Slow { get; private set; }
        [field: SerializeField] public int SlowPercentage { get; private set; }
        [field: SerializeField] public int SlowDuration { get; private set; }
        [field: SerializeField] public int AdditionalTargets { get; private set; }
    }
}