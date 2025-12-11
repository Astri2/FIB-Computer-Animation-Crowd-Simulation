using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(GridGenerator))]
public class GlobalAStar: MonoBehaviour
{
    [Header("PathFinding")]
    [SerializeField]
    [ReadOnly]
    private int startId = -1;
    [SerializeField]
    [ReadOnly]
    private int goalId = -1;

    [Header("Gizmos")]
    [SerializeField]
    private bool drawPathFinding = false;

    private Transform floorTransform;
    private GridGenerator generator;
    private bool useDiagonals;
    private float cellSize;

    private Grid grid;
    
    private bool shouldUpdatePath;
    private List<GridCell> path;

    public void Start()
    {
        generator = GetComponent<GridGenerator>();

        if(generator == null)
        {
            Debug.LogWarning("Could not retrieve generator");
        }

        // will not update during runtime
        floorTransform = generator.floorTransform;
        useDiagonals = generator.useDiagonals; 
        cellSize = generator.cellSize;

        // copy floor collider for onHover
        MeshCollider collider = this.AddComponent<MeshCollider>();
        collider.sharedMesh = floorTransform.GetComponent<MeshCollider>().sharedMesh;

        shouldUpdatePath = false;
        path = new List<GridCell>();
    }

    public void Update()
    {
        // try to retrieve the grid
        if(grid == null)
        {
            grid = generator.grid;
            if (grid == null) return;
        }
        
        if (!shouldUpdatePath) return;

        shouldUpdatePath = false;
        path.Clear();

        Grid_A_Star gridAStar = new Grid_A_Star(-1, -1, -1);
        int found = -1;

        GridCell start = grid.getNode(startId);
        GridCell goal = grid.getNode(goalId);

        if (start == null || goal == null)
        {
            Debug.LogWarning("Could not find start or goal nodes of path finding");
            return;
        }

        GridHeuristic heuristic = new GridHeuristic(goal, useDiagonals);
        path = gridAStar.findpath(grid, start, goal, heuristic, ref found);
    }

    public void OnMouseOver()
    {

        Debug.Log("Over");
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) return;
        
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos = new Vector3(pos.x, floorTransform.position.y, pos.z);

        // set pos origin on origin of the plane
        pos -= grid.getOrigin();
        // convert float coordinates to grid coordinates
        pos /= cellSize;
        Vector3Int gridCoords = Vector3Int.FloorToInt(pos);

        int id = grid.gridCoordToId(gridCoords.z, gridCoords.x);

        GridCell node = grid.getNode(id);
        if (node == null)
        {
            Debug.Log("Could not find node:" + id.ToString());
            return;
        }

        if (node.occupied) return;

        if (Input.GetMouseButtonDown(0)) startId = id;
        else goalId = id;

        // recompute path only if the nodes have changed
        if (startId != -1 && goalId != -1) shouldUpdatePath = true;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (grid == null) return;
        if (grid.nodes == null) return;
        if (grid.connections == null) return;
        if (path == null) return;

        if (drawPathFinding)
        {
            Gizmos.color = Color.orange;
            if (startId != -1)
            {

                GridCell start = grid.getNode(startId);
                if (start != null) Gizmos.DrawWireSphere(start.center, cellSize / 3.0f);
            }

            if (goalId != -1)
            {

                GridCell goal = grid.getNode(goalId);
                if (goal != null) Gizmos.DrawSphere(goal.center, cellSize / 3.0f);
            }

            // draw path
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].center, path[i + 1].center);
            }
        }
    }
}

