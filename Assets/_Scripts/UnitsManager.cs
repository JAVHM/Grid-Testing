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
    public bool isNpcTurn = false;

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
        isNpcTurn = true;
        Unit[] units = FindObjectsOfType<Unit>(); 

        foreach (Unit unit in npcUnits) 
        {
            if(unit._isNpc)
            {
                NodeBase node = unit._actualNode;
                (List<NodeBase> path, var costs) = Pathfinding._Scripts.Pathfinding.FindNearestEnemyNode(node, units, unit._team);
                if (path != null)
                {
                    if (path.Count > 1)
                    {
                        yield return new WaitForSeconds(2f);
                        node.NodeIsSelected();
                        yield return new WaitForSeconds(2f);
                        if (costs[costs.Count - 2] <= unit._movements)
                            path[path.Count - (path.Count - 1)].NodeIsMoved();
                        else
                        {
                            int index = 0;
                            foreach(int cost in costs)
                            {
                                // print(cost + " > " + unit._movements + "index: " + (index + 1));
                                if (cost > unit._movements)
                                    break;
                                index++;
                            }
                            path[path.Count - index].NodeIsMoved();
                        }
                    }
                }
            }
        }
        isNpcTurn = false;
    }
}
