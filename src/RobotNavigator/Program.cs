using System;
using System.Collections.Generic;
using System.Drawing;

namespace RobotNavigator
{
    class Program
    {
        static void Main(string[] args)
        {


            Program p = new Program();
            if (args[0].ToLower() == "random")
            {
                if (args.Length != 5)
                {
                    Console.WriteLine("Invalid syntax. Syntax is:\n\nrandom <maze width> <maze height> <number of goal squares> <search method>");
                    return;
                }
                else
                {
                    int worldWidth = int.Parse(args[1]);
                    int worldHeight = int.Parse(args[2]);
                    int numberOfGoalSquares = int.Parse(args[3]);
                    p.applySearchAlgorithm<World>(worldWidth, worldHeight, numberOfGoalSquares, args[4]);
                }
            }
            else
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Invalid syntax. Syntax is:\n\n<problem file to search> <search method>");
                    return;
                }
                else
                {
                    p.applySearchAlgorithm<World>(args[0], args[1]);
                }
            }
        }

        public void applySearchAlgorithm<T>(int worldHeight, int worldWidth, int numberOfGoalSquares, string searchAlgorithm)
        {
            try
            {
                RandomMazeGenerator mazeGen = new RandomMazeGenerator();

                World initialState = mazeGen.generateRandomMazeLayout(worldHeight, worldWidth, numberOfGoalSquares);

                applySearchAlgorithm<T>("RANDOM", initialState, searchAlgorithm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generating custom maze. The following exception occurred:");
                Console.WriteLine(e.Message);
            }
        }


        public void applySearchAlgorithm<T>(string problemFilePath, string searchAlgorithm)
        {
            MapFileReader fileReader = new MapFileReader();

            World initialState = fileReader.ReadFloorMap(problemFilePath);


            applySearchAlgorithm<T>(problemFilePath, initialState, searchAlgorithm);
        }


        public void applySearchAlgorithm<T>(string problemFilePath, World initialState, string searchAlgorithm)
        {
            MapNode initialNode = new MapNode(null, initialState, 0, 0, null);

            // Create a SearchAlgorithm object which corresponds with the search algorithm being used.
            SearchAlgorithm<World> s;

            searchAlgorithm = searchAlgorithm.Trim().ToUpper();
            switch (searchAlgorithm)
            {
                case "BFS":
                    s = new SearchBFS<World>();
                    break;
                case "DFS":
                    s = new SearchDFS<World>();
                    break;
                case "GBFS":
                    s = new SearchGBFS<World>();
                    break;
                case "A*":
                    s = new SearchAStar<World>();
                    break;
                case "AS":
                    s = new SearchAStar<World>();
                    break;
                case "CUS1":
                    s = new SearchCUS1<World>();
                    break;
                default:
                    Console.WriteLine($"Unrecognized Search Algorithm: {searchAlgorithm}");
                    Environment.Exit(0);
                    return;
            }

            SearchResult<World> result = s.Search(initialNode);

            PrintSearchResult(problemFilePath, searchAlgorithm, result.NodesFound, result.SolutionNode);
        }

        public void PrintSearchResult(string filename, string searchMethod, int totalNodesFound, Node<World> solutionNode)
        {
            Console.WriteLine($"\n{filename} {searchMethod} {totalNodesFound}");

            if (solutionNode == null)
            {
                Console.WriteLine("No solution found.");
                return;
            }

            // Print the path of actions which result in the solution.

            Stack<Node<World>> nodeStack = new Stack<Node<World>>();
            Node<World> currentNode = solutionNode;

            while (currentNode != null)
            {
                nodeStack.Push(currentNode);
                currentNode = currentNode.Parent;
            }

            while (nodeStack.Count > 0)
            {
                currentNode = nodeStack.Pop();
                if (currentNode.Action == null) continue; // Don't print the root node
                Console.Write(currentNode.Action + "; ");
            }
        }
    }
}
