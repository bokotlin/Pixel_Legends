using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{

    public Tilemap map;

    void Start()
    {
        Pathfinding2D.LoadMap(map, 22, 10);
    }

}
