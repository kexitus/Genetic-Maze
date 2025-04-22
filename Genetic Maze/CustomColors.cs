using System.Drawing;

namespace Genetic_Maze
{
    public static class CustomColors
    {
         public static Color Empty { get; } = Color.FromArgb(43, 43, 48);
         public static Color Background { get; } = Color.FromArgb(65, 68, 96);
         public static Color TextField { get; } = Color.FromArgb(87, 107, 158);
         public static Color DarkContent { get; } = Color.FromArgb(111, 125, 175);
         public static Color Content { get; } = Color.FromArgb(132, 158, 223);
         public static Color Start { get; } = Color.FromArgb(212, 123, 58);
         public static Color End { get; } = Color.FromArgb(47, 218, 105);
         public static Color Individual { get; } = Color.FromArgb(214, 26, 42);
    }
}