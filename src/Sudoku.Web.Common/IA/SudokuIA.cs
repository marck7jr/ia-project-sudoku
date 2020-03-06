using System;
using System.Collections.Generic;

namespace Sudoku.Web.Common.IA
{
    public class SudokuIA
    {
        // Matriz responsável por receber o tabuleiro
        private char[,] _board;
        // Stack é um tipo de implementação da estrutura de LIFO (Last In First Out) também conhecida como pilha
        private Stack<(int Index, IList<char> Values)> _stack;

        /// <summary>
        /// Construtor do SudokuIA
        /// </summary>
        /// <param name="board">
        /// Espera receber uma matriz 2D com as dimensões 9x9.
        /// </param>
        public SudokuIA(char[,] board)
        {
            if (!(board.GetLength(0) is 9))
            {
                throw new ArgumentException("Uma matriz 9x9 deve passada, por favor verifique se o arquivo contém 81 digítos e também se não possui quebras de linhas entre eles.");
            }
            _board = board;
            _stack = new Stack<(int Index, IList<char> Values)>();
        }

        public char[,] GetResult()
        {
            return _board;
        }

        /// <summary>
        /// Método responsável por gerar uma nova possível solução.
        /// </summary>
        /// <param name="index">
        /// Valor inteiro entre 0 e 80 que serve para descobrir a origem do item a ser avaliado.
        /// Por exemplo, o 47 quando divido por 9 tem como divisor 5 que resulta no valor 45 e o valor do resto, 2, seria o representante da coluna.
        /// </param>
        /// <returns>
        /// Retorna uma lista de caracteres com os possíveis valores da solução do sudoku
        /// </returns>

        private IList<char> GetSeed(int index)
        {
            // Obtém Linha
            int row = index / 9;
            // Obtém Coluna
            int column = index % 9;

            // Lista responsável por gerar o resultante
            IList<char> result = new List<char>(9);

            // O tipo HashSet é uma implementação de Lista que não permite valores duplicados
            // HashSet para a validação da Linha
            ISet<char> rowValidation = new HashSet<char>();
            // HashSet para a validação da Coluna
            ISet<char> columnValidation = new HashSet<char>();
            // HashSet para a validação da Area 3x3
            ISet<char> areaValidation = new HashSet<char>();

            // Faz a iteração na tabuleiro
            for (int i = 0; i < 9; i++)
            {
                // Insere todos os valores da coluna do item avaliado diferente de 0 na lista para validação da coluna
                if (_board[i, column] != '0')
                {
                    columnValidation.Add(_board[i, column]);
                }

                // Insere todos os valores da linha do item avaliado diferente de 0 na lista para validação da linha
                if (_board[row, i] != '0')
                {
                    rowValidation.Add(_board[row, i]);
                }
            }

            // Obtém o indice de inicio do bloco, por exemplo, o item no indice 47 tem como valores para linha e coluna, 5 e 2 respectivamente.
            // Logo, o inicio do bloco daria pela divisão do valor da linha por 3.
            // Por exemplo, 5 / 3 é igual a 1 seria atribuído à areaX
            int areaX = row / 3;
            // Enquanto, 2 / 3 é igual a 0 seria atribuído à areaY
            int areaY = column / 3;

            // Itera novamente no tabuleiro, desta vez procurando os valores pré-existentes do bloco no qual o item avaliado pertence
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

            // For final, insere na lista de resultado todos os valores de 0 a 9 que NÂO contém em nenhuma das listas de validação
            for (char ch = '1'; ch <= '9'; ch++)
            {
                if (!areaValidation.Contains(ch) && !rowValidation.Contains(ch) && !columnValidation.Contains(ch))
                {
                    result.Add(ch);
                }
            }

            return result;
        }

        /// <summary>
        /// Função responsável por processar e alterar o tabuleiro com os valores da solução encontrada.
        /// </summary>

        public void SolveSudoku()
        {
            // Valor reservado para encontrar o indice inicial que será o primeiro item encontrado que, no qual, o valor encontrado é igual a 0. Pode variar entre 0 e 80.
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

            // Se não houver nenhum valor 0, a solução foi encontrada
            if (!startPoint.HasValue)
            {
                return;
            }

            // Insere na pilha o indice inicial junto com a primeira coleção de valores de uma possível solução.
            _stack.Push((startPoint.Value, GetSeed(startPoint.Value)));

            // Enquanto tiver itens na pilha
            while (_stack.Count > 0)
            {
                // Retira do top da pilha SEM remove-la.
                var (Index, Values) = _stack.Peek();
                // Se a lista com os valores de uma possivel solução estiver vazia, inicia o gatilho do backtracking na busca de soluções alternativas
                if (Values.Count == 0)
                {
                    // Retira da pilha
                    _stack.Pop();
                    // Atualiza o seu valor para 0
                    _board[Index / 9, Index % 9] = '0';
                    // Ignora o fluxo e passa para a próxima iteração.
                    continue;
                }

                // Pega o ultimo possível valor da solução encontrado, que no caso, segue a lógica da busca em profundidade com backtracking
                // que seria um refinamento das possíveis soluções que podem ser examinadas sem serem explicitamente examinadas
                var lastChar = Values[Values.Count - 1];
                // Atualiza o tabuleiro com ele
                _board[Index / 9, Index % 9] = lastChar;
                // Remove ele da lista dos possíveis valores
                Values.RemoveAt(Values.Count - 1);

                // Pega o primeiro item vizinho
                var next = Index + 1;
                // Iteração que ira repetir até encontrar um vizinho com valor 0 OU se o indice do vizinho for maior que 80
                while (true)
                {
                    // Se for maior que 80, a solução foi encontrada
                    if (next > 80)
                    {
                        return;
                    }

                    // Se o valor do vizinho for igual a zero, a iteração é interrompida.
                    if (_board[next / 9, next % 9] == '0')
                    {
                        break;
                    }

                    // Incrementa o valor do vizinho para pegar o "vizinho do vizinho"
                    next++;
                }

                // Pega os valores possíveis encontrados para o valor vizinho
                var newSeed = GetSeed(next);
                // Insere o vizinho na pilha junto com os valores possíveis como nodo da árvore de busca
                _stack.Push((next, newSeed));
            }
        }
    }
}
