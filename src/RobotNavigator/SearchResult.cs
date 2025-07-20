using System;
using System.Collections.Generic;
using System.Text;

namespace RobotNavigator
{

    // Class used to encapsulate the data
    // gathered within a state-space search.
    public class SearchResult<T>
    {
        private int _nodesTotal; // Number of nodes present in the search tree
        private int _nodesExpanded; // Number of nodes expanded in the search tree
        private Node<T> _solutionNode; // The node containing the goal state of the search if it was completed effectively, or null if otherwise.

        public SearchResult(int totalNodesFound, int numberOfNodesExpanded)
        {
            _nodesTotal = totalNodesFound;
            _nodesExpanded = numberOfNodesExpanded;
            _solutionNode = null;
        }

        public SearchResult(int totalNodesFound, int numberOfNodesExpanded, Node<T> goalState)
        {
            _nodesTotal = totalNodesFound;
            _nodesExpanded = numberOfNodesExpanded;
            _solutionNode = goalState;
        }

        public int NodesFound
        {
            get
            {
                return _nodesTotal;
            }
        }

        public int NodesExpanded
        {
            get
            {
                return _nodesExpanded;
            }
        }
        public Node<T> SolutionNode
        {
            get
            {
                return _solutionNode;
            }
        }
    }
}
