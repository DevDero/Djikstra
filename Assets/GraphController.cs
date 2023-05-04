using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using System;

public class GraphController : MonoBehaviour, GraphActions
{

    List<RaycastResult> raycastResultList = new List<RaycastResult>();
    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
    Coroutine clickBuffer;

    Node selectedNode;
    bool isPathMode;

    private void Update()
    {
        NodeCast();
    }

    public void NodeCast()
    {
        pointerEventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        if (Input.GetMouseButtonDown(0) && clickBuffer == null)
        {
            clickBuffer = StartCoroutine(NodeClickRoutine());   
        }
    }

    public IEnumerator NodeClickRoutine()
    {
        float stallTime = .15f;
        int maxinput = 5;

        while (isPathMode)
        {
            if (Input.GetMouseButtonUp(0))
                PathCommand();
            yield return null;
        }

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
    }
    public void AssignSource(Node node)
    {
    }    
    public void PlaceNode(Vector3 position)
    {
        GameObject go = SceneManager.instance.nodePool.Dequeue();
        go.transform.position = position;
        go.SetActive(true);

    }
    public void DeleteNode(GameObject gameObject)
    {
        SceneManager.instance.nodePool.Enqueue(gameObject);
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
                        if (commandEventData.clickTime > .2)
                        {
                            CreatePath(selectedNode = lastHit.GetComponent<Node>());
                            isPathMode = true;
                        }
                        else if (commandEventData.clickCount > 1)                  
                            DeleteNode(raycastResultList[0].gameObject);                      
                        break;
                    case "Path":
                        if (commandEventData.clickCount > 1)
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


    private bool FilterNodes(RaycastResult go)
    {
        
        if (go.gameObject.GetComponent<Node>() != null)
            return false;
        else
            return true;
    }
}