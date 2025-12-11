using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathFinding{

	public class A_Star<TNode,TConnection,TNodeConnection,TGraph,THeuristic> : PathFinder<TNode,TConnection,TNodeConnection,TGraph,THeuristic>
	where TNode : Node
	where TConnection : Connection<TNode>
	where TNodeConnection : NodeConnections<TNode,TConnection>
	where TGraph : Graph<TNode,TConnection,TNodeConnection>
	where THeuristic : Heuristic<TNode>
	{
	// Class that implements the A* pathfinding algorithm
	// You have to implement the findpath function.
	// You can add whatever you need.
				
		protected List<TNode> visitedNodes; // list of visited nodes 
		
		protected NodeRecord currentBest; // current best node found
		
		protected enum NodeRecordCategory{ OPEN, CLOSED, UNVISITED };
				
		protected class NodeRecord{	
		// You can use (or not) this structure to keep track of the information that we need for each node
			
            public NodeRecord(TNode node, float costSoFar, float totalCost, NodeRecordCategory category, NodeRecord parent = null)
            {
                this.node = node;
                this.connection = parent;
                this.costSoFar = costSoFar;
                this.estimatedTotalCost = totalCost;
                this.category = category;
                this.depth = parent == null ? 0 : parent.depth + 1;
            }

            public TNode node; 
			public NodeRecord connection;	// connection traversed to reach this node 
			public float costSoFar; // cost accumulated to reach this node
			public float estimatedTotalCost; // estimated total cost to reach the goal from this node
			public NodeRecordCategory category; // category of the node: open, closed or unvisited
			public int depth; // depth in the search graph
		};

		public	A_Star(int maxNodes, float maxTime, int maxDepth):base(){ 
			
			visitedNodes = new List<TNode> ();
			
		}

		public virtual List<TNode> getVisitedNodes(){
			return visitedNodes;
		}
		


        public override List<TNode> findpath(TGraph graph, TNode start, TNode end, THeuristic heuristic, ref int found)
		{
			List<TNode> path = new List<TNode>();

            // set S, implemented as a priority queue to easily find the best node to retrieve
            PriorityQueue<NodeRecord> S = new PriorityQueue<NodeRecord>(Comparer<NodeRecord>.Create((a, b) => a.estimatedTotalCost.CompareTo(b.estimatedTotalCost)));			
			
			// used to store all records
			Dictionary<int, NodeRecord> idsToRecords = new Dictionary<int, NodeRecord>();

			NodeRecord rec = new NodeRecord(start, 0, 0 + heuristic.estimateCost(start), NodeRecordCategory.OPEN, null);
			idsToRecords.Add(start.id, rec);

            // init S with only Start
            S.Enqueue(rec);

			NodeRecord current = S.Dequeue();

			// todo: use heurisitic.goalReached
			while (!heuristic.goalReached(current.node))
			{
				if (current.category != NodeRecordCategory.OPEN) Debug.LogWarning("Current's category should not be CLOSED");

				current.category = NodeRecordCategory.CLOSED;

				foreach (TConnection connection in graph.getConnections(current.node).connections) {
					TNode neighbor = connection.toNode;
					float cost = current.costSoFar + connection.cost;
					if(idsToRecords.ContainsKey(neighbor.id))
					{
						NodeRecord neighborRecord = idsToRecords[neighbor.id];
						if(neighborRecord.category == NodeRecordCategory.OPEN && cost < neighborRecord.costSoFar)
						{
							// new path is better
							neighborRecord.category = NodeRecordCategory.CLOSED;
							// priority implementation does not allow me to remove it from S, I'll have to check that dequeue is OPEN
                        }

						// no negative weights / dynamic graph for now, no need to check for reopen						
					}

					// neighbor is not tracked yet => not in OPEN nor CLOSED
					else
					{
						NodeRecord neighborRecord = new NodeRecord(neighbor, cost, cost + heuristic.estimateCost(neighbor), NodeRecordCategory.OPEN, current);
						idsToRecords.Add(neighbor.id, neighborRecord);
						S.Enqueue(neighborRecord);
					}
				}

                // Loop here because we could not dequeue the closed neighbors
                do
				{
                    // There is no more OPEN nodes and we did not find goal yet. Abort
                    if (S.Count == 0)
                    {
						found = -1;
						return path;
                    }
                    current = S.Dequeue();
				} while(current.category == NodeRecordCategory.CLOSED);
			}

			// reconstruct reverse path
			while (current.node.id != start.id)
			{
				path.Add(current.node);
				current = current.connection;
			}
			path.Add(start);

			// we built path from end to start but we want it from start to end
			path.Reverse();

			found = 1;
            return path;
		}
	};
}