using System;
using System.Collections.Generic;
using System.Text;

namespace RobotNavigator
{
    class SearchAStar<T> : SearchAlgorithm<T>
    {
        public SearchAStar() : base(true)
        {

        }

        protected override List<Node<T>> sortNodesInFrontier(List<Node<T>> frontier)
        {
            List<Node<T>> sortedFrontier = frontier;

            float minimumHeuristicValue = 999999999999;

            // Get the lowest heuristic value of all nodes in the search tree
            foreach (Node<T> node in frontier)
            {
                float nodeHeuristicValue = node.HeuristicValue + node.PathCost;
                minimumHeuristicValue = Math.Min(minimumHeuristicValue, nodeHeuristicValue);
            }

            // Add all of the nodes with the lowest heuristic value into a list
            List<Node<T>> desiredNodes = new List<Node<T>>();
            foreach (Node<T> node in frontier)
            {
                float nodeHeuristicValue = node.HeuristicValue + node.PathCost;
                if (nodeHeuristicValue == minimumHeuristicValue) desiredNodes.Add(node);
            }

            // The first node of this list is the best node.
            Node<T> bestNode = desiredNodes[0];

            // Move this optimal node to the top of the frontier.
            sortedFrontier.Remove(bestNode);
            sortedFrontier.Insert(0, bestNode);

            return sortedFrontier;
        }
    }
}
