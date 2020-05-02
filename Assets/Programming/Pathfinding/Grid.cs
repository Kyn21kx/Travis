using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public Transform startPos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float distance;
    Node[,] grid;
    public List<Node> finalPath;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Start() {
        nodeDiameter = nodeRadius * 2f;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid () {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2f - Vector3.forward * gridWorldSize.y / 2f;
        for (int i = 0; i < gridSizeX; i++) {
            for (int j = 0; j < gridSizeX; j++) {
                Vector3 worldPoint = bottomLeft + Vector3.right * (j * nodeDiameter + nodeRadius) + Vector3.forward * (i * nodeDiameter + nodeRadius);
                bool obstacle = true;
                if (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask)) {
                    obstacle = false;
                }
                grid[i, j] = new Node(obstacle, worldPoint, j, i);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 4, gridWorldSize.y));
        if (grid != null) {
            foreach (var node in grid) {
                if (node.isObstacle) {
                    Gizmos.color = Color.white;
                }
                else {
                    Gizmos.color = Color.yellow;
                }
                if (finalPath != null) {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawCube(node.position, Vector3.one * (nodeDiameter - distance));
            }
        }
    }

}
