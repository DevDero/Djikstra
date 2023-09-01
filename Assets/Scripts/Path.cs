using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Path : MonoBehaviour
{
    public GameObject innerPath;
    public float Weight
    {
        get
        {
            return GetComponent<RectTransform>().sizeDelta.y;
        }
    }

    public Rect ScaleOuterPath()
    {
        return new Rect();
    }
    public Rect ScaleInnerPath()
    {
        return new Rect();
    }
    public void DrawInnerPath()
    {
        innerPath.GetComponent<RectTransform>().sizeDelta = new Vector2(4, GetComponent<RectTransform>().sizeDelta.y);
    }
}