using System;
using System.Collections.Generic;
using System.Text;

namespace RobotNavigator
{
    class SearchBFS<T> : SearchAlgorithm<T>
    {
        // BFS search does not sort the frontier.
        protected override List<Node<T>> sortNodesInFrontier(List<Node<T>> frontier)
        {
            return frontier;
        }
    }
}
