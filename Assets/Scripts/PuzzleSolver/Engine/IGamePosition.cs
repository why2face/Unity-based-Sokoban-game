using System.Collections.Generic;

namespace PanJanek.SokobanSolver.Engine
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
