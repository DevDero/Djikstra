using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;
    [SerializeField] GameObject nodeInstance;
    [SerializeField] GameObject noodPool;

    public Queue<GameObject> nodePool;
    private int minNodePool=100;

    private void Start()
    {
        instance = this;
        CreatePool();

    }
    public void CreatePool()
    {
        nodePool = new Queue<GameObject>(minNodePool);

        for (int i = 0; i < minNodePool; i++)
        {
            GameObject go =
                GameObject.Instantiate<GameObject>(nodeInstance);
            go.name = ("node " + i);
            go.SetActive(false);
            go.transform.SetParent(noodPool.transform);
            nodePool.Enqueue(go);
        }
    }
}