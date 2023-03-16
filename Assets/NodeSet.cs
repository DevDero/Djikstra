using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSet
{
    Node finalNode;
    Node startNode;
    Node currentNode;

    List<Node> nodes;
    

    public NodeSet(Node finalNode, Node startNode, List<Node> nodes)
    {
        this.finalNode = finalNode;
        this.startNode = startNode;
        this.nodes = nodes;
    }


}
public enum NodeType { final,source,common}
