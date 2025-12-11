using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PathFinding;

[System.Serializable]
public class GridHeuristic : Heuristic<GridCell>
{

	[SerializeField]
	[ReadOnly]
	private bool useEuclidian;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="goal"></param>
	/// <param name="useEuclidian">use L2 distance, or L1 if false</param>
	public GridHeuristic(GridCell goal, bool useEuclidian):base(goal){
		goalNode = goal;
		this.useEuclidian = useEuclidian;
	}
	
	 // generates an estimated cost to reach the stored goal from the given node
	public override float estimateCost(GridCell fromNode) {
		// L2
		if(useEuclidian) 
			return Vector3.Distance(goalNode.center, fromNode.center);

		// L1
		Vector3 diff = fromNode.center - goalNode.center;
		return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) + Mathf.Abs(diff.z);


    }

	// determines if the goal node has been reached by node
	public override bool goalReached(GridCell node){
		return goalNode.id == node.id;
	}

};
