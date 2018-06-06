using System;
using System.Collections.Generic;
using UnityEngine;

namespace SokobanSolver.Sokoban
{
    public static class SokobanUtil
    {
        public static String GetFullPath(SokobanPosition[] path)
        {
            List<SokobanPosition> result = new List<SokobanPosition>();
            for(int i = 0; i < path.Length - 1; i++)
            {
                var start = path[i];
                var stop = path[i + 1];
                result.Add(start);

                bool[,] visited = new bool[start.Width, start.Height];
                visited[start.Player.X, start.Player.Y] = true;
                Array.Clear(visited, 0, visited.Length);
                PointXY[] stack = new PointXY[start.Width * start.Height];
                PointXY[,] track = new PointXY[start.Width, start.Height];
                track[start.Player.X, start.Player.Y].X = 0;
                track[start.Player.X, start.Player.Y].Y = 0;
                int stackTop = 0;
                stack[0] = start.Player;
                PointXY p;
                SokobanPosition found = null;
                while (stackTop >= 0)
                {
                    p = stack[stackTop];
                    stackTop--;
                    //try push left
                    if ((start.Map[p.X - 1, p.Y] == Constants.STONE || start.Map[p.X - 1, p.Y] == Constants.GOALSTONE) && (start.Map[p.X - 2, p.Y] == Constants.EMPTY || start.Map[p.X - 2, p.Y] == Constants.GOAL))
                    {
                        var next = start.ClonePush(p, Direction.Left);
                        if (next.GetUniqueId() == stop.GetUniqueId())
                        {
                            found = next;
                            track[p.X - 1, p.Y] = p;
                            break;
                        }
                    }
                    //try push right
                    if ((start.Map[p.X + 1, p.Y] == Constants.STONE || start.Map[p.X + 1, p.Y] == Constants.GOALSTONE) && (start.Map[p.X + 2, p.Y] == Constants.EMPTY || start.Map[p.X + 2, p.Y] == Constants.GOAL))
                    {
                        var next = start.ClonePush(p, Direction.Right);
                        if (next.GetUniqueId() == stop.GetUniqueId())
                        {
                            found = next;
                            track[p.X + 1, p.Y] = p;
                            break;
                        }
                    }
                    //try push up
                    if ((start.Map[p.X, p.Y - 1] == Constants.STONE || start.Map[p.X, p.Y - 1] == Constants.GOALSTONE) && (start.Map[p.X, p.Y - 2] == Constants.EMPTY || start.Map[p.X, p.Y - 2] == Constants.GOAL))
                    {
                        var next = start.ClonePush(p, Direction.Up);
                        if (next.GetUniqueId() == stop.GetUniqueId())
                        {
                            found = next;
                            track[p.X, p.Y - 1] = p;
                            break;
                        }
                    }
                    //try push down
                    if ((start.Map[p.X, p.Y + 1] == Constants.STONE || start.Map[p.X, p.Y + 1] == Constants.GOALSTONE) && (start.Map[p.X, p.Y + 2] == Constants.EMPTY || start.Map[p.X, p.Y + 2] == Constants.GOAL))
                    {
                        var next = start.ClonePush(p, Direction.Down);
                        if (next.GetUniqueId() == stop.GetUniqueId())
                        {
                            found = next;
                            track[p.X, p.Y + 1] = p;
                            break;
                        }
                    }
                    //try walk left
                    if (!visited[p.X - 1, p.Y] && (start.Map[p.X - 1, p.Y] == Constants.EMPTY || start.Map[p.X - 1, p.Y] == Constants.GOAL))
                    {
                        stackTop++;
                        stack[stackTop].X = p.X - 1;
                        stack[stackTop].Y = p.Y;
                        visited[p.X - 1, p.Y] = true;
                        track[p.X - 1, p.Y] = p;
                    }
                    //try walk right
                    if (!visited[p.X + 1, p.Y] && (start.Map[p.X + 1, p.Y] == Constants.EMPTY || start.Map[p.X + 1, p.Y] == Constants.GOAL))
                    {
                        stackTop++;
                        stack[stackTop].X = p.X + 1;
                        stack[stackTop].Y = p.Y;
                        visited[p.X + 1, p.Y] = true;
                        track[p.X + 1, p.Y] = p;
                    }
                    //try walk up
                    if (!visited[p.X, p.Y - 1] && (start.Map[p.X, p.Y - 1] == Constants.EMPTY || start.Map[p.X, p.Y - 1] == Constants.GOAL))
                    {
                        stackTop++;
                        stack[stackTop].X = p.X;
                        stack[stackTop].Y = p.Y - 1;
                        visited[p.X, p.Y - 1] = true;
                        track[p.X, p.Y - 1] = p;
                    }
                    //try walk down
                    if (!visited[p.X, p.Y + 1] && (start.Map[p.X, p.Y + 1] == Constants.EMPTY || start.Map[p.X, p.Y + 1] == Constants.GOAL))
                    {
                        stackTop++;
                        stack[stackTop].X = p.X;
                        stack[stackTop].Y = p.Y + 1;
                        visited[p.X, p.Y + 1] = true;
                        track[p.X, p.Y + 1] = p;
                    }
                }               
                if (found != null)
                {
                    var t = stop.Player;
                    List<SokobanPosition> intermediate = new List<SokobanPosition>();
                    while (!(t.X == start.Player.X && t.Y == start.Player.Y))
                    {
                        SokobanPosition pos = start.Clone();
                        pos.Player.X = t.X;
                        pos.Player.Y = t.Y;
                        if (!(t.X == stop.Player.X && t.Y == stop.Player.Y))
                        {
                            intermediate.Add(pos);
                        }

                        t = track[t.X, t.Y];
                    }
                    intermediate.Reverse();
                    result.AddRange(intermediate);
                }
            }
            result.Add(path[path.Length - 1]);
            //Console.WriteLine(moves);
            string moves = "";
            int currentX = result[0].Player.X;
            int currentY = result[0].Player.Y;
            for (int i = 1; i < result.Count; i++)
            {
                if (result[i].Player.X - currentX == 1)
                    moves += "↑";
                if (result[i].Player.X - currentX == -1)
                    moves += "↓";
                if (result[i].Player.Y - currentY == 1)
                    moves += "→";
                if (result[i].Player.Y - currentY == -1)
                    moves += "←";
                currentX = result[i].Player.X;
                currentY = result[i].Player.Y;
            }
            return moves;
        }
    }
}
