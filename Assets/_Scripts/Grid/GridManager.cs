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

        [SerializeField] private Unit _unitPrefab;
        [SerializeField] private ScriptableSquareGrid _scriptableGrid;
        [SerializeField] private bool _drawConnections;

        public Dictionary<Vector2, NodeBase> tiles { get; private set; }

        public List<Unit> _unitList = new List<Unit>();
        public List<Unit> _unitListInstance = new List<Unit>();

        private NodeBase _currentNode, _goalNodeBase;
        private Unit _currentUnit;

        public bool _isTileSelected = false;

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
            foreach(Unit unit in _unitList)
            {
                NodeBase randomNode = tiles.Where(t => t.Value.Walkable).OrderBy(t => Random.value).First().Value;
                Unit instanceUnit = Instantiate(unit, randomNode.Coords.Pos, Quaternion.identity);
                instanceUnit.Init(unit._sprite);
                randomNode.tileUnit = instanceUnit;
                instanceUnit._actualNode = randomNode;
                if(instanceUnit._isNpc)
                {
                    UnitsManager.Instance.npcUnits.Add(instanceUnit);
                }
            }
        }

        private void OnDestroy() => NodeBase.OnSelectTile -= TileSelected;

        private void TileSelected(NodeBase nodeBase) {
            _goalNodeBase = nodeBase;

            foreach (var t in tiles.Values) t.RevertTile();

            print(_goalNodeBase.gameObject.transform.position);

            if (Pathfinding.IsReachableNodes(_currentNode, _currentNode.tileUnit._movements).Contains(_goalNodeBase))
            {
                var path = Pathfinding.FindPath(_currentNode, _goalNodeBase);
                _currentUnit.transform.position = _goalNodeBase.transform.position;
                _currentNode.tileUnit = null;
                _currentNode = _goalNodeBase;
                _currentNode.tileUnit = _currentUnit;
                _currentNode.tileUnit._actualNode = _currentNode; 
            }

            ResetReachebleNodes();
        }

        private void TileMapped(NodeBase nodeBase)
        {
            _currentNode = nodeBase;
            _currentUnit = nodeBase.tileUnit;

            foreach (var t in tiles.Values) t.RevertTile();

            reacheableNodes = Pathfinding.MarkReachableNodes(nodeBase, nodeBase.tileUnit._movements);
        }

        public void TestFourDirections(NodeBase nodeBase)
        {
            Pathfinding.MarkReachableNodesInFourDirections(nodeBase, 3);
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