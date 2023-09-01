using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using System;
using UnityEditor.Experimental.GraphView;

public class GraphController : MonoBehaviour, GraphActions
{
    List<RaycastResult> raycastResultList = new List<RaycastResult>();
    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
    Coroutine clickBuffer;
    Node selectedNode;
    bool isPathMode;
    private void Update()
    {
        InputCast();
    }
    private bool FilterNodes(RaycastResult go)
    {
        if (go.gameObject.GetComponent<Node>() != null)
            return false;
        else
            return true;
    }
    public void InputCast()
    {
        pointerEventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        if (Input.GetMouseButtonDown(0) && clickBuffer == null && !isPathMode)
        {
            clickBuffer = StartCoroutine(NodeClickRoutine());
        }
        if (isPathMode)
            StartCoroutine(PathRoutine());
    }
    public IEnumerator PathRoutine()
    {
        while (isPathMode)
        {
            if (Input.GetMouseButtonUp(0))
                PathCommand();
            yield return null;
        }
    }
    public IEnumerator NodeClickRoutine()
    {
        float stallTime = .15f;
        int maxinput = 5;
        while (stallTime > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pointerEventData.clickCount++;
                stallTime = .15f;
            }
            if (Input.GetMouseButton(0))
            {
                pointerEventData.clickTime += Time.deltaTime;
                stallTime = .15f;
            }
            if (pointerEventData.clickCount == maxinput && pointerEventData.clickTime > stallTime * 2)
            {
                break;
            }
            else
                stallTime -= Time.deltaTime;
            yield return null;
        }
        ExecuteCommand(pointerEventData);
    }
    public void AssignFinish(Node node)
    {
        NodeSet.FinalNode = node;
    }
    public void AssignSource(Node node)
    {
        NodeSet.StartNode = node;
    }
    public void PlaceNode(Vector3 position)
    {
        GameObject go = SceneManager.instance.nodePool.Pop();
        go.transform.position = position;
        go.SetActive(true);
    }
    public void DeleteNode(GameObject gameObject)
    {
        gameObject.GetComponent<Node>().ClearPaths(true);
        SceneManager.instance.nodePool.Push(gameObject);
        gameObject.SetActive(false);
    }
    public void CreatePath(Node sourceNode)
    {
        sourceNode.CreatePath();
    }
    public void DeletePath(Path path)
    {
        path.GetComponentInParent<Node>().DeletePath(path);
    }
    public void PathCommand()
    {
        GameObject lastHit = null;
        //filter non node raycasts
        raycastResultList.RemoveAll((FilterNodes));
        if (raycastResultList.Count > 0)
        {
            lastHit = raycastResultList[0].gameObject;
            selectedNode.NodeTarget = lastHit.GetComponent<Node>();
        }
        clickBuffer = null;
        isPathMode = false;
    }
    public void ExecuteCommand(PointerEventData commandEventData)
    {
        GameObject lastHit = null;
        if (raycastResultList.Count > 0)
            lastHit = raycastResultList[0].gameObject;
        if (!isPathMode)
        {
            if (lastHit != null)
            {
                switch (lastHit.tag)
                {
                    case "Node":
                        if (commandEventData.clickTime > .2 && commandEventData.clickCount == 1)
                        {
                            CreatePath(selectedNode = lastHit.GetComponent<Node>());
                            isPathMode = true;
                        }
                        else if (commandEventData.clickCount == 2)
                            DeleteNode(raycastResultList[0].gameObject);
                        else if (commandEventData.clickCount == 3)
                            AssignSource(lastHit.GetComponent<Node>());
                        else if (commandEventData.clickCount == 4)
                            AssignFinish(lastHit.GetComponent<Node>());
                        break;
                    case "Path":
                        if (commandEventData.clickCount == 2)
                            DeletePath(lastHit.GetComponent<Path>());
                        break;
                }
            }
            else if (raycastResultList.Count < 1 && !isPathMode)
                PlaceNode(commandEventData.position);
        }
        pointerEventData = new PointerEventData(EventSystem.current);
        clickBuffer = null;
    }
}