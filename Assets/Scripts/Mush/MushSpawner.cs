using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mush
{
    public class MushSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject normalMush;
        [SerializeField] private GameObject rareMush;
        [SerializeField] private Vector2 boundaryStart;
        [SerializeField] private Vector2 boundaryEnd;
        private int _spawnCount;
        private float _rareSpawnRate;
        private float _mushDistance;
        
        private readonly List<Vector2> _spawnedPositions = new();

        private void Awake()
        {
            _spawnCount = 15;
            _rareSpawnRate = 0.2f;
            _mushDistance = 1f;
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
                    Instantiate(prefabToSpawn, randomPos, Quaternion.identity, transform);
                    _spawnedPositions.Add(randomPos);
                    spawned++;
                }

                attempts++;
            }
            
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
