using System;
using System.Collections.Generic;
using System.Text;

namespace RobotNavigator
{
    public abstract class SearchAlgorithm<T>
    {
        protected bool _ignoreExistingStates;

        public SearchAlgorithm()
        {
            _ignoreExistingStates = false;
        }

        public SearchAlgorithm(bool ignoreExistingStates)
        {
            _ignoreExistingStates = ignoreExistingStates;
        }

        // Creates a search tree using Node rootNode as a base.
        // Searches through the tree of RootNode until it finds a solution.
        // Returns the solution of the search as a SearchResult object.
        public SearchResult<T> Search(Node<T> rootNode)
        {
            Dictionary<T, int> knownStates;

            knownStates = new Dictionary<T, int>();
            knownStates.Add(rootNode.State, rootNode.Depth);

            int nodesExpanded = 0;
            int nodesDiscovered = 0;
            int currentDepth = 0;

            // Create an empty frontier
            List<Node<T>> frontier = new List<Node<T>>();

            // Expand the root node of the tree and put all child nodes in the frontier
            nodesExpanded++;
            foreach (Node<T> childNode in rootNode.Expand())
            {
                nodesDiscovered++;
                frontier.Add(childNode);
            }

            Node<T> solutionNode = null;

            while (frontier.Count > 0)
            {
                if (frontier.Count > 1)
                {
                    frontier = sortNodesInFrontier(frontier);
                }
                if (frontier.Count < 1) break;

                /*
                Console.WriteLine("Printing Frontier");
                foreach (Node<T> node in frontier)
                {
                    Console.WriteLine(node.Action + ", " + node.Depth);
                }
                */

                // Take the first node of the frontier.
                Node<T> currentNode = frontier[0];
                frontier.RemoveAt(0);

                // If that node is in a winning state, stop searching.
                if (currentNode.IsSuccess())
                {
                    solutionNode = currentNode;
                    break;
                }

                currentDepth = currentNode.Depth;
                knownStates.Add(currentNode.State, currentNode.Depth);

                // Remove all nodes in the "known states" dictionary which have a higher
                // depth than the depth of the current node in the frontier.
                foreach (T state in knownStates.Keys)
                {
                    // Set Depth to the Depth of the node in the KnownStates array
                    int depth;

                    if (knownStates.TryGetValue(state, out depth))
                    {
                        if (depth > currentDepth)
                        {
                            knownStates.Remove(state);
                        }
                    }

                }

                // Otherwise, expand this node and put all child nodes at the end of the frontier.
                nodesExpanded++;
                //Console.WriteLine($"Expanding node with depth {currentNode.Depth} and action {currentNode.Action}");
                
                
                foreach (Node<T> childNode in currentNode.Expand())
                {
                    //Console.WriteLine(childNode.Action + ", " + childNode.Depth);
                    // Check for each child node that their state is unique before adding it to the search tree.
                    bool isStateKnown = false;
                    foreach (T state in knownStates.Keys)
                    {
                        if (childNode.IsStateIdentical(state))
                        {
                            //Console.WriteLine("State is Identical");
                            isStateKnown = true;
                            break;
                        }
                    }

                    if (isStateKnown && _ignoreExistingStates) continue;

                    nodesDiscovered++;
                    frontier.Add(childNode);
                }
            }

            // SEARCH COMPLETE
            // Create a SearchResult object to encapsulate the search findings

            SearchResult<T> result = new SearchResult<T>(nodesDiscovered, nodesExpanded, solutionNode);

            return result;
        }

        protected abstract List<Node<T>> sortNodesInFrontier(List<Node<T>> frontier);
    }
}
