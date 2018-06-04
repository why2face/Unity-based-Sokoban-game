using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PanJanek.SokobanSolver.Engine
{
    public class Solver<T> where T : IGamePosition
    {
        public const int CollectionsSize = 10000000;

        public Solution<T> AStar(T startPosition)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            FastPriorityQueue<SearchNode<T>> priorityQueue = new FastPriorityQueue<SearchNode<T>>(CollectionsSize);
            HashSet<string> visited = new HashSet<string>();
            Dictionary<string, SearchNode<T>> queueMap = new Dictionary<string, SearchNode<T>>(CollectionsSize / 10);
            SearchNode<T> startNode = new SearchNode<T>() { Position = startPosition, Key = startPosition.GetUniqueId(), G = 0 };
            priorityQueue.Enqueue(startNode, 0);
            queueMap[startNode.Position.GetUniqueId()] = startNode;
            int expandedCount = 0;
            while (priorityQueue.Count > 0)
            {
                var node = priorityQueue.Dequeue();
                expandedCount++;
                queueMap.Remove(node.Position.GetUniqueId());
                visited.Add(node.Position.GetUniqueId());
                var childredPositions = node.Position.GetSuccessors();
                foreach (var childrenPosition in childredPositions)
                {
                    if (!visited.Contains(childrenPosition.GetUniqueId()))
                    {
                        var childNode = new SearchNode<T>() { Position = (T)childrenPosition, Key = childrenPosition.GetUniqueId() };
                        int distance = node.Position.DistanceTo(childNode.Position);
                        int t = node.G + distance;
                        bool isBetter = false;
                        if (!queueMap.ContainsKey(childNode.Key))
                        {
                            isBetter = true;
                        }
                        else if (t < childNode.G)
                        {
                            isBetter = true;
                        }

                        if (isBetter)
                        {
                            childNode.H = childNode.Position.Heuristics();
                            if (childNode.H == 0)
                            {
                                stopwatch.Stop();
                                Solution<T> solution = new Solution<T>();
                                solution.FinalPosition = childNode.Position;
                                solution.VisitedNodesCount = visited.Count;
                                solution.Time = stopwatch.Elapsed;
                                solution.ExpandedNodesCount = expandedCount;
                                return solution;
                            }
                            else if (childNode.H == int.MaxValue)
                            {
                                visited.Add(childNode.Key);
                            }
                            else
                            {

                                childNode.G = t;
                                childNode.F = childNode.G + childNode.H;
                                if (!queueMap.ContainsKey(childNode.Key))
                                {
                                    priorityQueue.Enqueue(childNode, childNode.F);
                                    queueMap[childNode.Key] = childNode;
                                }
                                else
                                {
                                    var existing = queueMap[childNode.Key];
                                    priorityQueue.UpdatePriority(existing, childNode.F);
                                }
                            }
                        }
                    }
                }
            }

            stopwatch.Stop();
            return new Solution<T>() { Time = stopwatch.Elapsed, FinalPosition = default(T), VisitedNodesCount = visited.Count, ExpandedNodesCount = expandedCount };
        }
    }

    public class SearchNode<T> : FastPriorityQueueNode where T : IGamePosition
    {
        public T Position;

        public int G;

        public int H;

        public int F;

        public string Key;
    }

    public class Solution<T> where T : IGamePosition
    {
        public T FinalPosition;

        public int VisitedNodesCount;

        public int ExpandedNodesCount;

        public TimeSpan Time;

        public List<T> GetPath()
        {
            List<T> path = new List<T>();
            var current = this.FinalPosition;
            while (current.Parent != null)
            {
                path.Add(current);
                current = (T)current.Parent;
            }

            path.Add(current);
            path.Reverse();
            return path;
        }
    }
}
