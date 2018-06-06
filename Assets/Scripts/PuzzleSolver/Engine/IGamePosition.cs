using System.Collections.Generic;

namespace SokobanSolver.Engine
{
    public interface IGamePosition
    {
        IGamePosition Parent { get; set; }

        int Heuristics();

        int DistanceTo(IGamePosition other);

        string GetUniqueId();

        List<IGamePosition> GetSuccessors();
    }
}
