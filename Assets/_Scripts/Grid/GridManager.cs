using System.Collections.Generic;
using System.Linq;
using Nodes.Tiles;
using Pathfinding._Scripts.Grid.Scriptables;
using Pathfinding._Scripts.Units;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Pathfinding._Scripts.Grid {
    public class GridManager : MonoBehaviour {
        public static GridManager Instance;

        [SerializeField] private Sprite _playerSprite, _goalSprite;
        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private ScriptableSquareGrid _scriptableGrid;
        [SerializeField] private bool _drawConnections;

        public Dictionary<Vector2, NodeBase> tiles { get; private set; }

        private NodeBase _playerNodeBase, _goalNodeBase;
        private Unit _spawnedPlayer, _spawnedGoal;

        private List<NodeBase> reacheableNodes = new List<NodeBase>();

        void Awake() => Instance = this;

        private void Start() {
            tiles = _scriptableGrid.GenerateGrid();
         
            foreach (var tile in tiles.Values) tile.CacheNeighbors();

            SpawnUnits();
            NodeBase.OnSelectTile += TileSelected;
            NodeBase.OnMappedTile += TileMapped;
        }
        void SpawnUnits()
        {
            _playerNodeBase = tiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Value;
            _spawnedPlayer = Instantiate(_unitPrefab, _playerNodeBase.Coords.Pos, Quaternion.identity);
            _spawnedPlayer.Init(_playerSprite);
            _playerNodeBase.tileUnit = _spawnedPlayer;

            _spawnedGoal = Instantiate(_unitPrefab, new Vector3(50, 50, 50), Quaternion.identity);
            _spawnedGoal.Init(_goalSprite);
        }

        private void OnDestroy() => NodeBase.OnSelectTile -= TileSelected;

        private void TileSelected(NodeBase nodeBase) {
            _goalNodeBase = nodeBase;
            _spawnedGoal.transform.position = _goalNodeBase.Coords.Pos;

            foreach (var t in tiles.Values) t.RevertTile();

            if (Pathfinding.IsReachableNodes(_playerNodeBase, 10).Contains(_goalNodeBase))
            {
                var path = Pathfinding.FindPath(_playerNodeBase, _goalNodeBase);
                _spawnedPlayer.transform.position = _goalNodeBase.transform.position;
                _playerNodeBase = _goalNodeBase;
            }

            ResetReachebleNodes();
        }

        private void TileMapped(NodeBase nodeBase)
        {
            _goalNodeBase = nodeBase;
            _spawnedGoal.transform.position = _goalNodeBase.Coords.Pos;

            foreach (var t in tiles.Values) t.RevertTile();

            reacheableNodes = Pathfinding.MarkReachableNodes(nodeBase, 10);
        }

        private void ResetReachebleNodes()
        {
            foreach(NodeBase n in reacheableNodes)
            {
                n._isInRange = false;
            }
        }

        public NodeBase GetTileAtPosition(Vector2 pos) => tiles.TryGetValue(pos, out var tile) ? tile : null;

        private void OnDrawGizmos() {
            if (!Application.isPlaying || !_drawConnections) return;
            Gizmos.color = Color.red;
            foreach (var tile in tiles) {
                if (tile.Value.Connection == null) continue;
                Gizmos.DrawLine((Vector3)tile.Key + new Vector3(0, 0, -1), (Vector3)tile.Value.Connection.Coords.Pos + new Vector3(0, 0, -1));
            }
        }
    }
}