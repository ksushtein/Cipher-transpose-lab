using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr5_CipherTranspos
{
    public class MatrixHandler
    {

        void FillMatrix(ref char[,] matrix, int r, int c, ref int m, string message)
        {
            if (m < message.Length)
            {
                matrix[r, c] = message[m];
                m++;
            }
            else
                matrix[r, c] = '_';
        }

        /// <summary>
        /// Записывает построчно в матрицу
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cRows">Строка</param>
        /// <param name="cCols">Столбец</param>
        /// <returns></returns>
        public char[,] GetMatrix(string message, int cRows, int cCols, Input input)
        {
            char[,] matrix = new char[cRows, cCols];
            int m = 0;

            if (input == Input.LeftRight)
                for (int r = 0; r < cRows; r++)
                    for (int c = 0; c < cCols; c++)
                        FillMatrix(ref matrix, r, c, ref m, message);

            if (input == Input.RightLeft)
                for (int r = 0; r < cRows; r++)
                    for (int c = cCols - 1; c >= 0; c--)
                        FillMatrix(ref matrix, r, c, ref m, message);


            if (input == Input.UpDown)
                for (int c = 0; c < cCols; c++)
                    for (int r = 0; r < cRows; r++)
                        FillMatrix(ref matrix, r, c, ref m, message);

            if (input == Input.DownUp)
                for (int c = 0; c < cCols; c++) 
                    for (int r = cRows-1; r >= 0; r--)
                        FillMatrix(ref matrix, r, c, ref m, message);

            return matrix;
        }

        internal string OutputStringFromMatrix(char[,] matrix, int rows, int cols, Output output)
        {
            string result = "";

            if (output == Output.UpDown)
                for (int c = 0; c < cols; c++)
                    for (int r = 0; r < rows; r++)
                        result += matrix[r, c];

            if (output == Output.DownUp)
                for (int c = 0; c < cols; c++)
                    for (int r = rows-1; r >= 0; r--)
                        result += matrix[r, c];


            if (output == Output.LeftRight)
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        result += matrix[r, c];
            
            if (output == Output.RightLeft)
                for (int r = 0; r < rows; r++) //(int r = rows-1; r >= 0; r--)
                    for (int c = cols-1; c >= 0; c--)
                        result += matrix[r, c];

            if (result == "")
                result = "Smth get wrong in output";

            return result;

        }

        internal char[,] TransposeCols(char[,] matrix, int rows, int cols, int[] keysCol)
        {
            char[,] newMatrix = new char[rows, cols];
            
            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                    newMatrix[r, keysCol[c] - 1] = matrix[r, c];

            return newMatrix;
        }

        internal char[,] TransposeRows(char[,] matrix, int rows, int cols, int[] keysRow)
        {
            char[,] newMatrix = new char[rows, cols];

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    newMatrix[keysRow[r] - 1, c] = matrix[r, c];

            return newMatrix;
        }
    }
}
 