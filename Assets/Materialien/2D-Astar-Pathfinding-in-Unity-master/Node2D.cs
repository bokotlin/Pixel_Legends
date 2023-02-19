using UnityEngine;

public class Node2D
{
    public int gCost, hCost;
    public bool obstacle;
    public Vector3 worldPosition;

    public int gridX, gridY;
    public Node2D parent;

    public Node2D(bool _obstacle, Vector3 _worldPos, int _gridX, int _gridY)
    {
        obstacle = _obstacle;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }

    }

    public void SetObstacle(bool isOb)
    {
        obstacle = isOb;
    }

}
