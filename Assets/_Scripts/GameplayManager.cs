using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private bool _isSelected = false;

    private GridManager _gridManager;
    private Camera _mainCamera;

    private void Awake()
    {
        _gridManager = GridManager.Instance;
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!_gridManager._isNpcTurn && !_gridManager._isUnitMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClickMap();
            }
        }
    }

    private void ClickMap()
    {
        NodeBase node = GetNodeUnderMouse();

        if (node != null)
        {
            if (!_isSelected && node._tileUnit != null && node._tileUnit._team == 1)
            {
                node.NodeIsSelected();
                _isSelected = true;
            }
            else if (node._tileUnit != null && node._tileUnit._team == 1)
            {
                GridManager.Instance._currentNode.NodeIsUnselected();
                node.NodeIsSelected();
            }
            else if (_isSelected && node._isWalkable && node._isInRange)
            {
                if (_gridManager._currentNode == node || node._tileUnit != null)
                {
                    node.NodeIsUnselected();
                }
                else
                {
                    node.NodeIsMoved();
                }
                _isSelected = false;
            }
            else if (_isSelected && _gridManager._currentNode.Neighbors.Contains(node) && node._tileUnit != null)
            {
                node._tileUnit.GetComponent<Health>().TakeDamage(10);
                node.NodeIsUnselected();
                _isSelected = false;
            }
        }
    }

    private NodeBase GetNodeUnderMouse()
    {
        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.gameObject.GetComponent<NodeBase>();
        }

        return null;
    }
}
