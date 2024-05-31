using Nodes.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(0);
        }

        if (Input.GetMouseButtonDown(1))
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

            node.NodeIsSelectect();
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

            node.NodeIsMapped();
        }
    }
}
