using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Checkers.Forms.Models
{
    [Serializable]
    public class Board : IEnumerable<int>
    {
        private int[,] _board;
        public const int BoardSize = 8;

        public int this[int i, int j]
        {
            get
            {
                return _board[i, j];
            }
            set
            {
                _board[i, j] = value;
            }
        }
        public Board()
        {
            _board = new int[BoardSize, BoardSize]{
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 2, 0, 2, 0, 2, 0, 2, 0 },
                { 0, 2, 0, 2, 0, 2, 0, 2 },
                { 2, 0, 2, 0, 2, 0, 2, 0 }
            };
        }

        public Board(int[,] bytes)
        {
            _board = bytes;
        }

        public int GetSize()
        {
            return BoardSize;
        }

        public void CopyTo(Board board)
        {
            for (int i = 0; i < GetSize(); i++)
            {
                for (int j = 0; j < GetSize(); j++)
                {
                    board[i, j] = this[i, j];
                }
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return (IEnumerator<int>)_board.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < GetSize(); i++)
            {
                for (int j = 0; j < GetSize(); j++)
                {
                    builder.Append(this[i, j] + " ");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
