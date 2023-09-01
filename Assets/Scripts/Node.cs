using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Node : MonoBehaviour
{
    public TMP_Text distanceText;
    private Dictionary<Path, Node> NodePathPairs = new Dictionary<Path, Node>();
    public Dictionary<Path, Node> NodePathPair { get => NodePathPairs; }
    private Node nodeTarget;
    private bool hasVisited = false;
    public bool HasVisited
    {
        get => hasVisited;
        set
        {
            hasVisited = value;
            if (value)
            {
                SetColor = UnityEngine.Color.yellow;
                ClearPaths(false);
            }
        }
    }
    private Coroutine pathMode;
    private Path pathCache;
    private float distance = float.MaxValue;
    public float Distance
    {
        get => distance;
        set
        {
            distance = value;
            distanceText.text = Mathf.Round(value).ToString();
        }
    }

    public GameObject pathParent, pathFab;
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
    public UnityEngine.Color SetColor
    {
        set
        {
            gameObject.GetComponent<Image>().color = value;
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
    public void DeletePath(Path path)
    {
        NodePathPair.Remove(path);
        GameObject.Destroy(path.gameObject);
    }
    public void ClearPaths(bool DeletePaths)
    {
        if (NodePathPairs.Count > 0)
        {
            foreach (var item in NodePathPairs)
            {
                item.Value.NodePathPairs.Remove(item.Key);
                if (DeletePaths)
                    GameObject.Destroy(item.Key.gameObject);
            }
            NodePathPairs.Clear();
        }
    }
    private void ValidatePath(RectTransform rect)
    {
        if (!NodePathPair.ContainsValue(nodeTarget))
        {
            NodePathPairs.Add(pathCache, nodeTarget);
            nodeTarget.NodePathPairs.Add(pathCache, this);
            pathCache.transform.rotation = Lookat2D(nodeTarget.gameObject.transform.position, transform.position);
            rect.sizeDelta = new Vector2(10, Vector2.Distance(nodeTarget.gameObject.transform.position, transform.position));
            pathCache = null;
            nodeTarget = null;
        }
        else DropPath(rect);
    }
    private void DropPath(RectTransform rect)
    {
        rect.sizeDelta = new Vector2(10, 0);
        nodeTarget = null;
    }
    private IEnumerator PathMode(Path currentPath)
    {
        var image = gameObject.GetComponent<Image>();
        UnityEngine.Color color = image.color;

        var tempRect = pathCache.GetComponent<RectTransform>();
        currentPath.transform.localPosition = Vector3.zero;
        while (!Input.GetMouseButtonUp(0))
        {
            pathCache.transform.rotation = Lookat2D(Input.mousePosition, transform.position);
            tempRect.sizeDelta = new Vector2(10, Vector2.Distance(Input.mousePosition, transform.position));
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
    private Quaternion Lookat2D(Vector2 vec1, Vector2 vec2)
    {
        Vector2 dir = vec1 - vec2;
        var ang = MathF.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(ang - 90, Vector3.forward);
    }
    public Node CalculaTentativeDistance()
    {
        //visit neighbor nodes
        float shortestDistance = float.MaxValue;
        Node closestNode = null;
        foreach (var item in NodePathPairs)
        {

            float tentativeDistance = item.Key.Weight + Distance;
            if (tentativeDistance < item.Value.Distance)
            {
                item.Value.Distance = tentativeDistance;
            }
            if (tentativeDistance < shortestDistance)
            {
                shortestDistance = item.Value.Distance;
                closestNode = item.Value;
            }
        }
        NodePathPairs.FirstOrDefault(
                 (item) => (item.Value == closestNode)).Key.DrawInnerPath();
        HasVisited = true;

        return closestNode;
    }

}
