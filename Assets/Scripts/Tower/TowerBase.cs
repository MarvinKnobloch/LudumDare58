using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tower
{
    public class TowerBase : MonoBehaviour
    {
        public static UnityEvent<TowerBase, BodyObject> BodyPartEquipped { get; set; } = new();
        public static UnityEvent<TowerBase, BodyObject> BodyPartUnequipped { get; set; } = new();
        
        [SerializeField] private readonly float _baseAttackSpeed;
        [SerializeField] private readonly float _baseDamage;
        [SerializeField] private readonly float _baseRange;
        [SerializeField] private bool _isAoe = false;

        private float _currentAttackSpeed;
        private float _currentDamage;
        private float _currentRange;

        public readonly List<BodyObject> EquippedBodyParts = new();

        private void Awake()
        {
            _currentRange = _baseRange;
            _currentDamage = _baseDamage;
            _currentRange = _baseRange;
        }

        private void OnEnable()
        {
            BodyPartEquipped.AddListener(OnBodyPartEquipped);
            BodyPartUnequipped.AddListener(OnBodyPartUnequipped);
        }

        private void OnDisable()
        {
            BodyPartEquipped.RemoveListener(OnBodyPartEquipped);
            BodyPartUnequipped.RemoveListener(OnBodyPartUnequipped);
        }

        private void OnBodyPartEquipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;
            
            EquippedBodyParts.Add(bodyObject);
            _currentAttackSpeed += bodyObject.AttackSpeedModifier;
            _currentDamage += bodyObject.DamageModifier;
            _currentRange += bodyObject.RangeModifier;
            _isAoe = bodyObject.IsAoe;
        }

        private void OnBodyPartUnequipped(TowerBase tower, BodyObject bodyObject)
        {
            if (tower != this)
                return;
            
            EquippedBodyParts.Remove(bodyObject);
            _currentAttackSpeed -= bodyObject.AttackSpeedModifier;
            _currentDamage -= bodyObject.DamageModifier;
            _currentRange = bodyObject.RangeModifier;
            _isAoe = false;
        }
    }
}
