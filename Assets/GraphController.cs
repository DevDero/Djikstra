using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GraphController : MonoBehaviour, GraphActions
{

    List<RaycastResult> raycastResultList = new List<RaycastResult>();
    PointerEventData pointerEventData = new PointerEventData(EventSystem.current);

    Coroutine clickBuffer;
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
                pointerEventData.delta += new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            }
            if (pointerEventData.clickCount == maxinput && pointerEventData.clickTime > .5f)
            {
                break;
            }
            else
                stallTime -= Time.deltaTime;

            yield return null;
        }
        ExecuteCommand(pointerEventData);

        pointerEventData = null;
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
    public void CreatePath(Node node)
    {
    }
    public void DeletePath(Node node)
    {
    }

    public void ExecuteCommand(PointerEventData commandEventData)
    {
        GameObject lastHit = null;
        if (raycastResultList.Count > 0)
            lastHit = raycastResultList[0].gameObject;

        if (commandEventData.clickTime > .25f && lastHit.CompareTag("Node"))
            CreatePath(lastHit.GetComponent<Node>());
        if (commandEventData.clickCount > 1 && lastHit.CompareTag("Path"))
            DeletePath(lastHit.GetComponent<Node>());

        if (raycastResultList.Count < 1) 
            PlaceNode(commandEventData.position);
        if (commandEventData.clickCount > 1 && lastHit.CompareTag("Node"))
            DeleteNode(raycastResultList[0].gameObject);

    }
   
}