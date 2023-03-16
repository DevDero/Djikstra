using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class Node : MonoBehaviour
{

    public Dictionary<GameObject , Node > pathPair = new Dictionary<GameObject, Node>();
    NodeType type;
    Vector3 distance;
    bool hasVisited;
    Node[] adjacentNodes;

    GameObject path,pathParent;

    public GameObject CreateBind(Vector2 nextNodePos)
    {
        GameObject go = GameObject.Instantiate(path);

        go.transform.SetParent(pathParent.transform);

        RectTransform tempRect = go.GetComponent<RectTransform>();
        tempRect.transform.eulerAngles = new Vector3(0 , 0 ,Vector2.SignedAngle(Vector2.zero, nextNodePos));

        float phytag = Mathf.Sqrt(nextNodePos.x * nextNodePos.x + nextNodePos.y + nextNodePos.x);

        tempRect.sizeDelta = new Vector2(10, phytag);
        tempRect.anchoredPosition = nextNodePos;

        return go;
    }
    public void DeleteBind(GameObject go)
    {


    }
    
    public void BindingMode()
    {
        gameObject.GetComponent<Image>().tintColor = UnityEngine.Color.blue;

        
    }
}
