using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodeSet
{
    private static Node finalNode;
    private static Node startNode;
    static Node currentNode;
    static List<Node> nodes;

    public static Node FinalNode
    {
        get => finalNode;
        set
        {
            if (finalNode)
            {
                FinalNode.SetColor = Color.white;
            }
            value.SetColor = Color.red;
            finalNode = value;
        }
    }
    public static Node StartNode
    {
        get => startNode;
        set
        {
            if (startNode)
            {
                startNode.SetColor = Color.white;
            }
            value.SetColor = Color.green;
            startNode = value;
        }
    }
    public static void RunDjikstra()
    {
        startNode.Distance = 0;
        currentNode = startNode;
        while (currentNode != finalNode)
        {
            currentNode = currentNode.CalculaTentativeDistance();
        }
    }
}
