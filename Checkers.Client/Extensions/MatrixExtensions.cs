using Checkers.Forms.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public static void WriteToStream(this Board board, StreamWriter writer)
        {
            string res = "";

            for (int i = 0; i < board.GetSize(); i++)
            {
                for (int j = 0; j < board.GetSize(); j++)
                {
                    res += byte.Parse(board[i, j].ToString());
                }
            }

            writer.WriteLine(res);
        }

        public static void Parse(this Board board, string unparsed)
        {
            if (string.IsNullOrEmpty(unparsed))
            {
                return;
            }

            string[] rows = new string[board.GetSize()];
            int sigma = 0;

            for (int k = 0; k < board.GetSize(); k++)
            {
                int startIndex = board.GetSize() * sigma;
                int length = board.GetSize();
                rows[k] = unparsed.Substring(startIndex, length);
                sigma++;
            }


            for (int i = 0; i < board.GetSize(); i++)
            {
                for (int j = 0; j < board.GetSize(); j++)
                {
                    board[i, j] = int.Parse(rows[i][j].ToString());
                }
            }
        }
    }
}
