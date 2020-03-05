using System.Collections.Generic;

namespace Sudoku.Web.Common.IA
{
    public class SudokuIA
    {
        private char[,] _board;

        public SudokuIA(char[,] board)
        {
            _board = board;
        }

        public char[,] GetResult()
        {
            return _board;
        }

        private IList<char> GetSeed(int index)
        {
            int row = index / 9;
            int column = index % 9;

            IList<char> result = new List<char>(9);

            ISet<char> rowValidation = new HashSet<char>();
            ISet<char> columnValidation = new HashSet<char>();
            ISet<char> areaValidation = new HashSet<char>();

            for (int i = 0; i < 9; i++)
            {
                if (_board[i, column] != '0')
                {
                    columnValidation.Add(_board[i, column]);
                }

                if (_board[row, i] != '0')
                {
                    rowValidation.Add(_board[row, i]);
                }
            }

            int areaX = row / 3;
            int areaY = column / 3;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (_board[areaX * 3 + i, areaY * 3 + j] != '.')
                    {
                        areaValidation.Add(_board[areaX * 3 + i, areaY * 3 + j]);
                    }
                }
            }

            for (char ch = '1'; ch <= '9'; ch++)
            {
                if (!areaValidation.Contains(ch) && !rowValidation.Contains(ch) && !columnValidation.Contains(ch))
                {
                    result.Add(ch);
                }
            }

            return result;
        }

        public void SolveSudoku()
        {
            int? startPoint = null;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_board[i, j] == '0')
                    {
                        startPoint = i * 9 + j;
                        break;
                    }
                }

                if (startPoint != null)
                {
                    break;
                }
            }

            if (!startPoint.HasValue)
            {
                return;
            }

            Stack<(int Index, IList<char> Values)> stack = new Stack<(int Index, IList<char> Values)>();
            stack.Push((startPoint.Value, GetSeed(startPoint.Value)));

            while (stack.Count > 0)
            {
                var peek = stack.Peek(); // Retira do top da pilha SEM remove-la.
                if (peek.Values.Count == 0)
                {
                    stack.Pop();
                    _board[peek.Index / 9, peek.Index % 9] = '0';
                    continue;
                }

                var lastChar = peek.Values[peek.Values.Count - 1];
                _board[peek.Index / 9, peek.Index % 9] = lastChar;
                peek.Values.RemoveAt(peek.Values.Count - 1);

                var next = peek.Index + 1;
                while (true)
                {
                    if (next > 80)
                    {
                        return;
                    }

                    if (_board[next / 9, next % 9] == '0')
                    {
                        break;
                    }

                    next++;
                }

                var newSeed = GetSeed(next);
                stack.Push((next, newSeed));
            }
        }
    }
}
