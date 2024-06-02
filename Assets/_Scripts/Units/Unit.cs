using UnityEngine;

namespace Pathfinding._Scripts.Units {
    public class Unit : MonoBehaviour {
        [SerializeField] private SpriteRenderer _renderer;
        public Sprite _sprite;    

        public void Init(Sprite sprite) {
            _renderer.sprite = sprite;
        }
    }
}
