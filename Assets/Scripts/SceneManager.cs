using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    [SerializeField] GameObject nodeInstance;
    [SerializeField] GameObject noodPool;
    public Stack<GameObject> nodePool;
    private int minNodePool = 100;
    private void Start()
    {
        instance = this;
        CreatePool();
    }
    public void CreatePool()
    {
        nodePool = new Stack<GameObject>(minNodePool);

        for (int i = minNodePool; i > 0; i--)
        {
            GameObject go =
                GameObject.Instantiate<GameObject>(nodeInstance);
            go.name = ("node " + i);
            go.SetActive(false);
            go.transform.SetParent(noodPool.transform);
            nodePool.Push(go);
        }
    }
    public void RunDjikstra()
    {
        NodeSet.RunDjikstra();
    }
}