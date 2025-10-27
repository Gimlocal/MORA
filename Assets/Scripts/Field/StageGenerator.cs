using System.Collections.Generic;
using System.Linq;
using Database;
using UnityEngine;

namespace Field
{
    public class StageGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        
        [Header("Database")]
        public RoomDatabase roomDB;

        [Header("Stage Layout")]
        public Vector2Int gridSize = new Vector2Int(6, 6);
        public int minRooms = 10;
        public int maxRooms = 18;
        public Vector2 roomStride = new Vector2(16f, 9f);

        [Header("Random")]
        public bool randomSeed = true;
        public int seed;

        private System.Random _rng;
        
        private class Node
        {
            public Vector2Int GridPos;
            public DoorMask Direction;
            public RoomType Type;
            public GameObject Room;
        }

        Dictionary<Vector2Int, Node> _nodes = new();

        private void Awake()
        {
            SetSeed();
            Generate();
        }

        private void SetSeed()
        {
            if (randomSeed) seed = Random.Range(int.MinValue, int.MaxValue);
            _rng = new System.Random(seed);
        }

        private void Generate()
        {
            // 방 초기화
            foreach (Transform c in transform) Destroy(c.gameObject);
            _nodes.Clear();

            // 방 개수 및 시작위치 설정
            int targetRooms = Mathf.Clamp(_rng.Next(minRooms, maxRooms + 1), 1, gridSize.x * gridSize.y);

            Vector2Int start = new Vector2Int(_rng.Next(gridSize.x), _rng.Next(gridSize.y));
            AddNode(start);

            // 시작방으로부터 확장
            Vector2Int[] four = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
            while (_nodes.Count < targetRooms)
            {
                var baseCell = _nodes.Keys.ElementAt(_rng.Next(_nodes.Count));
                var dir = four[_rng.Next(4)];
                var next = baseCell + dir;
                if (!Inside(next)) continue;
                if (_nodes.ContainsKey(next)) continue;
                AddNode(next);
            }

            // 보스방을 시작방으로부터 제일 먼 곳으로 설정
            Vector2Int boss = start;
            int dist = -1;
            foreach (var k in _nodes.Keys)
            {
                int d = Mathf.Abs(k.x - start.x) + Mathf.Abs(k.y - start.y);
                if (d > dist) { dist = d; boss = k; }
            }

            // 비트마스크로 문 방향 설정
            foreach (var n in _nodes.Values)
            {
                DoorMask m = DoorMask.None;
                if (_nodes.ContainsKey(n.GridPos + Vector2Int.up)) m |= DoorMask.N;
                if (_nodes.ContainsKey(n.GridPos + Vector2Int.right)) m |= DoorMask.E;
                if (_nodes.ContainsKey(n.GridPos + Vector2Int.down)) m |= DoorMask.S;
                if (_nodes.ContainsKey(n.GridPos + Vector2Int.left)) m |= DoorMask.W;
                n.Direction = m;
            }
            
            AssignTypes(start, boss);

            // 5) 인스턴스화 & 문 활성화
            BuildWorld();

            player.transform.position = new Vector3(
                (start.x - (gridSize.x - 1) / 2f) * roomStride.x,
                (start.y - (gridSize.y - 1) / 2f) * roomStride.y,
                0f);
            
            Debug.Log($"GridStage generated: seed {seed}, rooms {_nodes.Count}, start {start}, boss {boss}");
        }

        private bool Inside(Vector2Int g) => (uint)g.x < (uint)gridSize.x && (uint)g.y < (uint)gridSize.y;

        private void AddNode(Vector2Int pos)
        {
            _nodes[pos] = new Node { GridPos = pos, Direction = DoorMask.None };
        }

        // 방 타입 설정
        private void AssignTypes(Vector2Int start, Vector2Int boss)
        {
            var all = roomDB.Rooms ?? new List<RoomType>();
            if (all.Count == 0)
            {
                Debug.LogError("RoomDatabase is empty"); 
                return;
            }

            // 시작, 보스, 일반방들을 분리
            var startCandidates = all.Where(r => r.isStart).ToList();
            var bossCandidates  = all.Where(r => r.isBoss).ToList();
            var normalCandidates = all.Where(r => !r.isStart && !r.isBoss).ToList();

            // 특수방이 사용됐는지 체크
            var usedUniques = new HashSet<int>();

            // 시작, 보스 먼저 설정
            _nodes[start].Type = WeightedPickWithDoors(startCandidates, _nodes[start].Direction, usedUniques);
            _nodes[boss].Type  = WeightedPickWithDoors(bossCandidates,  _nodes[boss].Direction,  usedUniques);

            // 나머지
            foreach (var kv in _nodes)
            {
                if (kv.Key == start || kv.Key == boss) continue;
                var picked = WeightedPickWithDoors(normalCandidates, kv.Value.Direction, usedUniques);

                // 전부 실패 시 문 제약 완화 후 재시도
                if (picked == null)
                    picked = WeightedPickRelaxed(normalCandidates, usedUniques);

                kv.Value.Type = picked;
            }
        }

        // 문 방향을 모두 만족하는 후보만
        private RoomType WeightedPickWithDoors(List<RoomType> candidates, DoorMask need, HashSet<int> usedUniques)
        {
            var pool = new List<RoomType>();
            foreach (var r in candidates)
            {
                if ((r.allowedDoors & need) != need)
                {
                    continue;
                }
                if (r.uniquePerStage && usedUniques.Contains(r.id))
                {
                    continue;
                }
                
                int w = Mathf.Max(1, r.weight);
                for (int i = 0; i < w; i++)
                {
                    pool.Add(r);
                }
            }
            if (pool.Count == 0)
            {
                return null;
            }
            var chosen = pool[_rng.Next(pool.Count)];
            if (chosen.uniquePerStage)
            {
                usedUniques.Add(chosen.id);
            }
            return chosen;
        }

        // 문 방향이 안맞아도 사용
        private RoomType WeightedPickRelaxed(List<RoomType> candidates, HashSet<int> usedUniques)
        {
            var pool = new List<RoomType>();
            foreach (var r in candidates)
            {
                if (r.uniquePerStage && usedUniques.Contains(r.id))
                {
                    continue;
                }
                
                int w = Mathf.Max(1, r.weight);
                for (int i = 0; i < w; i++)
                {
                    pool.Add(r);
                }
            }
            if (pool.Count == 0)
            {
                return null;
            }
            var chosen = pool[_rng.Next(pool.Count)];
            if (chosen.uniquePerStage)
            {
                usedUniques.Add(chosen.id);
            }
            return chosen;
        }

        private void BuildWorld()
        {
            foreach (var n in _nodes.Values)
            {
                if (n.Type == null || n.Type.roomPrefab == null)
                {
                    Debug.LogError("Room type or prefab missing at " + n.GridPos);
                    continue;
                }

                Vector3 pos = new Vector3(
                    (n.GridPos.x - (gridSize.x - 1) / 2f) * roomStride.x,
                    (n.GridPos.y - (gridSize.y - 1) / 2f) * roomStride.y,
                    0f);

                n.Room = Instantiate(n.Type.roomPrefab, pos, Quaternion.identity, transform);

                // 문 활성화/비활성
                foreach (var door in n.Room.GetComponentsInChildren<DoorMarker>(true))
                {
                    bool on = (n.Direction & door.direction) != 0;
                    door.gameObject.SetActive(on);
                }
            }
        }
    }
}
