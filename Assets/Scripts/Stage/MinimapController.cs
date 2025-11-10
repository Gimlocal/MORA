using System.Collections.Generic;
using Database;
using UnityEngine;

namespace Stage
{
    /// <summary>
    /// - 시작방을 미니맵 중앙에 고정
    /// - 현재 위치한 방은 흰색
    /// - 현재 위치하지 않고 방문한 방은 옅은 회색
    /// - 방문하지 않았고, 방문했던 방의 이웃방들은 진한 회색
    /// - 나머지 방들은 안보임
    /// </summary>
    public class MinimapController : MonoBehaviour
    {
        [Header("References")]
        public RectTransform content;
        public GameObject miniCellPrefab;

        [Header("Layout")]
        public Vector2 cellSize = new Vector2(16, 16);
        public float cellGap;

        [Header("Visuals")]
        [Range(0f, 1f)]
        public float frontierFogAlpha = 0.75f;
        
        public bool highlightCurrent = true;

        private StageGenerator _generator;
        private Transform _player;
        
        class CellView
        {
            public RectTransform RT;
            public MiniCellView View;
            public bool Discovered;
        }

        private readonly Dictionary<Vector2Int, CellView> _cells = new();
        private readonly HashSet<Vector2Int> _visited = new();
        private readonly HashSet<Vector2Int> _frontier = new();

        private Vector2Int _startGrid;
        private Vector2Int _currentGrid;

        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left
        };

        private static readonly DoorMask[] DoorMasks =
        {
            DoorMask.N, DoorMask.E, DoorMask.S, DoorMask.W
        };

        private void Awake()
        {
            _generator = FindAnyObjectByType<StageGenerator>();
            
            if (_generator != null)
            {
                _generator.OnStageBuilt += SetMiniMap;
            }
        }

        private void Start()
        {
            _player = Player.Player.Instance.transform;
        }

        private void OnDestroy()
        {
            if (_generator != null)
            {
                _generator.OnStageBuilt -= SetMiniMap;
            }
        }
        
        private void SetMiniMap(IReadOnlyList<StageGenerator.NodeSnapshot> nodes, Vector2Int start, Vector2Int boss)
        {
            foreach (Transform c in content)
            {
                Destroy(c.gameObject);
            }
            _cells.Clear();
            _visited.Clear();
            _frontier.Clear();

            _startGrid = start;
            _currentGrid = start;

            // content를 중앙 기준으로
            content.pivot = new Vector2(0.5f, 0.5f);
            content.anchorMin = content.anchorMax = new Vector2(0.5f, 0.5f);
            content.anchoredPosition = Vector2.zero;
            content.localScale = Vector3.one;

            // 모든 노드에 대해 셀 생성 (시작방을 (0,0)으로 오프셋)
            foreach (var n in nodes)
            {
                var go = Instantiate(miniCellPrefab, content);
                var rt = go.GetComponent<RectTransform>();
                var v  = go.GetComponent<MiniCellView>();

                Vector2Int local = n.gridPos - _startGrid; // 시작방 기준 좌표
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

                // 좌하 원점이 아닌 중앙 기준이므로 y는 그대로 곱해도 됨(위로 +)
                rt.anchoredPosition = new Vector2(
                    local.x * (cellSize.x + cellGap),
                    local.y * (cellSize.y + cellGap)
                );

                v.SetType(n.roomType);
                v.SetDoors(n.doors);
                v.SetHighlight(false);
                v.SetDiscovered(false);

                _cells[n.gridPos] = new CellView { RT = rt, View = v, Discovered = false };

                // 처음엔 Unknown → 비활성화
                go.SetActive(false);
            }

            // 시작방 방문 처리
            SetCurrent(start);

            // Frontier 구성 후 가시성 일괄 갱신
            RecomputeFrontier();
            UpdateVisibility();
        }

        public void UpdateMiniMap()
        {
            if (_cells.Count == 0 || _player == null || _generator == null)
            {
                return;
            }

            var g = _generator.WorldToGrid(_player.position);
            if (g != _currentGrid && _cells.ContainsKey(g))
            {
                SetCurrent(g);
                RecomputeFrontier();
                UpdateVisibility();
            }
        }

        // ---------- 현재 방/방문 처리 ----------
        private void SetCurrent(Vector2Int g)
        {
            if (highlightCurrent && _cells.TryGetValue(_currentGrid, out var cell))
            {
                cell.View.SetHighlight(false);
            }

            _currentGrid = g;
            _visited.Add(g);

            if (_cells.TryGetValue(g, out var c))
            {
                c.RT.gameObject.SetActive(true);
                c.View.SetDiscovered(true); // 안개 제거
                if (highlightCurrent)
                {
                    c.View.SetHighlight(true);
                }
                c.Discovered = true;
            }
        }

        // ---------- Frontier 재계산: 방문 방과 '문으로 연결된' 미방문 이웃 ----------
        private void RecomputeFrontier()
        {
            _frontier.Clear();

            foreach (var p in _visited)
            {
                if (!_cells.TryGetValue(p, out var cell))
                {
                    continue;
                }
                var doors = cell.View.Doors;

                for (int i = 0; i < 4; i++)
                {
                    if ((doors & DoorMasks[i]) == 0)
                    {
                        continue;   // 문이 없으면 연결 아님
                    } 
                    var n = p + Directions[i];
                    if (!_cells.ContainsKey(n))
                    {
                        continue;   // 실제 방이 없으면 패스
                    }    
                    if (_visited.Contains(n))
                    {
                        continue;   // 이미 방문이면 패스
                    }    
                    _frontier.Add(n);
                }
            }
        }

        // ---------- 가시성 규칙 적용 ----------
        private void UpdateVisibility()
        {
            // 1) 방문(Visited): 항상 보임 + 안개 제거
            foreach (var p in _visited)
            {
                if (!_cells.TryGetValue(p, out var c))
                {
                    continue;
                }
                if (!c.RT.gameObject.activeSelf)
                {
                    c.RT.gameObject.SetActive(true);
                }
                c.View.SetDiscovered(true); // fog.alpha = 0
                c.Discovered = true;
            }

            // 2) 프론티어(Frontier): 보이되 반투명 안개
            foreach (var p in _frontier)
            {
                if (!_cells.TryGetValue(p, out var c))
                {
                    continue;
                }
                if (!c.RT.gameObject.activeSelf)
                {
                    c.RT.gameObject.SetActive(true);
                }

                // 기본 SetDiscovered(false)는 alpha=1이지만,
                // 프론티어는 존재만 알리고 미방문 표시를 위해 약간 투명
                c.View.SetDiscovered(false);
                if (c.View.fog != null)
                {
                    c.View.fog.alpha = frontierFogAlpha;
                }
            }

            // 3) Unknown: 숨김
            foreach (var kv in _cells)
            {
                var p = kv.Key;
                if (_visited.Contains(p) || _frontier.Contains(p))
                {
                    continue;
                }

                var c = kv.Value;
                if (c.RT.gameObject.activeSelf)
                {
                    c.RT.gameObject.SetActive(false);
                }
            }
        }
    }
}
