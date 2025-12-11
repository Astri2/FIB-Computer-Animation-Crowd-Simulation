using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PathFinding;

public class GridCell : Node 
{
    protected float xMin;
    protected float xMax;
    protected float zMin;
    protected float zMax;

    public bool occupied { get; protected set; }

    public Vector3 center { get; protected set; }

    public GridCell(int id, int i, int j, Vector3 origin, Vector3 cellSize, float obstacleProb):base(id) {
		// i is x. j is z. bottom is bottom left
		this.xMin = origin.x + cellSize.x * j;
		this.xMax = this.xMin + cellSize.x;
		this.zMin = origin.z + cellSize.z * i;
		this.zMax = this.zMax + cellSize.z;
		this.center = new Vector3(this.xMin + cellSize.x / 2.0f, origin.y, this.zMin + cellSize.z / 2.0f);

		if (obstacleProb < 0.0f || obstacleProb > 1.0f) Debug.LogWarning("Obstacle probability is not between 0 and 1: " + obstacleProb.ToString());
		this.occupied = (Random.Range(0.0f, 1.0f) < obstacleProb);
	}
	public GridCell(GridCell n):base(n) {
		this.xMin = n.xMin;
		this.xMax = n.xMax;
		this.zMin = n.zMin;
		this.zMax = n.zMax;
		this.occupied = n.occupied;
		this.center = n.center;
	}
};
