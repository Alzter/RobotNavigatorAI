using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace RobotNavigator
{
    public class RandomMazeGenerator
    {
        public World generateRandomMazeLayout(int width, int height, int numberOfGoalTiles)
        {
            if (width <= 0 || height <= 0) throw new Exception("World width and height must be greater than 0.");
            if (numberOfGoalTiles <= 0) throw new Exception("The world must have at least 1 goal tile.");

            int maxGoalTiles = (width * height) - 1;
            if (numberOfGoalTiles > maxGoalTiles) throw new Exception($"Too many goal tiles. A world of dimensions {width}x{height} can have a max of {maxGoalTiles} goal tiles.");

            // Generate a random agent position within the world size.
            Random rng = new Random();

            Point agentPosition = generateRandomPositionInWorld(width, height);

            //Console.WriteLine($"Agent position: [{agentPosition.X}, {agentPosition.Y}]");

            Point worldSize = new Point(width, height);

            List<Point> goalTilePositions = new List<Point>();


            // ==========================================================================================================================================
            // PLACING GOAL TILES

            List<Point> emptyWorldTiles = new List<Point>();

            // Fill the emptyWorldTiles list with all empty spaces within the world.
            // We will need this list to know which spaces are available to place goal tiles in.

            // Enumerate through all possible positions within the 2D world.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Console.WriteLine($"[{x},{y}]");
                    // For each cell in the world, if it is not being taken up by the player, add it to the empty world spaces list
                    if (agentPosition.X != x || agentPosition.Y != y)
                    {
                        emptyWorldTiles.Add(new Point(x, y));
                        //Console.WriteLine("ADDED");
                    }
                }
            }

            //Console.WriteLine("==============");

            //foreach (Point pos in emptyWorldTiles) { Console.WriteLine($"[{pos.X},{pos.Y}]"); }

            int numberOfGoalTilesToPlace = numberOfGoalTiles;
            for (int i = 0; i < numberOfGoalTiles; i++)
            {
                if (emptyWorldTiles.Count < 1) throw new Exception("Error placing goal tiles in random world: ran out of empty spaces to place tiles in");

                // Set RandomCellID to a random item in the emptyWorldTiles list
                int randomCellID = rng.Next(0, emptyWorldTiles.Count - 1);

                // Get the corresponding position from the empty world tiles array.
                Point randomCellPosition = emptyWorldTiles[randomCellID];

                // REMOVE THIS POSITION FROM THE EMPTY WORLD TILES ARRAY because we are about to place a goal tile in it so it will no longer be empty.
                emptyWorldTiles.Remove(randomCellPosition);

                goalTilePositions.Add(randomCellPosition);
                //Console.WriteLine($"Added goal tile at position [{randomCellPosition.X}, {randomCellPosition.Y}]");
            }

            // ======================================================================================================================================
            // GENERATE A PATH FROM THE PLAYER TO THE GOAL NODE. This path will be guaranteed to be clear in the final maze.

            // Generate an empty world with the player and goal tiles.

            World emptyWorld = new World(worldSize, agentPosition, goalTilePositions, new List<Point>());

            // Use a random uninformed search to generate a path between the player and one of the goal tiles within the world.
            // We need to create a path between the player and at least one goal node and prevent any walls from being placed there to ensure that the random maze is solvable.

            SearchCUS1<World> randomSearch = new SearchCUS1<World>();

            MapNode initialNode = new MapNode(null, emptyWorld, 0, 0, null);

            SearchResult<World> pathToGoal = randomSearch.Search(initialNode);

            // =======================================================================================================
            // We now have a clear path from the player to one goal node in the maze.
            // We will now remove all cells which are in this path from the emptyWorldTiles list, so that Walls will not be placed on the path.
            // This ensures that the random maze will be solvable.

            // Push all of the nodes from the random search onto a stack
            Stack<Node<World>> nodeStack = new Stack<Node<World>>();
            Node<World> currentNode = pathToGoal.SolutionNode;

            while (currentNode != null)
            {
                nodeStack.Push(currentNode);
                currentNode = currentNode.Parent;
            }

            List<Point> tilesToRemove = new List<Point>();

            // For each node in the random path from the player to the goal:
            while (nodeStack.Count > 0)
            {
                currentNode = nodeStack.Pop();

                Point pathPosition = currentNode.State.PlayerPosition;

                foreach (Point position in emptyWorldTiles)
                {
                    // If the node's position is in the emptyWorldTiles list, remove it from the list
                    if (position.X == pathPosition.X && position.Y == pathPosition.Y)
                    {
                        tilesToRemove.Add(position);
                    }
                }
            }

            foreach (Point tile in tilesToRemove) { emptyWorldTiles.Remove(tile); }

            // ================================================================================================================
            // PLACE WALLS IN THE WORLD
            // Now that we have a guaranteed random path from the player to a goal node,
            // we can fill in SOME of the remaining tiles of the maze with walls.

            List<Point> wallTilePositions = new List<Point>();

            foreach (Point emptyTile in emptyWorldTiles)
            {
                if (rng.Next(2) == 1) wallTilePositions.Add(emptyTile);
            }

            foreach (Point tile in wallTilePositions) { emptyWorldTiles.Remove(tile); }

            // ==================================================================================================================
            // GENERATE THE FINAL RANDOM MAZE.

            World randomWorld = new World(worldSize, agentPosition, goalTilePositions, wallTilePositions);

            // PRINT THE RANDOM WORLD.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool tileOccupied = false;
                    if (agentPosition.X == x && agentPosition.Y == y)
                    {
                        Console.Write("[A]");
                        tileOccupied = true;
                    }
                    if (tileOccupied) continue;
                    foreach (Point tile in goalTilePositions)
                    {
                        if (tile.X == x && tile.Y == y)
                        {
                            Console.Write("[G]");
                            tileOccupied = true;
                            break;
                        }
                    }
                    if (tileOccupied) continue;
                    foreach (Point tile in wallTilePositions)
                    {
                        if (tile.X == x && tile.Y == y)
                        {
                            Console.Write("[W]");
                            tileOccupied = true;
                            break;
                        }
                    }
                    if (tileOccupied) continue;
                    Console.Write("[ ]");
                }
                Console.WriteLine("");
            }

             return randomWorld;
        }

        // Generates a random position within a 2D world of dimensions [WIDTH,HEIGHT]
        // Note that we subtract width and height by 1 because they specify the outer edges of the world.
        private Point generateRandomPositionInWorld(int worldWidth, int worldHeight)
        {
            Random rng = new Random();
            Point randomPos = new Point();
            randomPos.X = rng.Next(0, worldWidth - 1);
            randomPos.Y = rng.Next(0, worldHeight - 1);
            return randomPos;
        }
    }
}
