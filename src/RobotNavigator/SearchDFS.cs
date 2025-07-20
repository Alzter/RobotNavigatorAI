using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RobotNavigator
{
    class SearchDFS<T> : SearchAlgorithm<T>
    {
        public SearchDFS() : base(true)
        {

        }

        protected override List<Node<T>> sortNodesInFrontier(List<Node<T>> frontier)
        {
            List<Node<T>> sortedFrontier = frontier;

            int maxDepth = 0;

            // Set maxDepth to the Depth of the deepest node(s) in the frontier.
            foreach (Node<T> node in frontier)
            {
                maxDepth = Math.Max(maxDepth, node.Depth);
            }

            // Add all the nodes with the highest depth into the list desirableNodes.
            List<Node<T>> desirableNodes = new List<Node<T>>();

            foreach (Node<T> node in frontier)
            {
                if (node.Depth == maxDepth) desirableNodes.Add(node);
            }

            // The first node of this list is the best node.
            // It has the best action priority (UP before LEFT before DOWN before RIGHT) and was created the earliest.
            Node<T> bestNode = desirableNodes[0];

            // Move this node to the top of the frontier.
            sortedFrontier.Remove(bestNode);
            sortedFrontier.Insert(0, bestNode);

            return sortedFrontier;
        }
    }
}
