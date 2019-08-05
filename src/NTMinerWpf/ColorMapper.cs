using System;
using System.Windows.Media;

namespace NTMiner {
    public static class ColorMapper {
        public static Color ToMediaColor(this ConsoleColor consoleColor) {
            switch (consoleColor) {
                case ConsoleColor.Black:
                    return Colors.Black;
                case ConsoleColor.DarkBlue:
                    return Colors.DarkBlue;
                case ConsoleColor.DarkGreen:
                    return Colors.DarkGreen;
                case ConsoleColor.DarkCyan:
                    return Colors.DarkCyan;
                case ConsoleColor.DarkRed:
                    return Colors.DarkRed;
                case ConsoleColor.DarkMagenta:
                    return Colors.DarkMagenta;
                case ConsoleColor.DarkYellow:
                    return Colors.LightGoldenrodYellow;
                case ConsoleColor.Gray:
                    return Colors.Gray;
                case ConsoleColor.DarkGray:
                    return Colors.DarkGray;
                case ConsoleColor.Blue:
                    return Colors.Blue;
                case ConsoleColor.Green:
                    return Colors.Green;
                case ConsoleColor.Cyan:
                    return Colors.Cyan;
                case ConsoleColor.Red:
                    return Colors.Red;
                case ConsoleColor.Magenta:
                    return Colors.Magenta;
                case ConsoleColor.Yellow:
                    return Colors.Yellow;
                case ConsoleColor.White:
                    return Colors.White;
                default:
                    return Colors.White;
            }
        }

        public static System.Drawing.Color ToDrawingColor(this ConsoleColor consoleColor) {
            switch (consoleColor) {
                case ConsoleColor.Black:
                    return System.Drawing.Color.Black;
                case ConsoleColor.DarkBlue:
                    return System.Drawing.Color.DarkBlue;
                case ConsoleColor.DarkGreen:
                    return System.Drawing.Color.DarkGreen;
                case ConsoleColor.DarkCyan:
                    return System.Drawing.Color.DarkCyan;
                case ConsoleColor.DarkRed:
                    return System.Drawing.Color.DarkRed;
                case ConsoleColor.DarkMagenta:
                    return System.Drawing.Color.DarkMagenta;
                case ConsoleColor.DarkYellow:
                    return System.Drawing.Color.Yellow;
                case ConsoleColor.Gray:
                    return System.Drawing.Color.Gray;
                case ConsoleColor.DarkGray:
                    return System.Drawing.Color.DarkGray;
                case ConsoleColor.Blue:
                    return System.Drawing.Color.Blue;
                case ConsoleColor.Green:
                    return System.Drawing.Color.Green;
                case ConsoleColor.Cyan:
                    return System.Drawing.Color.Cyan;
                case ConsoleColor.Red:
                    return System.Drawing.Color.Red;
                case ConsoleColor.Magenta:
                    return System.Drawing.Color.Magenta;
                case ConsoleColor.Yellow:
                    return System.Drawing.Color.Yellow;
                case ConsoleColor.White:
                    return System.Drawing.Color.White;
                default:
                    return System.Drawing.Color.White;
            }
        }

        public static ConsoleColor ToConsoleColor(this Color color) {
            if (Colors.Black == color)
                return ConsoleColor.Black;
            else if (Colors.DarkBlue == color)
                return ConsoleColor.DarkBlue;
            else if (Colors.DarkGreen == color)
                return ConsoleColor.DarkGreen;
            else if (Colors.DarkCyan == color)
                return ConsoleColor.DarkCyan;
            else if (Colors.DarkRed == color)
                return ConsoleColor.DarkRed;
            else if (Colors.DarkMagenta == color)
                return ConsoleColor.DarkMagenta;
            else if (Colors.LightGoldenrodYellow == color)
                return ConsoleColor.DarkYellow;
            else if (Colors.Gray == color)
                return ConsoleColor.Gray;
            else if (Colors.DarkGray == color)
                return ConsoleColor.DarkGray;
            else if (Colors.Blue == color)
                return ConsoleColor.Blue;
            else if (Colors.Green == color)
                return ConsoleColor.Green;
            else if (Colors.Cyan == color)
                return ConsoleColor.Cyan;
            else if (Colors.Red == color)
                return ConsoleColor.Red;
            else if (Colors.Magenta == color)
                return ConsoleColor.Magenta;
            else if (Colors.Yellow == color)
                return ConsoleColor.Yellow;
            else if (color == Colors.White)
                return ConsoleColor.White;
            else {
                return ConsoleColor.White;
            }
        }
    }
}
