using System.Collections.Generic;
using Nodes.Tiles;
using UnityEngine;

namespace Pathfinding._Scripts.Grid.Scriptables {
    public abstract class ScriptableGrid : ScriptableObject {
        [SerializeField] protected NodeBase nodeBasePrefab;
        [SerializeField,Range(0,6)] private int _obstacleWeight = 3;
        public abstract Dictionary<Vector2, NodeBase> GenerateGrid();
        
        protected bool DecideIfObstacle() => Random.Range(1, 20) > _obstacleWeight;
    }
}
