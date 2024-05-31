using System.Collections.Generic;
using Nodes.Tiles;
using UnityEngine;

namespace Pathfinding._Scripts.Grid.Scriptables
{
    [CreateAssetMenu(fileName = "New Scriptable Square Grid")]
    public class ScriptableSquareGrid : ScriptableObject
    {
        [SerializeField, Range(3, 50)] private int _gridWidth = 16;
        [SerializeField, Range(3, 50)] private int _gridHeight = 9;
        [SerializeField] protected List<NodeBase> nodeBasePrefabs;
        [SerializeField, Range(0, 6)] private int _obstacleWeight = 3;

        protected bool DecideIfObstacle() => Random.Range(1, 20) > _obstacleWeight;

        public Dictionary<Vector2, NodeBase> GenerateGrid()
        {
            var tiles = new Dictionary<Vector2, NodeBase>();
            var grid = new GameObject
            {
                name = "Grid"
            };
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    var randomPrefabIndex = Random.Range(0, nodeBasePrefabs.Count);
                    var tile = Instantiate(nodeBasePrefabs[randomPrefabIndex], grid.transform);
                    tile.Init(DecideIfObstacle(), new SquareCoords { Pos = new Vector3(x, y) });
                    tiles.Add(new Vector2(x, y), tile);
                }
            }

            return tiles;
        }
    }
}
