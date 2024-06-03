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

    void Start()
    {
        
    }

    // Update is called once per frame
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
            NodeBase node = unit.actualNode;
            List<NodeBase> path = Pathfinding._Scripts.Pathfinding.FindNearestEnemyNode(node, units);
            yield return new WaitForSeconds(3);
            node.NodeIsSelected();
            yield return new WaitForSeconds(3);
            path[path.Count - (path.Count - 1)].NodeIsMoved();
            yield return new WaitForSeconds(3);
        }
    }
}
