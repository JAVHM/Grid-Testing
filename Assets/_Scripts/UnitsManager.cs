using Nodes.Tiles;
using Pathfinding._Scripts.Grid;
using Pathfinding._Scripts.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance;
    public List<Unit> npcUnits = new List<Unit>();

    void Awake() => Instance = this;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Test());
        }
    }

    IEnumerator Test()
    {
        Unit[] units = FindObjectsOfType<Unit>(); 

        foreach (Unit unit in npcUnits) 
        {
            NodeBase node = unit._actualNode;
            List<NodeBase> path = Pathfinding._Scripts.Pathfinding.FindNearestEnemyNode(node, units);
            yield return new WaitForSeconds(3);
            node.NpcNodeIsSelected();
            yield return new WaitForSeconds(1);
            if(path.Count <= unit._movements)
                path[path.Count - (path.Count - 1)].NodeIsMoved();
            else
                path[path.Count - unit._movements].NodeIsMoved();

            yield return new WaitForSeconds(2);
        }
    }
}
