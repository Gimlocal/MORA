using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mush
{
    public enum MushRarity
    {
        Normal,
        Gold,
        Boom,
    }

    public struct MushType
    {
        public MushRarity Rarity;
        [Range(0f, 100f)] public float Probability;
    }

    public class MushSpawner : MonoBehaviour
    {
        private static readonly int Normal = Shader.PropertyToID("_Normal");
        private static readonly int EffectColor = Shader.PropertyToID("_Color");
        
        [SerializeField] private GameObject normalMush;
        [SerializeField] private GameObject rareMush;
        [SerializeField] private Vector2 boundaryStart;
        [SerializeField] private Vector2 boundaryEnd;
        
        private int _spawnCount;
        private float _mushDistance;
        private float _rareSpawnRate;
        
        private readonly List<Vector2> _spawnedPositions = new();
        
        private MushType[] _probabilities = 
        {
            new MushType { Rarity = MushRarity.Normal, Probability = 91f },
            new MushType { Rarity = MushRarity.Gold,   Probability = 6f },
            new MushType { Rarity = MushRarity.Boom,   Probability = 300f },
        };

        private Dictionary<MushRarity, Color> _mushColors = new()
        {
            { MushRarity.Normal , Color.white},
            { MushRarity.Gold , new Color(191f/255f, 184f/255f, 11f/255f, 86f/255f)},
            { MushRarity.Boom , new Color(191f/255f, 67f/255f, 11f/255f, 86f/255f)},
        };

        private void Awake()
        {
            _spawnCount = 15;
            _mushDistance = 1f;
            _rareSpawnRate = 0.2f;
        }

        private void Start()
        {
            _spawnedPositions.Add(Player.Player.Instance.transform.position);
            SpawnMush();
        }

        private void SpawnMush()
        {
            int attempts = 0;
            int spawned = 0;

            while (spawned < _spawnCount && attempts < _spawnCount * 10)
            {
                Vector2 randomPos = GetRandomPosition();

                if (IsPositionValid(randomPos))
                {
                    GameObject prefabToSpawn = Random.value < _rareSpawnRate ? rareMush : normalMush;
                    MushRarity rarity = GetRandomRarity();
                    
                    GameObject mushPrefab = Instantiate(prefabToSpawn, randomPos, Quaternion.identity, transform);
                    Mush mush = mushPrefab.GetComponent<Mush>();
                    mush.rarity = rarity;
                    if (rarity != MushRarity.Normal)
                    {
                        Material mat = mushPrefab.GetComponent<Renderer>().material;
                        
                        mat.SetFloat(Normal, 0);
                        mat.SetColor(EffectColor, _mushColors[rarity]);
                    }
                    
                    _spawnedPositions.Add(randomPos);
                    spawned++;
                }

                attempts++;
            }
        }

        private MushRarity GetRandomRarity()
        {
            float total = _probabilities.Sum(p => p.Probability);
            float random = Random.Range(0, total + 1);
            
            float cumulative = 0f;
            foreach (var p in _probabilities)
            {
                cumulative += p.Probability;
                if (random < cumulative)
                {
                    return p.Rarity;
                }
            }

            return MushRarity.Normal;
        }
        
        private Vector2 GetRandomPosition()
        {
            float x = Random.Range(boundaryStart.x, boundaryEnd.x);
            float y = Random.Range(boundaryStart.y, boundaryEnd.y);
            return new Vector2(x, y);
        }
        
        private bool IsPositionValid(Vector2 newPos)
        {
            foreach (var pos in _spawnedPositions)
            {
                if (Vector2.Distance(pos, newPos) < _mushDistance)
                    return false;
            }

            return true;
        }
    }
}
