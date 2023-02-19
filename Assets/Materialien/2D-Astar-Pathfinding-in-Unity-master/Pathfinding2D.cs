using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Pathfinding2D : MonoBehaviour
{
    static Node2D seekerNode, targetNode;

    static Vector3 gridWorldSize;
    static Node2D[,] grid;
    static Tilemap obstaclemap;
    static Vector3 worldBottomLeft;

    static float nodeRadius = 0.5f;
    static float nodeDiameter;
    static int gridSizeX, gridSizeY;

    static Vector3 pathfindingSubjectOffset = new Vector3(0.25f, 0.5f);

    public static void LoadMap(Tilemap tilemap, int worldSizeX, int worldSizeY)
    {
        Pathfinding2D.obstaclemap = tilemap;
        gridWorldSize = new Vector3(worldSizeX, worldSizeY, 0);
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public static List<Node2D> FindPath(Vector3 startPosition, Vector3 finishPosition)
    {
        if(grid == null)
        {
            return new List<Node2D>() { };
        }

        //get player and target position in grid coords
        seekerNode = NodeFromWorldPoint(startPosition);
        targetNode = NodeFromWorldPoint(finishPosition);

        if (seekerNode == null)
        {
            // Outside of map
            // Debug.Log("Seeker outside of map");
            return new List<Node2D>() { NodeFromWorldPoint(Vector2.zero) };
        }
        if (targetNode == null)
        {
            // Outside of map
            // Debug.Log("Target outside of map");
            return new List<Node2D>() { };
        }

        List<Node2D> openSet = new List<Node2D>();
        HashSet<Node2D> closedSet = new HashSet<Node2D>();
        openSet.Add(seekerNode);

        //calculates path for pathfinding
        while (openSet.Count > 0)
        {

            //iterates through openSet and finds lowest FCost
            Node2D node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost <= node.FCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            //If target found, retrace path
            if (node == targetNode)
            {
                return RetracePath(seekerNode, targetNode, startPosition);
            }

            //adds neighbor nodes to openSet
            foreach (Node2D neighbour in GetNeighbors(node))
            {
                if (neighbour.obstacle || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // No path found
        return new List<Node2D>() { };
    }

    //reverses calculated path so first node is closest to seeker
    static List<Node2D> RetracePath(Node2D startNode, Node2D endNode, Vector3 startPosition)
    {
        List<Node2D> path = new List<Node2D>();
        Node2D currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        for (int i = 0; i < path.Count; i++)
        {
            Node2D node = path[i];
            if (node.worldPosition == startPosition)
            {
                path.Remove(node);
                i--;
            }
        }
        if (path.Count < 1)
        {
            path.Add(endNode);
        }

        return path;
    }

    //gets distance between 2 nodes for calculating cost
    static int GetDistance(Node2D nodeA, Node2D nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    static void CreateGrid()
    {
        grid = new Node2D[gridSizeX, gridSizeY];
        worldBottomLeft = obstaclemap.transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                grid[x, y] = new Node2D(false, worldPoint, x, y);

                if (obstaclemap.HasTile(obstaclemap.WorldToCell(grid[x, y].worldPosition)))
                    grid[x, y].SetObstacle(true);
                else
                    grid[x, y].SetObstacle(false);


            }
        }
    }

    //gets the neighboring nodes in the 4 cardinal directions. If you would like to enable diagonal pathfinding, uncomment out that portion of code
    private static List<Node2D> GetNeighbors(Node2D node)
    {
        List<Node2D> neighbors = new List<Node2D>();

        //checks and adds top neighbor
        if (node.gridX >= 0 && node.gridX < gridSizeX && node.gridY + 1 >= 0 && node.gridY + 1 < gridSizeY)
            neighbors.Add(grid[node.gridX, node.gridY + 1]);

        //checks and adds bottom neighbor
        if (node.gridX >= 0 && node.gridX < gridSizeX && node.gridY - 1 >= 0 && node.gridY - 1 < gridSizeY)
            neighbors.Add(grid[node.gridX, node.gridY - 1]);

        //checks and adds right neighbor
        if (node.gridX + 1 >= 0 && node.gridX + 1 < gridSizeX && node.gridY >= 0 && node.gridY < gridSizeY)
            neighbors.Add(grid[node.gridX + 1, node.gridY]);

        //checks and adds left neighbor
        if (node.gridX - 1 >= 0 && node.gridX - 1 < gridSizeX && node.gridY >= 0 && node.gridY < gridSizeY)
            neighbors.Add(grid[node.gridX - 1, node.gridY]);



        /* Uncomment this code to enable diagonal movement
        
        //checks and adds top right neighbor
        if (node.GridX + 1 >= 0 && node.GridX + 1< gridSizeX && node.GridY + 1 >= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY + 1]);

        //checks and adds bottom right neighbor
        if (node.GridX + 1>= 0 && node.GridX + 1 < gridSizeX && node.GridY - 1 >= 0 && node.GridY - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX + 1, node.GridY - 1]);

        //checks and adds top left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY + 1>= 0 && node.GridY + 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY + 1]);

        //checks and adds bottom left neighbor
        if (node.GridX - 1 >= 0 && node.GridX - 1 < gridSizeX && node.GridY  - 1>= 0 && node.GridY  - 1 < gridSizeY)
            neighbors.Add(Grid[node.GridX - 1, node.GridY - 1]);
        */

        return neighbors;
    }

    private static Node2D NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x - pathfindingSubjectOffset.x + (gridSizeX / 2.0f));
        int y = Mathf.RoundToInt(worldPosition.y - pathfindingSubjectOffset.y + (gridSizeY / 2.0f));

        if (x > grid.GetLength(0) - 1 || y > grid.GetLength(1) - 1 || x < 0 || y < 0)
        {
            // Outside of range
            return null;
        }
        return grid[x, y];
    }

    public static void DrawGizmoz(Transform targetTransform, List<Node2D> path)
    {
        Gizmos.DrawWireCube(targetTransform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null)
        {
            foreach (Node2D n in grid)
            {
                if (path != null && path.Contains(n))
                {
                    Gizmos.color = Color.black;
                }
                else
                {
                    if (n.obstacle)
                        Gizmos.color = Color.red;
                    else
                        //Gizmos.color = Color.white;
                        continue;
                }

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius));

            }
        }
    }

}