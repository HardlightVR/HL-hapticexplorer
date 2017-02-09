/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://www.hardlightvr.com/wp-content/uploads/2017/01/NullSpace-SDK-License-Rev-3-Jan-2016-2.pdf
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NullSpace.SDK
{
	public class GraphEngine
	{
		public class SuitNode
		{
			private AreaFlag _name;
			public AreaFlag Location
			{
				get { return _name; }
			}
			public SuitNode(AreaFlag loc)
			{
				_name = loc;
			}
			public override string ToString()
			{
				return _name.ToString();
			}

		}
		private Dictionary<AreaFlag, SuitNode> _nodes;
		private Dictionary<SuitNode, List<SuitNode>> _graph;
		private Dictionary<SuitNode, Dictionary<SuitNode, int>> _weights;
		public GraphEngine()
		{
			_weights = new Dictionary<SuitNode, Dictionary<SuitNode, int>>();
			_graph = new Dictionary<SuitNode, List<SuitNode>>();
			_nodes = new Dictionary<AreaFlag, SuitNode>();
			SuitNode leftShoulder = new SuitNode(AreaFlag.Shoulder_Left);
			SuitNode leftChest = new SuitNode(AreaFlag.Chest_Left);
			SuitNode leftBack = new SuitNode(AreaFlag.Back_Left);
			SuitNode leftUpperArm = new SuitNode(AreaFlag.Upper_Arm_Left);
			SuitNode leftForearm = new SuitNode(AreaFlag.Forearm_Left);
			SuitNode leftUpperAbs = new SuitNode(AreaFlag.Upper_Ab_Left);
			SuitNode leftMidAbs = new SuitNode(AreaFlag.Mid_Ab_Left);
			SuitNode leftLowerAbs = new SuitNode(AreaFlag.Lower_Ab_Left);

			SuitNode rightShoulder = new SuitNode(AreaFlag.Shoulder_Right);
			SuitNode rightChest = new SuitNode(AreaFlag.Chest_Right);
			SuitNode rightBack = new SuitNode(AreaFlag.Back_Right);
			SuitNode rightUpperArm = new SuitNode(AreaFlag.Upper_Arm_Right);
			SuitNode rightForearm = new SuitNode(AreaFlag.Forearm_Right);
			SuitNode rightUpperAbs = new SuitNode(AreaFlag.Upper_Ab_Right);
			SuitNode rightMidAbs = new SuitNode(AreaFlag.Mid_Ab_Right);
			SuitNode rightLowerAbs = new SuitNode(AreaFlag.Lower_Ab_Right);




			List<SuitNode> nodes = new List<SuitNode> { rightShoulder, rightChest, rightBack, rightUpperArm, rightForearm, rightUpperAbs, rightMidAbs, rightLowerAbs, leftShoulder, leftChest, leftBack, leftUpperArm, leftForearm, leftUpperAbs, leftMidAbs, leftLowerAbs };
			foreach (var node in nodes)
			{
				_graph[node] = new List<SuitNode>();
				_weights[node] = new Dictionary<SuitNode, int>();
				_nodes[node.Location] = node;
			}

			InsertEdge(leftChest, rightChest, 10);
			InsertEdge(leftChest, leftUpperAbs, 10);
			InsertEdge(leftChest, rightUpperAbs, 17);
			InsertEdge(leftChest, leftShoulder, 10);

			//Right chest
			InsertEdge(rightChest, leftChest, 10);
			InsertEdge(rightChest, rightUpperAbs, 10);
			InsertEdge(rightChest, leftUpperAbs, 17);
			InsertEdge(rightChest, rightShoulder, 10);

			//Left Upper Abs
			InsertEdge(leftUpperAbs, leftChest, 10);
			InsertEdge(leftUpperAbs, leftMidAbs, 10);
			InsertEdge(leftUpperAbs, rightUpperAbs, 10);
			InsertEdge(leftUpperAbs, rightMidAbs, 14);
			InsertEdge(leftUpperAbs, rightChest, 17);

			//Right Upper Abs
			InsertEdge(rightUpperAbs, rightChest, 10);
			InsertEdge(rightUpperAbs, rightMidAbs, 10);
			InsertEdge(rightUpperAbs, leftUpperAbs, 10);
			InsertEdge(rightUpperAbs, leftMidAbs, 14);
			InsertEdge(rightUpperAbs, leftChest, 17);

			//Left Mid Abs 
			InsertEdge(leftMidAbs, leftUpperAbs, 10);
			InsertEdge(leftMidAbs, leftLowerAbs, 10);
			InsertEdge(leftMidAbs, rightUpperAbs, 14);
			InsertEdge(leftMidAbs, rightMidAbs, 10);
			InsertEdge(leftMidAbs, rightLowerAbs, 14);

			//Right Mid Abs 
			InsertEdge(rightMidAbs, rightUpperAbs, 10);
			InsertEdge(rightMidAbs, rightLowerAbs, 10);
			InsertEdge(rightMidAbs, leftUpperAbs, 14);
			InsertEdge(rightMidAbs, leftMidAbs, 10);
			InsertEdge(rightMidAbs, leftLowerAbs, 14);

			//Left Lower Abs
			InsertEdge(leftLowerAbs, rightLowerAbs, 10);
			InsertEdge(leftLowerAbs, rightMidAbs, 14);
			InsertEdge(leftLowerAbs, leftMidAbs, 10);

			//Right Lower Abs   
			InsertEdge(rightLowerAbs, leftLowerAbs, 10);
			InsertEdge(rightLowerAbs, leftMidAbs, 14);
			InsertEdge(rightLowerAbs, rightMidAbs, 10);

			//Left shoulder
			InsertEdge(leftShoulder, leftChest, 10);
			InsertEdge(leftShoulder, leftBack, 20);
			InsertEdge(leftShoulder, leftUpperArm, 10);

			//Right shoulder
			InsertEdge(rightShoulder, rightChest, 10);
			InsertEdge(rightShoulder, rightBack, 20);
			InsertEdge(rightShoulder, rightUpperArm, 10);

			//Left upper arm
			InsertEdge(leftUpperArm, leftShoulder, 10);
			InsertEdge(leftUpperArm, leftForearm, 10);

			//Right upper arm
			InsertEdge(rightUpperArm, rightShoulder, 10);
			InsertEdge(rightUpperArm, rightForearm, 10);

			//Left forearm
			InsertEdge(leftForearm, leftUpperArm, 10);

			//Right forearm
			InsertEdge(rightForearm, rightUpperArm, 10);

			//Left back
			InsertEdge(leftBack, leftShoulder, 20);
			InsertEdge(leftBack, rightBack, 20);

			//Right back
			InsertEdge(rightBack, rightShoulder, 20);
			InsertEdge(rightBack, leftBack, 20);
		}
		public List<SuitNode> Dijkstras(AreaFlag beginNode, AreaFlag endNode)
		{
			Dictionary<SuitNode, float> dist = new Dictionary<SuitNode, float>();
			Dictionary<SuitNode, SuitNode> prev = new Dictionary<SuitNode, SuitNode>();

			var stack = new List<SuitNode>();
			foreach (var n in _nodes)
			{
				dist[n.Value] = float.PositiveInfinity;
				prev[n.Value] = null;
				stack.Add(n.Value);
			}
			var source = _nodes[beginNode];
			var sink = _nodes[endNode];
			dist[source] = 0;

			while (stack.Count > 0)
			{
				var u = minDist(stack, dist);
				if (u.Location == endNode)
				{
					break;
				}
				stack.Remove(u);
				foreach (var neighbor in Neighbors(u))
				{
					var alt = dist[u] + _weights[u][neighbor];
					if (alt < dist[neighbor])
					{
						dist[neighbor] = alt;
						prev[neighbor] = u;
					}
				}
			}

			List<SuitNode> s = new List<SuitNode>();
			var u2 = sink;
			while (prev[u2] != null)
			{
				s.Add(u2);
				u2 = prev[u2];
			}
			s.Add(source);
			s.Reverse();
			return s;
		}

		private SuitNode minDist(IList<SuitNode> stack, Dictionary<SuitNode, float> dist)
		{
			SuitNode best = null;
			float bestDist = float.PositiveInfinity;
			foreach (var node in stack)
			{
				if (dist[node] < bestDist)
				{
					best = node;
					bestDist = dist[node];
				}
			}

			return best;
		}

		public List<SuitNode> DFS(AreaFlag beginNode, AreaFlag endNode)
		{
			var stages = new List<SuitNode>();
			var stack = new Stack<SuitNode>();
			Dictionary<SuitNode, SuitNode> cameFrom = new Dictionary<SuitNode, SuitNode>();
			var source = _nodes[beginNode];
			//var sink = _nodes[endNode];
			Dictionary<SuitNode, bool> visited = new Dictionary<SuitNode, bool>();
			foreach (var n in _nodes)
			{
				visited[n.Value] = false;
			}
			stack.Push(source);
			while (stack.Count != 0)
			{
				var v = stack.Pop();
				if (!visited[v])
				{
					visited[v] = true;
					var neighbors = Neighbors(v);
					foreach (var neigh in neighbors)
					{
						stack.Push(neigh);
						cameFrom[neigh] = v;
					}
					stages.Add(v);


				}
			}

			return stages;

		}
		public List<List<SuitNode>> BFS(AreaFlag beginNode, int maxDepth)
		{
			maxDepth = Math.Max(0, maxDepth);
			List<List<SuitNode>> stages = new List<List<SuitNode>>();
			SuitNode source = _nodes[beginNode];
			Dictionary<SuitNode, bool> visited = new Dictionary<SuitNode, bool>();
			foreach (var n in _nodes)
			{
				visited[n.Value] = false;
			}
			Queue<SuitNode> queue = new Queue<SuitNode>();
			int currentDepth = 0;
			int elementsToDepthIncrease = 1;
			int nextElementsToDepthIncrease = 0;
			visited[source] = true;
			queue.Enqueue(source);
			stages.Add(new List<SuitNode>() { source });
			List<SuitNode> potentialNextStage = new List<SuitNode>();

			while (queue.Count != 0)
			{
				source = queue.Dequeue();
				var neighbors = Neighbors(source);



				foreach (var neighbor in neighbors)
				{
					if (!visited[neighbor])
					{
						visited[neighbor] = true;
						queue.Enqueue(neighbor);
						potentialNextStage.Add(neighbor);
						nextElementsToDepthIncrease++;
					}
				}

				if (--elementsToDepthIncrease == 0)
				{
					if (potentialNextStage.Count > 0) { stages.Add(new List<SuitNode>(potentialNextStage)); }
					if (++currentDepth == maxDepth) return stages;
					elementsToDepthIncrease = nextElementsToDepthIncrease;
					nextElementsToDepthIncrease = 0;
					potentialNextStage.Clear();
				}

			}

			return stages;
		}


		private int Cost(SuitNode a, SuitNode b)
		{
			return _weights[a][b];
		}


		private List<SuitNode> Neighbors(SuitNode node)
		{
			return _graph[node];
		}
		private void InsertEdge(SuitNode nodeA, SuitNode nodeB, int weight)
		{
			if (!_graph[nodeA].Contains(nodeB))
			{
				_graph[nodeA].Add(nodeB);
			}
			if (!_graph[nodeB].Contains(nodeA))
			{
				_graph[nodeB].Add(nodeA);
			}

			_weights[nodeA][nodeB] = weight;
			_weights[nodeB][nodeA] = weight;

		}
	}
}
