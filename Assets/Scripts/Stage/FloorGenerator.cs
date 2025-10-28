using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Stage
{
    public class FloorGenerator : MonoBehaviour
    {
        [Header("Map Settings")]
        public int width = 80;
        public int height = 80;
        public int maxRooms = 20;
        public int roomMinSize = 5;
        public int roomMaxSize = 12;
        public int seed = 0;
        public bool useRandomSeed = true;

        [Header("Tilemap & Tiles")]
        public Tilemap groundTilemap;
        public Tilemap wallTilemap;
        public TileBase floorTile;
        public TileBase wallTile;

        [Header("Prefabs Settings")]
        public GameObject player;
        public GameObject[] mineralPrefabs; // index: common -> rare
        public int baseMineralPerRoom = 2;
        [Range(0f,1f)] public float rareChance = 0.08f;

        private System.Random _rng;
        private int[,] _map; // 0 = wall, 1 = floor
        private List<RectInt> _rooms = new();
        private Vector2Int _playerStart;

        void Start()
        {
            if (useRandomSeed) seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            _rng = new System.Random(seed);
            GenerateDungeon();
        }

        #region Generation
        public void GenerateDungeon()
        {
            _map = new int[width, height];
            _rooms.Clear();
            ClearTilemap();
            GenerateRooms();
            SetCorridorsMST();
            SetTilemap();
            PlaceMinerals();
            ValidateAndFix();
            Debug.Log("Dungeon generated seed: " + seed);
        }

        void ClearTilemap()
        {
            if (groundTilemap != null) groundTilemap.ClearAllTiles();
        }

        void GenerateRooms()
        {
            int attempts = 0;
            while (_rooms.Count < maxRooms && attempts < maxRooms * 10)
            {
                attempts++;
                int rw = _rng.Next(roomMinSize, roomMaxSize + 1);
                int rh = _rng.Next(roomMinSize, roomMaxSize + 1);
                int rx = _rng.Next(1, width - rw - 1);
                int ry = _rng.Next(1, height - rh - 1);
                RectInt newRoom = new RectInt(rx, ry, rw, rh);

                bool overlaps = false;
                int padding = 2;
                foreach (var r in _rooms)
                {
                    RectInt expanded = new RectInt(
                        r.xMin - padding,
                        r.yMin - padding,
                        r.width + padding * 2,
                        r.height + padding * 2
                    );
                    
                    if (expanded.Overlaps(newRoom))
                    {
                        overlaps = true; 
                        break;
                    }
                }

                if (!overlaps)
                {
                    _rooms.Add(newRoom);
                    // Carve room immediately
                    for (int x = newRoom.xMin; x < newRoom.xMax; x++)
                    {
                        for (int y = newRoom.yMin; y < newRoom.yMax; y++)
                        {
                            _map[x, y] = 1;
                        }
                    }
                }
            }

            // set player start at center of first room
            if (_rooms.Count > 0)
            {
                var c = _rooms[0].center;
                _playerStart = new Vector2Int((int)c.x, (int)c.y);
            }
        }
        
        void SetCorridorsMST()
        {
            int n = _rooms.Count;
            if (n <= 1) return;

            // Prim-ish 방식으로 MST 생성 (O(n^2) 단순 구현)
            bool[] inTree = new bool[n];
            inTree[0] = true;
            List<(int a, int b)> edges = new List<(int a, int b)>();

            while (edges.Count < n - 1)
            {
                float bestDist = float.MaxValue;
                int bestFrom = -1, bestTo = -1;

                for (int i = 0; i < n; i++)
                {
                    if (!inTree[i]) continue;
                    for (int j = 0; j < n; j++)
                    {
                        if (inTree[j]) continue;
                        // center는 Vector2 => 안전하게 float 거리 구함
                        Vector2 ci = _rooms[i].center;
                        Vector2 cj = _rooms[j].center;
                        float dist = Vector2.Distance(ci, cj);
                        if (dist < bestDist)
                        {
                            bestDist = dist;
                            bestFrom = i;
                            bestTo = j;
                        }
                    }
                }

                if (bestFrom == -1 || bestTo == -1) break;
                edges.Add((bestFrom, bestTo));
                inTree[bestTo] = true;
            }

            // MST 간선들로 복도 carve
            foreach (var e in edges)
            {
                Vector2 cA = _rooms[e.a].center;
                Vector2 cB = _rooms[e.b].center;
                Vector2Int a = new Vector2Int(Mathf.RoundToInt(cA.x), Mathf.RoundToInt(cA.y));
                Vector2Int b = new Vector2Int(Mathf.RoundToInt(cB.x), Mathf.RoundToInt(cB.y));

                if (_rng.NextDouble() < 0.5)
                {
                    SetHorizLine(a.x, b.x, a.y);
                    SetVertLine(a.y, b.y, b.x);
                }
                else
                {
                    SetVertLine(a.y, b.y, a.x);
                    SetHorizLine(a.x, b.x, b.y);
                }
            }

            // --- 플레이어 시작을 MST의 leaf(차수==1)로 선택 ---
            int[] degree = new int[n];
            foreach (var e in edges)
            {
                degree[e.a]++;
                degree[e.b]++;
            }

            List<int> leaves = new List<int>();
            for (int i = 0; i < n; i++) if (degree[i] == 1) leaves.Add(i);

            if (leaves.Count == 0)
            {
                // 안전장치: leaf가 없으면 기존 첫 방 사용
                var c = _rooms[0].center;
                _playerStart = new Vector2Int(Mathf.RoundToInt(c.x), Mathf.RoundToInt(c.y));
            }
            else
            {
                // leaf들 중 하나를 선택하는 전략:
                // 1) 가장 먼 leaf 선택(던전의 '끝' 느낌 강화)
                //    또는 2) 랜덤 하나 선택 (주석 처리 후 바꿀 수 있음)
                int chosen = leaves[0];
                float bestDist = -1f;
                // 중심(또는 맵 중심)으로부터 거리 기준으로 가장 먼 leaf 선택
                Vector2 mapCenter = new Vector2(width / 2f, height / 2f);
                foreach (int idx in leaves)
                {
                    Vector2 c = _rooms[idx].center;
                    float d = Vector2.Distance(c, mapCenter);
                    if (d > bestDist)
                    {
                        bestDist = d;
                        chosen = idx;
                    }
                }

                var cc = _rooms[chosen].center;
                _playerStart = new Vector2Int(Mathf.RoundToInt(cc.x), Mathf.RoundToInt(cc.y));
                
                player.transform.position = (Vector2)_playerStart;
            }
        }


        void SetHorizLine(int x1, int x2, int y)
        {
            int start = Math.Min(x1, x2), end = Math.Max(x1, x2);
            for (int x = start; x <= end; x++)
                _map[x, y] = 1;
        }
        
        void SetVertLine(int y1, int y2, int x)
        {
            int start = Math.Min(y1, y2), end = Math.Max(y1, y2);
            for (int y = start; y <= end; y++)
                _map[x, y] = 1;
        }

        void SetTilemap()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (_map[x, y] == 1)
                    {
                        groundTilemap.SetTile(pos, floorTile);
                    }
                    else
                    {
                        wallTilemap.SetTile(pos, wallTile);
                    }
                }
        }
        #endregion

        #region Minerals
        void PlaceMinerals()
        {
            foreach (var room in _rooms)
            {
                int roomArea = room.size.x * room.size.y;
                int count = Mathf.Clamp(baseMineralPerRoom + _rng.Next(-1, 2), 1, Mathf.Max(1, roomArea / 10));
                int placed = 0;
                int tries = 0;
                while (placed < count && tries < count * 10)
                {
                    tries++;
                    int x = _rng.Next(room.xMin + 1, room.xMax - 1);
                    int y = _rng.Next(room.yMin + 1, room.yMax - 1);
                    if (_map[x, y] != 1) continue; // must be floor

                    // ensure no other mineral nearby (simple clustering rule)
                    if (IsNearbyMineral(x, y, 2)) continue;

                    // choose mineral rarity by distance from start
                    float dist = Vector2Int.Distance(_playerStart, new Vector2Int(x, y));
                    double rarityRoll = _rng.NextDouble() + dist / (width + height); // further -> rarer
                    int prefabIndex = 0;
                    if (rarityRoll > 0.9 - rareChance) prefabIndex = Mathf.Min(mineralPrefabs.Length - 1, 2); // rare
                    else if (rarityRoll > 0.6) prefabIndex = 1; // uncommon
                    else prefabIndex = 0; // common

                    Vector3 worldPos = groundTilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f, 0.5f, 0f);
                    Instantiate(mineralPrefabs[prefabIndex], worldPos, Quaternion.identity, transform);
                    placed++;
                }
            }
        }

        bool IsNearbyMineral(int x, int y, int radius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(groundTilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0.5f,0.5f,0f), radius);
            foreach (var c in hits) if (c.GetComponent<Mush.Mush>() != null) return true;
            return false;
        }
        #endregion

        #region Validation
        void ValidateAndFix()
        {
            // simple connectivity check: from playerStart, count reachable floor tiles
            bool[,] visited = new bool[width, height];
            Queue<Vector2Int> q = new Queue<Vector2Int>();
            q.Enqueue(_playerStart);
            visited[_playerStart.x, _playerStart.y] = true;
            int reachable = 0;
            int totalFloor = 0;
            for (int x=0;x<width;x++) for (int y=0;y<height;y++) if (_map[x,y]==1) totalFloor++;

            int[] dx = {1,-1,0,0}, dy = {0,0,1,-1};
            while (q.Count>0)
            {
                var p = q.Dequeue(); reachable++;
                for (int i=0;i<4;i++){
                    int nx=p.x+dx[i], ny=p.y+dy[i];
                    if (nx<0||ny<0||nx>=width||ny>=height) continue;
                    if (visited[nx,ny] || _map[nx,ny]==0) continue;
                    visited[nx,ny]=true; q.Enqueue(new Vector2Int(nx,ny));
                }
            }

            float reachRatio = (float)reachable / Mathf.Max(1, totalFloor);
            if (reachRatio < 0.6f)
            {
                Debug.LogWarning("Low connectivity detected ("+reachRatio+"), regenerating...");
                GenerateDungeon(); // naive: regenerate until good
            }
        }
        #endregion
    }
}
