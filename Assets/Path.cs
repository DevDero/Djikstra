using UnityEngine;

public class Path : MonoBehaviour
{
    GameObject outerPath, innerPath;
    float delta;

    public float Delta { get => delta; }

    public Rect ScaleOuterPath()
    {
        return new Rect();
    }

    public Rect ScaleInnerPath()
    {
        return new Rect();
    }
}