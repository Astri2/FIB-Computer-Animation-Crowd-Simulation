using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Plane))]
public class GridGenerator : MonoBehaviour
{
    [Header("Grid Size")]
    public Transform floorTransform;
    public float cellSize = 1.0f;

    [Header("Obstacles")]
    public GameObject obstaclePrefab;
    [UnityEngine.Range(0f, 1f)]
    public float obstacleProb = 0.3f;

    [Header("Grid")]
    [SerializeField]
    public bool useDiagonals = false;
    [ReadOnly]
    public Grid grid;
    
    [Header("Gizmos")]
    [SerializeField]
    private bool drawCellsAndConnections = false;

    public void Start()
    {
        // auto set the camera size to fit the plane
        Camera.main.orthographicSize = floorTransform.lossyScale.z * 5.0f;

        float minX = floorTransform.position.x - 5 * floorTransform.lossyScale.x;
        float maxX = floorTransform.position.x + 5 * floorTransform.lossyScale.x;
        float minZ = floorTransform.position.z - 5 * floorTransform.lossyScale.z;
        float maxZ = floorTransform.position.z + 5 * floorTransform.lossyScale.z;
        float height = floorTransform.position.y;
        
        grid = new Grid(minX, maxX, minZ, maxZ, cellSize, obstacleProb, height, useDiagonals);

        GameObject obstacles = new GameObject("obstacles");
        obstacles.transform.SetParent(floorTransform, true);

        foreach (GridCell cell in grid.nodes)
        {
            if (!cell.occupied) continue;
            Vector3 pos = cell.center;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
            GameObject obstacle = Instantiate(obstaclePrefab, pos, rotation, obstacles.transform);
            obstacle.name = cell.id.ToString();
            if(cellSize != 1.0f)
            {
                obstacle.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
            }
        }
        
    }

    public void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;
        if(grid == null) return;
        if(grid.nodes == null) return;
        if(grid.connections == null) return;

        if(drawCellsAndConnections)
        {
            Gizmos.color = Color.blue;
            foreach (GridConnections gridConnections in grid.connections)
            {
                foreach (CellConnection connection in gridConnections.connections)
                {
                    GridCell cell1 = connection.fromNode;
                    GridCell cell2 = connection.toNode;
                    // We added connections in both ways, but we only want to draw one
                    if (cell1.id < cell2.id) continue;

                    Gizmos.DrawLine(cell1.center, cell2.center);
                }
            }

            foreach (GridCell cell in grid.nodes)
            {
                Gizmos.color = cell.occupied ? Color.red : Color.green;
                Gizmos.DrawSphere(cell.center, cellSize / 4.0f);
            }
        }
    }
}
