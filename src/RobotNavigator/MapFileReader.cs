using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace RobotNavigator
{
    public class MapFileReader
    {

        public World ReadFloorMap(string filePath)
        {
            Point map_boundaries = new Point();
            Point agent_position = new Point();
            List<Point> wall_positions = new List<Point>();
            List<Point> goal_positions = new List<Point>();

            // Regexes used to get data from the map.
            Regex map_dimensions_getter = new Regex(@"\[([\d]+),([\d]+)]");
            Regex xy_position_getter = new Regex(@"\((\d+),(\d+)\)"); // Gets positions from XY co-ordinates like (1,2)
            Regex wall_dimensions_getter = new Regex(@"\((\d+),(\d+),(\d+),(\d+)\)"); // Gets dimensions from walls in the format (x,y,w,h)

            // Read in the problem file (floor map) line by line.
            int line_counter = 0;

            try
            {
                foreach (string line in System.IO.File.ReadLines(filePath))
                {
                    line_counter++;

                    switch (line_counter)
                    {
                        case 1: // Set map boundaries.
                            Match map_bounds = map_dimensions_getter.Match(line);
                            if (map_bounds.Success)
                            {
                                // Get the X boundary for the map as a string (E.g. for "[2,3]" the X would be "2")
                                map_boundaries.X = int.Parse(map_bounds.Groups[2].Value);
                                map_boundaries.Y = int.Parse(map_bounds.Groups[1].Value);

                                //Console.WriteLine("Map boundaries: {0}, {1}", map_boundaries.X, map_boundaries.Y);

                            }
                            break;

                        case 2: // Get agent coords.
                            Match agent_pos = xy_position_getter.Match(line);
                            if (agent_pos.Success)
                            {
                                agent_position.X = int.Parse(agent_pos.Groups[1].Value);
                                agent_position.Y = int.Parse(agent_pos.Groups[2].Value);

                                //Console.WriteLine("Agent position: {0}, {1}", agent_position.X, agent_position.Y);
                            }
                            break;

                        case 3: // Get goal tile positions
                            MatchCollection goal_tile_positions = xy_position_getter.Matches(line);
                            foreach (Match goal in goal_tile_positions)
                            {
                                Point goal_position = new Point(int.Parse(goal.Groups[1].Value), int.Parse(goal.Groups[2].Value));

                                goal_positions.Add(goal_position);

                                //Console.WriteLine("Goal tile: {0}, {1}", goal_position.X, goal_position.Y);
                            }
                            break;

                        default: // Walls
                            Match wall = wall_dimensions_getter.Match(line);
                            if (wall.Success)
                            {
                                int wall_origin_x = int.Parse(wall.Groups[1].Value); // The origin of walls is their top-left corner tile.
                                int wall_origin_y = int.Parse(wall.Groups[2].Value);

                                int wall_width = int.Parse(wall.Groups[3].Value);  // A wall with 1 width and 1 height is one tile.
                                int wall_height = int.Parse(wall.Groups[4].Value);

                                for (int x = 0; x < wall_width; x++)
                                {
                                    for (int y = 0; y < wall_height; y++)
                                    {
                                        int tile_x = x + wall_origin_x;
                                        int tile_y = y + wall_origin_y;

                                        Point wall_position = new Point(tile_x, tile_y);

                                        wall_positions.Add(wall_position);

                                        //Console.WriteLine("Wall tile: {0}, {1}", wall_position.X, wall_position.Y);
                                    }
                                }
                            }
                            break;
                    }
                }

                return new World(map_boundaries, agent_position, goal_positions, wall_positions);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading file {0}\n{1}", filePath, e.Message);
                Environment.Exit(2);
                return null;
            }

        }

    }
}
