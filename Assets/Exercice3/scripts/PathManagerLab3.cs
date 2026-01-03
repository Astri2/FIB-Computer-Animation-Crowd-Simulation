using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PathManagerLab3 : MonoBehaviour
{

    public Grid grid;

    [SerializeField]
    private AgentLab3 agent;

    public Vector3 currentGoal;
    
    [SerializeField]
    private List<GridCell> path;
    
    [SerializeField]
    private int pathIndex;

    [SerializeField]
    private bool movingToPathEnd = false;

    [SerializeField]
    private bool shouldFindNewGoal = false;

    public bool drawGizmos = true;

    public GridCell FindSuitableCellInGrid(int nbMaxTries = 10)
    {
        int nbTries = 0;
        while (nbTries < nbMaxTries)
        {
            // find free goal location
            int i = Random.Range(0, grid.GetGridCols());
            int j = Random.Range(0, grid.GetGridRows());

            if (!grid.IsCellFree(i, j))
            {
                nbTries++;
                continue;
            }

            // retrieve the cell
            GridCell goal = grid.getNode(grid.GridCoordToId(i, j));

            if (goal == null)
            {
                Debug.LogWarning("Could not find start or goal nodes of path finding");
                nbTries++;
                continue;
            }

            return goal;
        }

        return null;
    }

    private void SetNewGoal()
    {
        if (grid == null)
        {
            Debug.LogError("No grid given to the pathManager.");
            return;
        }

        Grid_A_Star gridAStar = new Grid_A_Star(-1, -1, -1);
        int found = -1;

        // Find suitable goal location
        GridCell current = grid.getNode(grid.CoordToId(agent.transform.position));
        if (current == null)
        {
            Debug.LogError("Could not find start node of path finding");
            return;
        }
        
        int nbTries = 0;
        while (nbTries < 10)
        {
            GridCell goal = FindSuitableCellInGrid();

            // check if a path exists
            GridHeuristic heuristic = new GridHeuristic(goal, grid.UseDiagonals());
            path = gridAStar.findpath(grid, current, goal, heuristic, ref found);
            if (found <= 0 || path.Count < 2)
            {
                path = null;
                nbTries++;
                continue;
            }

            pathIndex = 1; // skip begin
            currentGoal = path[pathIndex].center;
            shouldFindNewGoal = false;
            movingToPathEnd = (pathIndex == path.Count - 1);

            return;
        }
    }

    private void Start()
    {
        agent = GetComponent<AgentLab3>();

        shouldFindNewGoal = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shouldFindNewGoal)
        {
            SetNewGoal();
            if (shouldFindNewGoal) return;
        }

        int goalCellId = grid.CoordToId(currentGoal);
        int currentCellId = grid.CoordToId(agent.transform.position);

        // TODO: regarding below, the last goal should be a random place in the cell, and the goal detection should use cell detection for all nodes, except last node which should used distanced detection

        // replace distance based goal detection, to cell based goal detection => avoid agents converging to the same cell center point
        if(goalCellId == currentCellId) {
        /*
        float distanceToGoal = new Vector2(agent.rb.position.x - currentGoal.x, agent.rb.position.z - currentGoal.z).magnitude;
        if(distanceToGoal <= agent.velocity.magnitude * Time.fixedDeltaTime / 1.5f) // should be 2.0, but adding more flexibility
        {
        */
            // if the agent arrived to the last node, create a new path
            if (pathIndex == path.Count - 1)
            {
                shouldFindNewGoal = true;
                agent.RandomizeColorAndSpeed();
            }
            // otherwise aim to the next node
            else
            {
                pathIndex++;
                currentGoal = path[pathIndex].center;
                movingToPathEnd = (pathIndex == path.Count - 1);
            }
        }
        
        if (shouldFindNewGoal) SetNewGoal();
    }

    public void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        if (path == null) return;

        Gizmos.color = Color.blueViolet;

        // draw a line to the current goal
        Gizmos.DrawLine(agent.transform.position, currentGoal);

        for (int i = pathIndex ; i < path.Count - 1 ; i++)
        {
            Gizmos.DrawSphere(path[i].center, 0.2f);
        }

        Gizmos.color = Color.violetRed;
        Gizmos.DrawSphere(path[^1].center, 0.2f);
    }
}
