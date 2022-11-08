using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Checkers.Forms.Extensions
{
    public static class MatrixExtensions
    {
        public static Tuple<int, int> Find(this Button[,] matrix, Button btn)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    if(matrix[i, j] == btn)
                    {
                        return Tuple.Create(i, j);
                    }
                }
            }

            return Tuple.Create(-1, -1);
        }
    }
}
