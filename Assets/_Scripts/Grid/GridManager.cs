using System.Collections;
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

        private NodeBase _currentNode, _goalNodeBase;
        private Unit _currentUnit;

        public bool _isTileMoved = false;

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
                randomNode._tileUnit = instanceUnit;
                instanceUnit._actualNode = randomNode;
                UnitsManager.Instance.npcUnits.Add(instanceUnit);
            }
        }

        private void OnDestroy() => NodeBase.OnSelectTile -= TileSelected;

        private void TileSelected(NodeBase nodeBase)
        {
            _goalNodeBase = nodeBase;

            foreach (var t in tiles.Values) t.RevertTile();

            if (Pathfinding.IsReachableNodes(_currentNode, _currentNode._tileUnit._movements).Contains(_goalNodeBase) || UnitsManager.Instance.isNpcTurn)
            {
                List<NodeBase> path = Pathfinding.FindPath(_currentNode, _goalNodeBase);

                if (path != null && path.Count > 0)
                {
                    // Reverse the path in place
                    path.Reverse();

                    // Iniciar la corrutina de movimiento
                    StartCoroutine(MoveUnitAlongPath(path));
                }
            }

            ResetReachebleNodes();
        }


        private IEnumerator MoveUnitAlongPath(List<NodeBase> path)
        {
            path.RemoveAt(0);

            var unitMover = _currentUnit.GetComponent<UnitMover>();

            yield return StartCoroutine(unitMover.MoveAlongPath(path, 25f));

            _currentUnit.transform.position = _goalNodeBase.transform.position;
            _currentNode._tileUnit = null;
            _currentNode = _goalNodeBase;
            _currentNode._tileUnit = _currentUnit;
            _currentNode._tileUnit._actualNode = _currentNode;
        }

        public IEnumerator MoveAlongPath(List<NodeBase> path, float speed)
        {
            foreach (var node in path)
            {
                Vector3 startPosition = transform.position;
                Vector3 endPosition = node.transform.position;
                float journey = 0f;

                while (journey < 1f)
                {
                    journey += Time.deltaTime * speed;
                    transform.position = Vector3.Lerp(startPosition, endPosition, journey);
                    yield return null;
                }
            }
        }

        private void TileMapped(NodeBase nodeBase)
        {
            _currentNode = nodeBase;
            _currentUnit = nodeBase._tileUnit;

            foreach (var t in tiles.Values) t.RevertTile();

            reacheableNodes = Pathfinding.MarkReachableNodes(nodeBase, nodeBase._tileUnit._movements);
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