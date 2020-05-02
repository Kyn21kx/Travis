﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public int gridX, gridY;
    public bool isObstacle;
    public Vector3 position;
    public Node parent;
    public int gCost;
    public int hCost;
    public int FCost { get { return gCost + hCost; } }

    public Node (bool _isObstacle, Vector3 _pos, int _gridX, int _gridY) {
        isObstacle = _isObstacle;
        position = _pos;
        gridX = _gridX;
        gridY = _gridY;
    }

}
