using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Checkers.Forms.Extensions
{
    public static class FormControlsExtension
    {
        public static int GetX(this Button button)
        {
            return button.Location.X;
        }

        public static int GetY(this Button button)
        {
            return button.Location.Y;
        }

        public static int GetRelativeX(this Button button, int cellSize)
        {
            return button.GetX() / cellSize;
        }

        public static int GetRelativeY(this Button button, int cellSize)
        {
            return button.GetY() / cellSize;
        }
    }
}
