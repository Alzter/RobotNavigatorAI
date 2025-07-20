using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RobotNavigator
{
    class SearchCUS1<T> : SearchAlgorithm<T>
    {
        public SearchCUS1() : base(true) // Random search - randomly sorts nodes in the frontier but ignores previously explored nodes
        {

        }

        // Randomly place a node at the top of the rontier.
        protected override List<Node<T>> sortNodesInFrontier(List<Node<T>> frontier)
        {
            List<Node<T>> sortedFrontier = frontier;

            Random rng = new Random();

            // Set ITEM to a random int range from 0 to the length of the frontier list
            int Item = rng.Next(0, sortedFrontier.Count - 1);

            // Select node ITEM from the frontier
            Node<T> item = sortedFrontier[Item];

            // Place that node at the front of the frontier
            sortedFrontier.Remove(item);
            sortedFrontier.Add(item);

            return sortedFrontier;
        }
    }
}
