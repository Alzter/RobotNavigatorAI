using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace RobotNavigator
{
    public class World
    {
        private PlayerAgent _player;
        private List<GoalTile> _goalTiles;
        private List<Wall> _walls;
        private int _path_cost;
        private Point _size; // Represents the width and height of the world from the top-left corner (exclusive).

        private Dictionary<PlayerAction, Point> movementValues;

        private World(Point world_size, Point agent_position)
        {
            CreateMovementDictionary();

            _size = world_size;

            _player = new PlayerAgent(agent_position.X, agent_position.Y);
        }

        public World(Point world_size, Point agent_position, List<Point> goal_tiles, List<Point> wall_tiles) : this(world_size, agent_position)
        {
            _goalTiles = new List<GoalTile>();
            foreach (Point p in goal_tiles) { _goalTiles.Add(new GoalTile(p.X, p.Y)); }

            _walls = new List<Wall>();
            foreach (Point p in wall_tiles) { _walls.Add(new Wall(p.X, p.Y)); }
        }

        public World(Point world_size, Point agent_position, List<GoalTile> goal_tile_objects, List<Wall> wall_tile_objects) : this(world_size, agent_position)
        {
            _goalTiles = goal_tile_objects;
            _walls = wall_tile_objects;
        }

        private void CreateMovementDictionary()
        {
            movementValues = new Dictionary<PlayerAction, Point>();
            movementValues.Add(PlayerAction.LEFT, new Point(-1, 0));
            movementValues.Add(PlayerAction.RIGHT, new Point(1, 0));
            movementValues.Add(PlayerAction.UP, new Point(0, -1));
            movementValues.Add(PlayerAction.DOWN, new Point(0, 1));
        }

        // Returns true if the player is able to move in a certain direction using an Action (LEFT, RIGHT, UP, DOWN).
        public bool IsActionPossible(PlayerAction action)
        {

            Point positionToMoveTo = new Point(_player.Position.X, _player.Position.Y);

            // Using a Dictionary, we will translate the player's movement action (i.e. "LEFT") into an equivalent position value (-1, 0).
            // Add this position value to the player's current position to get the tile the player is trying to move into with their action.
            // Assign this value to variable "positionToMoveTo"
            Point movement;
            if (movementValues.TryGetValue(action, out movement))
            {
                positionToMoveTo.Offset(movement); // Set "positionToMoveTo" to the tile the player is trying to move into

                // If the desired world position is out of the world's boundaries, return false
                if (!isPositionInsideBoundaries(positionToMoveTo)) return false;

                // Check if any walls are occupying the tile the player is trying to move into.
                foreach (Wall wall in _walls)
                {
                    if (wall.Position == positionToMoveTo)
                    {
                        return false; // If yes, the player cannot move into the desired space.
                    }
                }

                return true; // If not, the player may move into the desired space, therefore their action is possible.
            }
            else
            {
                // If we don't have a matching position value for our Player action (i.e. "SUCK") throw an exception.
                throw new Exception($"No movement value exists for action: {action}");
                return false;
            }
        }

        // Returns true if a specified grid position is within the boundaries of the world.
        private bool isPositionInsideBoundaries(Point position)
        {
            return (position.X >= 0 && position.X < _size.X && position.Y >= 0 && position.Y < _size.Y);
        }

        // Returns a list of all possible actions the player can perform in the current state.
        public List<PlayerAction> getPossibleActions()
        {
            List<PlayerAction> possibleActions = new List<PlayerAction>();
            // Get all possible actions in the PlayerActions enum.
            foreach (PlayerAction action in (PlayerAction[]) PlayerAction.GetValues(typeof(PlayerAction))) // Don't ask me why it's this hard to enumerate an enum.
            {
                if (IsActionPossible(action)) possibleActions.Add(action); 
            }
            return possibleActions;
        }

        // Applies a player action to the current world state.
        // I.e. "LEFT" moves the player left 1 tile and increases the path cost by 1.
        public void ApplyAction(PlayerAction action)
        {
            Point movement;
            if (movementValues.TryGetValue(action, out movement))
            {
                Point newPosition = new Point(PlayerPosition.X + movement.X, PlayerPosition.Y + movement.Y);
                _player.Position = newPosition;
                _path_cost++;
            }
            else
            {
                throw new Exception($"No movement value exists for action: {action}");
                return;
            }
        }

        // Returns true if player is standing on a goal tile.
        public bool IsSuccess()
        {
            foreach (GoalTile tile in _goalTiles)
            {
                if (tile.Position == _player.Position) return true;
            }

            return false;
        }

        // Returns a semi-deep copy of this object which can be modified independently.
        public World Copy()
        {
            return new World(_size, PlayerPosition, _goalTiles, _walls);
        }

        public Point PlayerPosition
        {
            get
            {
                return _player.Position;
            }
        }

        public List<Point> GoalTilePositions
        {
            get
            {
                return GetGoalTilePositions();
            }
        }

        // Returns a list of all positions of goal tiles
        private List<Point> GetGoalTilePositions()
        {
            List<Point> goalPositions = new List<Point>();
            foreach (GoalTile t in _goalTiles)
            {
                goalPositions.Add(t.Position);
            }
            return goalPositions;
        }

        public int PathCost
        {
            get
            {
                return _path_cost;
            }
        }
    }
}
