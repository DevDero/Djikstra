using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public GameObject pathParent, pathFab;

    private float distance;


    private Dictionary<Path , Node > NodePathPairs = new Dictionary<Path, Node>();
    private Node previousNode, nodeTarget;

    private NodeType type;
    private bool hasVisited = false;
    private Coroutine pathMode;
    private Path pathCache;

    public Dictionary<Path, Node> NodePathPair { get => NodePathPairs; }
    public Node NodeTarget
    {
        get => nodeTarget;
        set
        {
            //filter assigning node it self
            if (value != this)
                nodeTarget = value;
        }
    }

    public void CreatePath()
    {
        if (pathCache == null) 
        {
            GameObject path = GameObject.Instantiate(pathFab);
            path.name = pathParent.transform.childCount + "path";
            path.transform.SetParent(pathParent.transform);
            pathMode = StartCoroutine(PathMode(pathCache = path.GetComponent<Path>()));
        }
        else
            pathMode = StartCoroutine(PathMode(pathCache));
    }

    private void ValidatePath(RectTransform rect)
    {
        if (!NodePathPair.ContainsValue(nodeTarget))
        {
            //reciproc insert paths
            NodePathPairs.Add(pathCache, nodeTarget);
            nodeTarget.NodePathPairs.Add(pathCache, this);

            pathCache = null;
            nodeTarget = null;
        }
        else DropPath(rect);
    }

    private void DropPath(RectTransform rect)
    {
        rect.sizeDelta = new Vector2(4, 0);
        nodeTarget = null;
    }

    public void DeletePath(Path path)
    {
        NodePathPair.Remove(path);
        GameObject.Destroy(path.gameObject);
    }

    
    private IEnumerator PathMode(Path currentPath)
    {
        UnityEngine.Color color = UnityEngine.Color.white;
        var image = gameObject.GetComponent<Image>();
        var tempRect = pathCache.GetComponent<RectTransform>();
        currentPath.transform.localPosition = Vector3.zero;

        while (!Input.GetMouseButtonUp(0))
        {
            pathCache.transform.rotation = Lookat2D(Input.mousePosition, transform.position);
            tempRect.sizeDelta = new Vector2(4, Vector2.Distance(Input.mousePosition,transform.position));

            color.a = Mathf.Sin(Time.time * 10);
            image.color = color;
            yield return null;

        }
        color.a = 1;
        image.color = color;

        if (nodeTarget != null)
            ValidatePath(tempRect); 
        else
            DropPath(tempRect);

    }
    private Quaternion Lookat2D(Vector2 vec1,Vector2 vec2)
    {
        Vector2 dir = vec1 - vec2;
        var ang = MathF.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(ang-90, Vector3.forward);
    }

    private void CoreMethod()
    {
        foreach( var  pair in NodePathPairs)
        {
            float tentativeDist = this.distance + pair.Key.Delta;

            if (pair.Value.distance > tentativeDist && !pair.Value.hasVisited)
            {
                pair.Value.distance = tentativeDist;
            }
        }
        this.hasVisited = true;
    }
    
}

