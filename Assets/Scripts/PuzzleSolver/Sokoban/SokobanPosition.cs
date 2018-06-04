using PanJanek.SokobanSolver.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PanJanek.SokobanSolver.Sokoban
{
    public class SokobanPosition : IGamePosition
    {
        private static PointXY[] Stack = new PointXY[Constants.InteralStackMaxSize];

        private static bool[,] VisitedMap = new bool[Constants.InternalMapMaxWidth, Constants.InternalMapMaxHeight];

        private static int[,] distanceMatrix = null;

        private static int[,] distanceMatrixCopy = null;

        private static HungarianAlgorithm hungarian = null;

        public byte[,] StonesMap;

        public PointXY[] Stones;

        public PointXY[] Goals;

        public int Width;

        public int Height;

        public byte[,] Map;

        public bool[,] DeadlockMap;

        public PointXY Player;

        public IGamePosition Parent { get; set; }

        private byte[] Binary;

        private PointXY NormalizedPlayer;

        private PointXY stoneMovedFrom;

        private PointXY stoneMovedTo;

        private string CachedUniqueId;

        private int? CachedHeuristics;

        public static SokobanPosition LoadFromFile(string path)
        {
            string[] lines = File.ReadAllLines(path).Where(l => l.Length > 0).ToArray();
            SokobanPosition position = new SokobanPosition();
            position.Width = lines.Max(l => l.Length);
            position.Height = lines.Length;
            position.Map = new byte[position.Width, position.Height];
            position.StonesMap = new byte[position.Width, position.Height];
            position.NormalizedPlayer.X = 0;
            position.NormalizedPlayer.Y = 0;
            List<PointXY> stones = new List<PointXY>();
            List<PointXY> goals = new List<PointXY>();
            for(int y = 0; y<position.Height; y++)
            {
                string line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    PointXY p;
                    p.X = x;
                    p.Y = y;
                    position.StonesMap[x,y] = 0;
                    switch (line[x])
                    {
                        case '#':
                            position.Map[x, y] = Constants.WALL;
                            break;
                        case '$':
                            position.Map[x, y] = Constants.STONE;
                            stones.Add(p);
                            position.StonesMap[x, y] = (byte)stones.Count;
                            break;
                        case '.':
                            position.Map[x, y] = Constants.GOAL;
                            goals.Add(p);
                            break;
                        case '*':
                            position.Map[x, y] = Constants.GOALSTONE;
                            stones.Add(p);
                            goals.Add(p);
                            position.StonesMap[x, y] = (byte)stones.Count;
                            break;
                        case '+':
                            position.Player.X = x;
                            position.Player.Y = y;
                            break;
                        case '-':
                            position.Map[x, y] = Constants.EMPTY;
                            break;
                    }
                }
            }

            if (stones.Count != goals.Count)
            {
                throw new Exception("Number of stones is not equal to numer of goals!"); 
            }

            position.Goals = goals.ToArray();
            position.Stones = stones.ToArray();
            distanceMatrix = new int[position.Stones.Length, position.Stones.Length];
            distanceMatrixCopy = new int[position.Stones.Length, position.Stones.Length];
            hungarian = new HungarianAlgorithm(position.Stones.Length);
            return position;
        }

        public int Heuristics()
        {
            if (!this.CachedHeuristics.HasValue)
            {
                for (int s = 0; s < this.Stones.Length; s++)
                {
                    for (int g = 0; g < this.Goals.Length; g++)
                    {
                        int dx = this.Stones[s].X - this.Goals[g].X;
                        int dy = this.Stones[s].Y - this.Goals[g].Y;
                        
                        if (dx < 0)
                        {
                            dx = dx * -1;
                        }

                        if (dy < 0)
                        {
                            dy = dy * -1;
                        }

                        distanceMatrix[s, g] = dx + dy;
                    }
                }

                Array.Copy(distanceMatrix, distanceMatrixCopy, distanceMatrix.Length);
                var res = hungarian.execute(distanceMatrixCopy);
                int distanceSum = 0;
                for (int k = 0; k < res.Length; k++)
                {
                    int goalCol = res[k];
                    if (goalCol > -1)
                        distanceSum += distanceMatrix[k,goalCol];
                }

                if (distanceSum == 0)
                {
                    CachedHeuristics = 0;
                }
                else
                {
                    //var parent = (SokobanPosition)this.Parent;
                    //bool sameStoneMoved = parent != null && parent.stoneMovedTo.X == this.stoneMovedFrom.X && parent.stoneMovedTo.Y == this.stoneMovedFrom.Y;

                    this.CachedHeuristics = distanceSum * 5;
                }
            }

            return this.CachedHeuristics.Value;
        }

        public int DistanceTo(IGamePosition other)
        {
            //prefer pushing the same stone in next move
            SokobanPosition position = (SokobanPosition)other;
            return (this.stoneMovedTo.X == position.stoneMovedFrom.X && this.stoneMovedTo.Y == position.stoneMovedFrom.Y) ? 1 : 10;
        }

        public List<IGamePosition> GetSuccessors()
        {
            if (this.DeadlockMap == null)
            {
                this.CreateDeadlockMap();
            }

            List<IGamePosition> result = new List<IGamePosition>();
            Array.Clear(VisitedMap, 0, VisitedMap.Length);
            Stack[0] = this.Player;
            VisitedMap[this.Player.X, this.Player.Y] = true;
            int stackTop = 0;
            PointXY p;
            while (stackTop >=0)
            {
                p = Stack[stackTop];
                stackTop--;

                //try push left
                if ((this.Map[p.X - 1, p.Y] == Constants.STONE || this.Map[p.X - 1, p.Y] == Constants.GOALSTONE) && (this.Map[p.X - 2, p.Y] == Constants.EMPTY || this.Map[p.X - 2, p.Y] == Constants.GOAL) && !this.DeadlockMap[p.X-2, p.Y])
                {
                    var next = this.ClonePush(p, Direction.Left);
                    if (next.CheckDeadlockPattern(p.X-2, p.Y))
                    {
                        result.Add(next);
                    }
                }

                //try push right
                if ((this.Map[p.X + 1, p.Y] == Constants.STONE || this.Map[p.X + 1, p.Y] == Constants.GOALSTONE) && (this.Map[p.X + 2, p.Y] == Constants.EMPTY || this.Map[p.X + 2, p.Y] == Constants.GOAL) && !this.DeadlockMap[p.X + 2, p.Y])
                {
                    var next = this.ClonePush(p, Direction.Right);
                    if (next.CheckDeadlockPattern(p.X + 2, p.Y))
                    {
                        result.Add(next);
                    }
                }

                //try push up
                if ((this.Map[p.X, p.Y - 1] == Constants.STONE || this.Map[p.X, p.Y - 1] == Constants.GOALSTONE) && (this.Map[p.X, p.Y - 2] == Constants.EMPTY || this.Map[p.X, p.Y - 2] == Constants.GOAL) && !this.DeadlockMap[p.X, p.Y - 2])
                {
                    var next = this.ClonePush(p, Direction.Up);
                    if (next.CheckDeadlockPattern(p.X, p.Y - 2))
                    {
                        result.Add(next);
                    }
                }

                //try push down
                if ((this.Map[p.X, p.Y + 1] == Constants.STONE || this.Map[p.X, p.Y + 1] == Constants.GOALSTONE) && (this.Map[p.X, p.Y + 2] == Constants.EMPTY || this.Map[p.X, p.Y + 2] == Constants.GOAL) && !this.DeadlockMap[p.X, p.Y + 2])
                {
                    var next = this.ClonePush(p, Direction.Down);
                    if (next.CheckDeadlockPattern(p.X, p.Y + 2))
                    {
                        result.Add(next);
                    }
                }

                //try walk left
                if (!VisitedMap[p.X - 1, p.Y] && (this.Map[p.X - 1, p.Y] == Constants.EMPTY || this.Map[p.X - 1, p.Y] == Constants.GOAL))
                {
                    stackTop++;
                    Stack[stackTop].X = p.X - 1;
                    Stack[stackTop].Y = p.Y;
                    VisitedMap[p.X - 1, p.Y] = true;
                }

                //try walk right
                if (!VisitedMap[p.X + 1, p.Y] && (this.Map[p.X + 1, p.Y] == Constants.EMPTY || this.Map[p.X + 1, p.Y] == Constants.GOAL))
                {
                    stackTop++;
                    Stack[stackTop].X = p.X + 1;
                    Stack[stackTop].Y = p.Y;
                    VisitedMap[p.X + 1, p.Y] = true;
                }

                //try walk up
                if (!VisitedMap[p.X, p.Y - 1] && (this.Map[p.X, p.Y - 1] == Constants.EMPTY || this.Map[p.X, p.Y - 1] == Constants.GOAL))
                {
                    stackTop++;
                    Stack[stackTop].X = p.X;
                    Stack[stackTop].Y = p.Y - 1;
                    VisitedMap[p.X, p.Y - 1] = true;
                }

                //try walk down
                if (!VisitedMap[p.X, p.Y + 1] && (this.Map[p.X, p.Y + 1] == Constants.EMPTY || this.Map[p.X, p.Y + 1] == Constants.GOAL))
                {
                    stackTop++;
                    Stack[stackTop].X = p.X;
                    Stack[stackTop].Y = p.Y + 1;
                    VisitedMap[p.X, p.Y + 1] = true;
                }
            }

            return result;
        }

        private bool CheckDeadlockPattern(int px, int py)
        {
            int x1 = Math.Max(0, px - 1);
            int y1 = Math.Max(0, py - 1);
            int x2 = Math.Min(this.Width - 2, px);
            int y2 = Math.Min(this.Height - 2, py);

            //2x2 deadlock detection
            for (int x = x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    int d_stonesOnGoals = 0;
                    int d_stones = 0;
                    int d_walls = 0;

                    switch (this.Map[x, y])
                    {
                        case Constants.STONE:
                            d_stones++;
                            break;
                        case Constants.GOALSTONE:
                            d_stonesOnGoals++;
                            break;
                        case Constants.WALL:
                            d_walls++;
                            break;
                    }

                    if (d_stonesOnGoals == 1 || d_stones == 1 || d_walls == 1)
                    {
                        switch (this.Map[x + 1, y])
                        {
                            case Constants.STONE:
                                d_stones++;
                                break;
                            case Constants.GOALSTONE:
                                d_stonesOnGoals++;
                                break;
                            case Constants.WALL:
                                d_walls++;
                                break;
                        }

                        switch (this.Map[x, y + 1])
                        {
                            case Constants.STONE:
                                d_stones++;
                                break;
                            case Constants.GOALSTONE:
                                d_stonesOnGoals++;
                                break;
                            case Constants.WALL:
                                d_walls++;
                                break;
                        }

                        switch (this.Map[x + 1, y + 1])
                        {
                            case Constants.STONE:
                                d_stones++;
                                break;
                            case Constants.GOALSTONE:
                                d_stonesOnGoals++;
                                break;
                            case Constants.WALL:
                                d_walls++;
                                break;
                        }

                        if (d_stones > 0 && (d_stones + d_stonesOnGoals + d_walls == 4))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public string GetUniqueId()
        {
            bool changed = false;
            if (this.Binary == null)
            {
                this.Binary = new byte[2 + this.Width * this.Height / 8 + 1];
                for (int x = 0; x < this.Width; x++)
                {
                    for (int y = 0; y < this.Height; y++)
                    {
                        this.SetBinaryBit(x, y, (byte)((this.Map[x, y] == Constants.STONE || this.Map[x, y] == Constants.GOALSTONE) ? 1 : 0));
                    }
                }

                changed = true;
            }

            if (this.NormalizedPlayer.X == 0 || this.NormalizedPlayer.Y == 0)
            {
                Array.Clear(VisitedMap, 0, VisitedMap.Length);
                Stack[0] = this.Player;
                VisitedMap[this.Player.X, this.Player.Y] = true;
                int stackTop = 0;
                PointXY p;
                this.NormalizedPlayer.X = this.Width;
                this.NormalizedPlayer.Y = this.Height;
                while (stackTop >= 0)
                {
                    p = Stack[stackTop];
                    stackTop--;
                    if (p.Y < this.NormalizedPlayer.Y)
                    {
                        this.NormalizedPlayer = p;
                    }

                    if (p.Y == this.NormalizedPlayer.Y && p.X < this.NormalizedPlayer.X)
                    {
                        this.NormalizedPlayer = p;
                    }

                    //try walk left
                    if (!VisitedMap[p.X - 1, p.Y] && (this.Map[p.X - 1, p.Y] == Constants.EMPTY || this.Map[p.X - 1, p.Y] == Constants.GOAL))
                    {
                        stackTop++;
                        Stack[stackTop].X = p.X - 1;
                        Stack[stackTop].Y = p.Y;
                        VisitedMap[p.X - 1, p.Y] = true;
                    }

                    //try walk right
                    if (!VisitedMap[p.X + 1, p.Y] && (this.Map[p.X + 1, p.Y] == Constants.EMPTY || this.Map[p.X + 1, p.Y] == Constants.GOAL))
                    {
                        stackTop++;
                        Stack[stackTop].X = p.X + 1;
                        Stack[stackTop].Y = p.Y;
                        VisitedMap[p.X + 1, p.Y] = true;
                    }

                    //try walk up
                    if (!VisitedMap[p.X, p.Y - 1] && (this.Map[p.X, p.Y - 1] == Constants.EMPTY || this.Map[p.X, p.Y - 1] == Constants.GOAL))
                    {
                        stackTop++;
                        Stack[stackTop].X = p.X;
                        Stack[stackTop].Y = p.Y - 1;
                        VisitedMap[p.X, p.Y - 1] = true;
                    }

                    //try walk down
                    if (!VisitedMap[p.X, p.Y + 1] && (this.Map[p.X, p.Y + 1] == Constants.EMPTY || this.Map[p.X, p.Y + 1] == Constants.GOAL))
                    {
                        stackTop++;
                        Stack[stackTop].X = p.X;
                        Stack[stackTop].Y = p.Y + 1;
                        VisitedMap[p.X, p.Y + 1] = true;
                    }
                }

                this.Binary[0] = (byte)this.NormalizedPlayer.X;
                this.Binary[1] = (byte)this.NormalizedPlayer.Y;
                changed = true;
            }

            if (IsNullOrWhiteSpace(this.CachedUniqueId) || changed)
            {
                this.CachedUniqueId = Convert.ToBase64String(this.Binary);
                //this.CachedUniqueId = Encoding.Unicode.GetString(this.Binary);
                //this.CachedUniqueId = BitConverter.ToString(this.Binary).Replace("-", string.Empty);
            }

            return CachedUniqueId;
        }

        private void SetBinaryBit(int x, int y, int bit)
        {
            int i = (y * this.Width + x);
            int idx = 2 + i / 8;
            int offset = i % 8;
            byte mask = (byte)(1 << offset);
            if (bit == 1)
            {
                // set to 1
                this.Binary[idx] |= mask;
            } 
            else
            {
                this.Binary[idx] &= ((byte)~mask);
            }
        }

        private void CreateDeadlockMap()
        {
            this.DeadlockMap = new bool[this.Width, this.Height];
            bool[,] visited = new bool[this.Width, this.Height];
            PointXY[] stack = new PointXY[this.Width * this.Height];
            int stackTop = 0;
            PointXY p;
            var empty = new SokobanPosition() { Width = this.Width, Height = this.Height };
            empty.Map = new byte[this.Width, this.Height];
            empty.Stones = new PointXY[0];
            empty.Goals = new PointXY[0];
            empty.StonesMap = new byte[this.Width, this.Height];
            empty.Player = this.Player;

            // create list of goals
            var goals = new List<PointXY>();
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.Map[x, y] == Constants.GOAL || this.Map[x, y] == Constants.GOALSTONE)
                    {
                        PointXY g;
                        g.X = x;
                        g.Y = y;
                        goals.Add(g);
                    }

                    this.DeadlockMap[x, y] = true;
                    if (this.Map[x,y] == Constants.WALL)
                    {
                        empty.Map[x, y] = Constants.WALL;
                    }
                    else
                    {
                        empty.Map[x, y] = Constants.EMPTY;
                    }

                }
            }

            foreach (var checkGoal in goals)
            {
                //prepare map with one stone
                var start = new SokobanPosition() { Width = this.Width, Height = this.Height };
                start.Map = new byte[this.Width, this.Height];
                start.StonesMap = new byte[this.Width, this.Height];
                start.Stones = new PointXY[1];
                start.Goals = new PointXY[1];
                start.Goals[0] = checkGoal;
                start.Stones[0] = checkGoal;
                start.StonesMap[checkGoal.X, checkGoal.Y] = 1;
                start.Player = this.Player;
                for (int x = 0; x < this.Width; x++)
                {
                    for (int y = 0; y < this.Height; y++)
                    {
                        start.Map[x, y] = this.Map[x, y];
                        if (start.Map[x, y] == Constants.STONE || start.Map[x, y] == Constants.GOAL || start.Map[x, y] == Constants.GOALSTONE)
                        {
                            start.Map[x, y] = Constants.EMPTY;
                        }

                        if (x == checkGoal.X && y == checkGoal.Y)
                        {
                            start.Map[x, y] = Constants.STONE;
                            DeadlockMap[x, y] = false;
                        }
                    }
                }

                //search pull posibilities
                List<SokobanPosition> nodesToCheck = new List<SokobanPosition>();
                nodesToCheck.Add(start);
                HashSet<string> visitedNodes = new HashSet<string>();
                while (nodesToCheck.Count > 0)
                {
                    var node = nodesToCheck.ElementAt(0);
                    nodesToCheck.RemoveAt(0);
                    visitedNodes.Add(node.GetUniqueId());

                    // add pulls for any reachable player position
                    Array.Clear(visited, 0, visited.Length);
                    stack[0] = node.Player;
                    visited[node.Player.X, node.Player.Y] = true;
                    stackTop = 0;
                    while (stackTop >= 0)
                    {
                        p = stack[stackTop];
                        stackTop--;

                        //try pull right
                        if ((node.Map[p.X - 1, p.Y] == Constants.STONE) && (node.Map[p.X + 1, p.Y] == Constants.EMPTY))
                        {
                            var next = node.ClonePull(p, Direction.Right);
                            if (!visitedNodes.Contains(next.GetUniqueId()) && !nodesToCheck.Any(n => n.GetUniqueId() == next.GetUniqueId()))
                            {
                                nodesToCheck.Add(next);
                                this.DeadlockMap[p.X, p.Y] = false;
                            }
                        }

                        //try pull left
                        if ((node.Map[p.X + 1, p.Y] == Constants.STONE) && (node.Map[p.X - 1, p.Y] == Constants.EMPTY))
                        {
                            var next = node.ClonePull(p, Direction.Left);
                            if (!visitedNodes.Contains(next.GetUniqueId()) && !nodesToCheck.Any(n => n.GetUniqueId() == next.GetUniqueId()))
                            {
                                nodesToCheck.Add(next);
                                this.DeadlockMap[p.X, p.Y] = false;
                            }
                        }

                        //try pull down
                        if ((node.Map[p.X, p.Y - 1] == Constants.STONE) && (node.Map[p.X, p.Y + 1] == Constants.EMPTY))
                        {
                            var next = node.ClonePull(p, Direction.Down);
                            if (!visitedNodes.Contains(next.GetUniqueId()) && !nodesToCheck.Any(n => n.GetUniqueId() == next.GetUniqueId()))
                            {
                                nodesToCheck.Add(next);
                                this.DeadlockMap[p.X, p.Y] = false;
                            }
                        }

                        //try pull up
                        if ((node.Map[p.X, p.Y + 1] == Constants.STONE) && (node.Map[p.X, p.Y - 1] == Constants.EMPTY))
                        {
                            var next = node.ClonePull(p, Direction.Up);
                            if (!visitedNodes.Contains(next.GetUniqueId()) && !nodesToCheck.Any(n => n.GetUniqueId() == next.GetUniqueId()))
                            {
                                nodesToCheck.Add(next);
                                this.DeadlockMap[p.X, p.Y] = false;
                            }
                        }


                        //try walk left
                        if (!visited[p.X - 1, p.Y] && (node.Map[p.X - 1, p.Y] == Constants.EMPTY || node.Map[p.X - 1, p.Y] == Constants.GOAL))
                        {
                            stackTop++;
                            stack[stackTop].X = p.X - 1;
                            stack[stackTop].Y = p.Y;
                            visited[p.X - 1, p.Y] = true;
                        }

                        //try walk right
                        if (!visited[p.X + 1, p.Y] && (node.Map[p.X + 1, p.Y] == Constants.EMPTY || node.Map[p.X + 1, p.Y] == Constants.GOAL))
                        {
                            stackTop++;
                            stack[stackTop].X = p.X + 1;
                            stack[stackTop].Y = p.Y;
                            visited[p.X + 1, p.Y] = true;
                        }

                        //try walk up
                        if (!visited[p.X, p.Y - 1] && (node.Map[p.X, p.Y - 1] == Constants.EMPTY || node.Map[p.X, p.Y - 1] == Constants.GOAL))
                        {
                            stackTop++;
                            stack[stackTop].X = p.X;
                            stack[stackTop].Y = p.Y - 1;
                            visited[p.X, p.Y - 1] = true;
                        }

                        //try walk down
                        if (!visited[p.X, p.Y + 1] && (node.Map[p.X, p.Y + 1] == Constants.EMPTY || node.Map[p.X, p.Y + 1] == Constants.GOAL))
                        {
                            stackTop++;
                            stack[stackTop].X = p.X;
                            stack[stackTop].Y = p.Y + 1;
                            visited[p.X, p.Y + 1] = true;
                        }
                    }
                }
            }

            // remove deadlocks outside reachable area
            Array.Clear(visited, 0, visited.Length);
            stack[0] = empty.Player;
            visited[empty.Player.X, empty.Player.Y] = true;
            stackTop = 0;
            while (stackTop >= 0)
            {
                p = stack[stackTop];
                stackTop--;

                //try walk left
                if (!visited[p.X - 1, p.Y] && (empty.Map[p.X - 1, p.Y] == Constants.EMPTY || empty.Map[p.X - 1, p.Y] == Constants.GOAL))
                {
                    stackTop++;
                    stack[stackTop].X = p.X - 1;
                    stack[stackTop].Y = p.Y;
                    visited[p.X - 1, p.Y] = true;
                }

                //try walk right
                if (!visited[p.X + 1, p.Y] && (empty.Map[p.X + 1, p.Y] == Constants.EMPTY || empty.Map[p.X + 1, p.Y] == Constants.GOAL))
                {
                    stackTop++;
                    stack[stackTop].X = p.X + 1;
                    stack[stackTop].Y = p.Y;
                    visited[p.X + 1, p.Y] = true;
                }

                //try walk up
                if (!visited[p.X, p.Y - 1] && (empty.Map[p.X, p.Y - 1] == Constants.EMPTY || empty.Map[p.X, p.Y - 1] == Constants.GOAL))
                {
                    stackTop++;
                    stack[stackTop].X = p.X;
                    stack[stackTop].Y = p.Y - 1;
                    visited[p.X, p.Y - 1] = true;
                }

                //try walk down
                if (!visited[p.X, p.Y + 1] && (empty.Map[p.X, p.Y + 1] == Constants.EMPTY || empty.Map[p.X, p.Y + 1] == Constants.GOAL))
                {
                    stackTop++;
                    stack[stackTop].X = p.X;
                    stack[stackTop].Y = p.Y + 1;
                    visited[p.X, p.Y + 1] = true;
                }
            }

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (!visited[x,y])
                    {
                        this.DeadlockMap[x, y] = false;
                    }
                }
            }
        }

        public SokobanPosition Clone()
        {
            var clone = new SokobanPosition();
            clone.Parent = this;
            clone.Width = this.Width;
            clone.Height = this.Height;
            clone.Map = new byte[this.Width, this.Height];
            Array.Copy(this.Map, clone.Map, this.Map.Length);
            clone.Player = this.Player;
            clone.Goals = this.Goals;
            clone.Stones = new PointXY[this.Stones.Length];
            Array.Copy(this.Stones, clone.Stones, this.Stones.Length);
            clone.StonesMap = new byte[this.Width, this.Height];
            Array.Copy(this.StonesMap, clone.StonesMap, this.StonesMap.Length);
            clone.NormalizedPlayer = this.NormalizedPlayer;
            clone.DeadlockMap = this.DeadlockMap;
            if (this.Binary != null)
            {
                clone.Binary = new byte[this.Binary.Length];
                Array.Copy(this.Binary, clone.Binary, this.Binary.Length);
            }

            return clone;
        }

        public SokobanPosition ClonePush(PointXY p, Direction direction)
        {
            var clone = this.Clone();
            switch (direction)
            {
                case Direction.Left:
                    clone.Player.X = p.X - 1;
                    clone.Player.Y = p.Y;
                    clone.Map[p.X - 1, p.Y] = this.Map[p.X - 1, p.Y] == Constants.STONE ? Constants.EMPTY : Constants.GOAL;
                    clone.Map[p.X - 2, p.Y] = this.Map[p.X - 2, p.Y] == Constants.EMPTY ? Constants.STONE : Constants.GOALSTONE;
                    clone.stoneMovedFrom.X = p.X - 1;
                    clone.stoneMovedFrom.Y = p.Y;
                    clone.stoneMovedTo.X = p.X - 2;
                    clone.stoneMovedTo.Y = p.Y;
                    break;
                case Direction.Right:
                    clone.Player.X = p.X + 1;
                    clone.Player.Y = p.Y;
                    clone.Map[p.X + 1, p.Y] = this.Map[p.X + 1, p.Y] == Constants.STONE ? Constants.EMPTY : Constants.GOAL;
                    clone.Map[p.X + 2, p.Y] = this.Map[p.X + 2, p.Y] == Constants.EMPTY ? Constants.STONE : Constants.GOALSTONE;
                    clone.stoneMovedFrom.X = p.X + 1;
                    clone.stoneMovedFrom.Y = p.Y;
                    clone.stoneMovedTo.X = p.X + 2;
                    clone.stoneMovedTo.Y = p.Y;
                    break;
                case Direction.Up:
                    clone.Player.X = p.X;
                    clone.Player.Y = p.Y - 1;
                    clone.Map[p.X, p.Y - 1] = this.Map[p.X, p.Y - 1] == Constants.STONE ? Constants.EMPTY : Constants.GOAL;
                    clone.Map[p.X, p.Y - 2] = this.Map[p.X, p.Y - 2] == Constants.EMPTY ? Constants.STONE : Constants.GOALSTONE;
                    clone.stoneMovedFrom.X = p.X;
                    clone.stoneMovedFrom.Y = p.Y - 1;
                    clone.stoneMovedTo.X = p.X;
                    clone.stoneMovedTo.Y = p.Y - 2;
                    break;
                case Direction.Down:
                    clone.Player.X = p.X;
                    clone.Player.Y = p.Y + 1;
                    clone.Map[p.X, p.Y + 1] = this.Map[p.X, p.Y + 1] == Constants.STONE ? Constants.EMPTY : Constants.GOAL;
                    clone.Map[p.X, p.Y + 2] = this.Map[p.X, p.Y + 2] == Constants.EMPTY ? Constants.STONE : Constants.GOALSTONE;
                    clone.stoneMovedFrom.X = p.X;
                    clone.stoneMovedFrom.Y = p.Y + 1;
                    clone.stoneMovedTo.X = p.X;
                    clone.stoneMovedTo.Y = p.Y + 2;
                    break;
            }

            int stoneIdx = clone.StonesMap[clone.stoneMovedFrom.X, clone.stoneMovedFrom.Y];
            clone.StonesMap[clone.stoneMovedTo.X, clone.stoneMovedTo.Y] = (byte)stoneIdx;
            clone.StonesMap[clone.stoneMovedFrom.X, clone.stoneMovedFrom.Y] = 0;
            clone.Stones[stoneIdx - 1] = clone.stoneMovedTo;

            clone.NormalizedPlayer.X = 0;
            clone.NormalizedPlayer.Y = 0;
            if (this.Binary != null)
            {
                switch (direction)
                {
                    case Direction.Left:
                        clone.SetBinaryBit(p.X - 1, p.Y, 0);
                        clone.SetBinaryBit(p.X - 2, p.Y, 1);
                        break;
                    case Direction.Right:
                        clone.SetBinaryBit(p.X + 1, p.Y, 0);
                        clone.SetBinaryBit(p.X + 2, p.Y, 1);
                        break;
                    case Direction.Up:
                        clone.SetBinaryBit(p.X, p.Y - 1, 0);
                        clone.SetBinaryBit(p.X, p.Y - 2, 1);
                        break;
                    case Direction.Down:
                        clone.SetBinaryBit(p.X, p.Y + 1, 0);
                        clone.SetBinaryBit(p.X, p.Y + 2, 1);
                        break;
                }
            }

            return clone;
        }

        public SokobanPosition ClonePull(PointXY p, Direction direction)
        {
            var clone = this.Clone();
            switch (direction)
            {
                case Direction.Left:
                    clone.Player.X = p.X - 1;
                    clone.Player.Y = p.Y;
                    clone.Map[p.X + 1, p.Y] = this.Map[p.X + 1, p.Y] == Constants.STONE ? Constants.EMPTY : Constants.GOAL;
                    clone.Map[p.X, p.Y] = this.Map[p.X, p.Y] == Constants.EMPTY ? Constants.STONE : Constants.GOALSTONE;
                    clone.stoneMovedFrom.X = p.X + 1;
                    clone.stoneMovedFrom.Y = p.Y;
                    clone.stoneMovedTo.X = p.X;
                    clone.stoneMovedTo.Y = p.Y;
                    break;
                case Direction.Right:
                    clone.Player.X = p.X + 1;
                    clone.Player.Y = p.Y;
                    clone.Map[p.X - 1, p.Y] = this.Map[p.X - 1, p.Y] == Constants.STONE ? Constants.EMPTY : Constants.GOAL;
                    clone.Map[p.X, p.Y] = this.Map[p.X, p.Y] == Constants.EMPTY ? Constants.STONE : Constants.GOALSTONE;
                    clone.stoneMovedFrom.X = p.X - 1;
                    clone.stoneMovedFrom.Y = p.Y;
                    clone.stoneMovedTo.X = p.X;
                    clone.stoneMovedTo.Y = p.Y;
                    break;
                case Direction.Up:
                    clone.Player.X = p.X;
                    clone.Player.Y = p.Y - 1;
                    clone.Map[p.X, p.Y + 1] = this.Map[p.X, p.Y + 1] == Constants.STONE ? Constants.EMPTY : Constants.GOAL;
                    clone.Map[p.X, p.Y] = this.Map[p.X, p.Y] == Constants.EMPTY ? Constants.STONE : Constants.GOALSTONE;
                    clone.stoneMovedFrom.X = p.X;
                    clone.stoneMovedFrom.Y = p.Y + 1;
                    clone.stoneMovedTo.X = p.X;
                    clone.stoneMovedTo.Y = p.Y;
                    break;
                case Direction.Down:
                    clone.Player.X = p.X;
                    clone.Player.Y = p.Y + 1;
                    clone.Map[p.X, p.Y - 1] = this.Map[p.X, p.Y - 1] == Constants.STONE ? Constants.EMPTY : Constants.GOAL;
                    clone.Map[p.X, p.Y] = this.Map[p.X, p.Y] == Constants.EMPTY ? Constants.STONE : Constants.GOALSTONE;
                    clone.stoneMovedFrom.X = p.X;
                    clone.stoneMovedFrom.Y = p.Y - 1;
                    clone.stoneMovedTo.X = p.X;
                    clone.stoneMovedTo.Y = p.Y;
                    break;
            }

            int stoneIdx = clone.StonesMap[clone.stoneMovedFrom.X, clone.stoneMovedFrom.Y];
            clone.StonesMap[clone.stoneMovedTo.X, clone.stoneMovedTo.Y] = (byte)stoneIdx;
            clone.StonesMap[clone.stoneMovedFrom.X, clone.stoneMovedFrom.Y] = 0;
            clone.Stones[stoneIdx - 1] = clone.stoneMovedTo;

            clone.NormalizedPlayer.X = 0;
            clone.NormalizedPlayer.Y = 0;
            if (this.Binary != null)
            {
                //clone.Binary[2 + py * this.Width + px] = 0;
                switch (direction)
                {
                    case Direction.Left:
                        //clone.Binary[2 + py * this.Width + px - 1] = 1;
                        clone.SetBinaryBit(p.X + 1, p.Y, 0);
                        clone.SetBinaryBit(p.X, p.Y, 1);
                        break;
                    case Direction.Right:
                        //clone.Binary[2 + py * this.Width + px + 1] = 1;
                        clone.SetBinaryBit(p.X - 1, p.Y, 0);
                        clone.SetBinaryBit(p.X, p.Y, 1);
                        break;
                    case Direction.Up:
                        //clone.Binary[2 + (py - 1) * this.Width + px] = 1;
                        clone.SetBinaryBit(p.X, p.Y + 1, 0);
                        clone.SetBinaryBit(p.X, p.Y, 1);
                        break;
                    case Direction.Down:
                        //clone.Binary[2 + (py + 1) * this.Width + px] = 1;
                        clone.SetBinaryBit(p.X, p.Y - 1, 0);
                        clone.SetBinaryBit(p.X, p.Y, 1);
                        break;
                }
            }

            return clone;
        }

        public override string ToString()
        {
            string str = string.Empty;
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    if (this.Player.X == x && this.Player.Y == y)
                    {
                        str += "@";
                    }
                    else
                    {
                        switch (this.Map[x, y])
                        {
                            case Constants.WALL:
                                str += "#";
                                break;
                            case Constants.STONE:
                                str += "$";
                                break;
                            case Constants.GOAL:
                                str += ".";
                                break;
                            case Constants.GOALSTONE:
                                str += "*";
                                break;
                            default:
                                str += " ";
                                break;
                        }
                    }
                }

                str += "\r\n";
            }

            return str;
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }

    public struct PointXY
    {
        public int X;

        public int Y;
    }


}
