using Nodes.Tiles;
using UnityEngine;

namespace Pathfinding._Scripts.Units {
    public class Unit : MonoBehaviour {
        [SerializeField] private SpriteRenderer _renderer;
        public NodeBase actualNode;
        public Sprite _sprite;
        public bool _isNpc =  true;

        public void Init(Sprite sprite) {
            _renderer.sprite = sprite;
        }
    }
}
