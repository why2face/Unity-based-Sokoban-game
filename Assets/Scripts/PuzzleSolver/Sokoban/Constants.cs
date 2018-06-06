namespace SokobanSolver.Sokoban
{
    public static class Constants
    {
        public const byte EMPTY = 0;

        public const byte STONE = 1;

        public const byte GOAL = 2;

        public const byte GOALSTONE = 3;

        public const byte WALL = 4;

        public const int InteralStackMaxSize = 400;

        public const int InternalMapMaxWidth = 20;

        public const int InternalMapMaxHeight = 20;
    }

    public enum Direction : int
    {
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4
    }
}
