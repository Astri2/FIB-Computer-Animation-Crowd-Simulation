using UnityEngine;

using PathFinding;

[System.Serializable]
public class Grid : FiniteGraph<GridCell, CellConnection, GridConnections>
{
    // Class that represent the finite graph corresponding to a grid of cells
    // There is a known set of nodes (GridCells), 
    // and a known set of connections (CellConnections) between those nodes (GridConnections)


    [ReadOnly]
    [SerializeField]
    protected Vector3 gridOrigin;
    
    [ReadOnly]
    [SerializeField]
    protected Vector3 gridSize;
    
    [ReadOnly]
    [SerializeField]
    protected Vector3 sizeOfCell;

    
    [ReadOnly]
    [SerializeField]
    protected int numCells;
    
    [ReadOnly]
    [SerializeField]
    protected int numRows;
    
    [ReadOnly]
    [SerializeField]
    protected int numColumns;

    [ReadOnly]
    [SerializeField]
    private float obstacleProb;

    [ReadOnly]
    [SerializeField]
    private bool useDiagonals;


    // Example Constructor function declaration
    // public Grid(float minX, float maxX, float minZ, float maxZ, float cellSize, float height = 0):base()

    // You have basically to fill the base fields "nodes" and "connections", 
    // i.e. create your list of GridCells (with random obstacles) 
    // and then create the corresponding GridConnections for each one of them
    // based on where the obstacles are and the valid movements allowed between GridCells. 
    public Grid(float minX, float maxX, float minZ, float maxZ, float cellSize, float obstacleProb, float height, bool useDiagonals) : base()
	{
        this.gridOrigin = new Vector3(minX, height, minZ);
        this.gridSize = new Vector3(maxX - minX, 0, maxZ - minZ);
		this.sizeOfCell = new Vector3(cellSize, 0, cellSize);
        this.useDiagonals = useDiagonals;
        
        this.obstacleProb = obstacleProb;

        this.numColumns = Mathf.FloorToInt((maxX - minX) / cellSize);
        this.numRows = Mathf.FloorToInt((maxZ - minZ) / cellSize);
        this.numCells = numColumns * numRows;


        // insert all nodes
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numColumns; j++)
            {
                GridCell cell = new GridCell(gridCoordToId(i, j), i, j, gridOrigin, sizeOfCell, obstacleProb);
                this.nodes.Add(cell);

                GridConnections cellConnections = new GridConnections();
                this.connections.Add(cellConnections);

                // if obstacle, don't draw connections
                if (cell.occupied) continue;

                // try to connect to previous cells (because those are already existing)
                if (i != 0) addNeighborConnection(gridCoordToId(i - 1, j), cell, cellConnections);
                if (j != 0) addNeighborConnection(gridCoordToId(i, j - 1), cell, cellConnections);
                if (useDiagonals)
                {
                    if(i != 0 && j != 0) addNeighborConnection(gridCoordToId(i - 1, j - 1), cell, cellConnections);
                    if(i != 0 && j != numColumns - 1) addNeighborConnection(gridCoordToId(i - 1, j + 1), cell, cellConnections);
                }
            }
        }
    }

	protected void addNeighborConnection(int id, GridCell cell, GridConnections cellConnections)
	{
		// sanity check
        if (cell.occupied) return;

        GridCell neighbor = this.getNode(id);

		if (neighbor == null)
		{
            Debug.LogError("Could not find neighbor cell with id: " + id.ToString());
			return;
		}
		if (neighbor.occupied) return;

        cellConnections.Add(new CellConnection(cell, neighbor));
        GridConnections neighborConnections = this.getConnections(id);
        neighborConnections.Add(new CellConnection(neighbor, cell));
    }

    public Vector3 getOrigin()
    {
        return this.gridOrigin;
    }

    public int gridCoordToId(int i, int j)
    {
        return i * this.numColumns + j;
    }
};
