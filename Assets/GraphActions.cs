using UnityEngine;

public interface GraphActions
{
    public void PlaceNode(Vector3 position);

    public void DeleteNode(GameObject gameObject);

    public void CreatePath(Node node);

    public void DeletePath(Path node);

    public void AssignFinish(Node node);

    public void AssignSource(Node node);
}