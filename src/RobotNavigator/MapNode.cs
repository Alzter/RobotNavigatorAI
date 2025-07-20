using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Numerics;

namespace RobotNavigator
{
    public class MapNode : Node<World>
    {
        public MapNode(MapNode parent, World state, int depth, int pathCost, string action) : base(parent, state, depth, pathCost, action)
        {

        }

        public override List<Node<World>> Expand()
        {
            // Get a list of all possible player actions
            List<PlayerAction> possibleActions = State.getPossibleActions();

            // Create an empty list of child nodes
            List<Node<World>> childNodes = new List<Node<World>>();

            foreach (PlayerAction action in possibleActions)
            {
                // For every possible player action, create a child node that applies the action
                MapNode childNode = new MapNode(this, this.State.Copy(), Depth + 1, PathCost + 1, action.ToString()); // Assigning this.State may cause an error here. How do we make it a unique object?
                childNode.State.ApplyAction(action);

                childNodes.Add(childNode);
            }

            return childNodes;
        }

        public override bool IsStateIdentical(World stateToCompare)
        {
            //Console.WriteLine(stateToCompare.PlayerPosition + ", " + State.PlayerPosition);
            return (stateToCompare.PlayerPosition.X == State.PlayerPosition.X && stateToCompare.PlayerPosition.Y == State.PlayerPosition.Y);
        }

        public override bool IsSuccess()
        {
            return State.IsSuccess();
        }

        public new MapNode Parent
        {
            get
            {
                return (MapNode)_parent;
            }
            set
            {
                _parent = value; 
            }
        }

        public override float HeuristicValue
        {
            get
            {
                return getHeuristicValue();
            }
        }

        private float getHeuristicValue(bool useManhattanDistance = true)
        {
            List<Point> goalTilePositions = State.GoalTilePositions;
            Point playerPosition = State.PlayerPosition;

            float minimumDistanceToGoalTile = 999999999999999999;

            foreach (Point goalPos in goalTilePositions)
            {
                float distance;

                if (useManhattanDistance)
                {
                    // Get the Manhattan Distance between the goal tile and the player.
                    distance = Math.Abs(goalPos.X - playerPosition.X) + Math.Abs(goalPos.Y - playerPosition.Y);
                } else
                {
                    // Get the straight-line distance (Euclidean Distance) between the goal tile and the player.
                    Vector2 distanceVector = new Vector2();
                    distanceVector.X = goalPos.X - playerPosition.X;
                    distanceVector.Y = goalPos.Y - playerPosition.Y;
                    distance = distanceVector.Length();
                }

                minimumDistanceToGoalTile = Math.Min(distance, minimumDistanceToGoalTile);
            }

            return minimumDistanceToGoalTile;
        }
    }
}
