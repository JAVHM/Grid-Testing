using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private bool _isSelected = false;
    public bool _isRangeAttackTest = false;

    private void Update()
    {
        if (!GridManager.Instance._isNpcTurn && !GridManager.Instance._isUnitMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleClick(0);
            }
        }

        if (!_isRangeAttackTest && Input.GetMouseButtonDown(1))
        {
            _isRangeAttackTest = true;
        }
        if (_isRangeAttackTest == true)
        {
            HandleClick2(0);

        }
    }

    private void HandleClick(int mouseButton)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;

            NodeBase node = clickedObject.GetComponent<NodeBase>();


            if (_isSelected == false && node._tileUnit != null && node._tileUnit._team == 1)
            {
                node.NodeIsSelected();
                _isSelected = true;
            }
            else if (_isSelected && (node._isWalkable || node._isInRange))
            {
                if (GridManager.Instance._currentNode == node)
                {
                    node.NodeIsUnselected();
                }
                else if (GridManager.Instance._currentNode.Neighbors.Contains(node) && node._tileUnit != null)
                {
                    node._tileUnit.GetComponent<Health>().TakeDamage(10);
                    node.NodeIsUnselected();
                }
                else
                {
                    node.NodeIsMoved();
                }
                _isSelected = false;
            }
            else
            {
                _isSelected = false;
            }
        }
    }

    private void HandleClick2(int mouseButton)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;

            NodeBase node = clickedObject.GetComponent<NodeBase>();

            GridManager.Instance.TestAreaAttack(node);
        }
    }
}
