using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tower
{
    [RequireComponent(typeof(TowerBase))]
    public class TowerVisualizer : MonoBehaviour
    {
        private TowerBase _tower;
        private SpriteRenderer _renderer;
        [SerializeField] private Sprite _orcSprite;
        [SerializeField] private Sprite _frankensteinSprite;
        [SerializeField] private Sprite _skeletonSprite;
        [SerializeField] private Sprite _goblinSprite;
        [SerializeField] private Sprite _defaultSprite;

        private Dictionary<BodyType, Sprite> _bodySprites;

        public void Awake()
        {
            _tower = GetComponent<TowerBase>();
            _bodySprites = new Dictionary<BodyType, Sprite>
            {
                { BodyType.Default, _defaultSprite },
                { BodyType.Frankenstein, _frankensteinSprite },
                { BodyType.Orc, _orcSprite },
                { BodyType.Skeleton, _skeletonSprite },
                { BodyType.Goblin, _goblinSprite },
            };
        }

        private void OnEnable()
        {
            TowerBase.BodyPartEquipped.AddListener(CheckForVisualChange);
        }

        private void OnDisable()
        {
            TowerBase.BodyPartEquipped.RemoveListener(CheckForVisualChange);
        }

        private void CheckForVisualChange(TowerBase tower, BodyObject body)
        {
            if (tower != _tower) return;
            
            var _orcCount = 0;
            var _frankensteinCount = 0;
            var _skeleCount = 0;
            var _gobboCount = 0;

            foreach (var part in _tower.EquippedBodyObjects)
            {
                switch (part.Type)
                {
                    case BodyType.Default:
                        break;
                    case BodyType.Skeleton:
                        _skeleCount++;
                        break;
                    case BodyType.Goblin:
                        _gobboCount++;
                        break;
                    case BodyType.Frankenstein:
                        _frankensteinCount++;
                        break;
                    case BodyType.Orc:
                        _orcCount++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var countList = new List<(BodyType, int)>()
            {
                (BodyType.Orc, _orcCount),
                (BodyType.Frankenstein, _frankensteinCount),
                (BodyType.Goblin, _gobboCount),
                (BodyType.Skeleton, _skeleCount)
            };

            countList.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            var bodyType = countList.First().Item1;
            var potSprite =  _bodySprites[bodyType];
            
            if (_renderer.sprite != potSprite) _renderer.sprite = _bodySprites[bodyType];
        }
    }
}