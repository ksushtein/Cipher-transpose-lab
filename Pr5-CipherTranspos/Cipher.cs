using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Pr5_CipherTranspos
{
    public enum Algorithm { RouteTranspose, VerticalTranspose, KardanoTranspose, DoubleTranspose }
    public enum Input { LeftRight, RightLeft, UpDown, DownUp };
    public enum Output { LeftRight, RightLeft, UpDown, DownUp };
    public enum Transpose { Column, Row}
    
    public class Cipher
    {
        MatrixHandler mh = new MatrixHandler();


        internal string EncodeMatrix(char[,] matrix, int rows, int cols, Output output)
        {
            return mh.OutputStringFromMatrix(matrix, rows, cols, output);
        }

        internal char[,] MakeMatrix(string message, int rows, int cols, Input input)
        {
            return mh.GetMatrix(message, rows, cols, input);
        }

        //вертикальная перестановка
        public char[,] VerticalTranspose(char[,] matrix, int rows, int cols, int[] key)
        {
            char[,] newMatrix = mh.TransposeCols(matrix, rows, cols, key);
            return newMatrix;
        }

        //горизонтальная перестановка
        public char[,] HorizontalTranspose(char[,] matrix, int rows, int cols, int[] key)
        {
            char[,] newMatrix = mh.TransposeRows(matrix, rows, cols, key);
            return newMatrix;
        }

        //перестановка Карадано
        public char[,] KardanoTranspose(char[,] matrix, int rows, int cols, int grad, bool[,] isChosenCell, string message)
        {
            char[,] newMatrix = matrix;//берем текущую матрицу
             
            //без поворота (начальное положение)
            if (grad == 0)
            {
                int m = 0;
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        FindLetter(out char letter, message, m);
                        if (isChosenCell[r, c])
                        {
                            newMatrix[r, c] = letter;
                            m++;
                        }
                        else
                            matrix[r, c] = '•';
                    }
            }
                
            if (grad==90)
            {
                int m = 1 * GetCountCells(isChosenCell);
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        FindLetter(out char letter, message, m);
                        if (isChosenCell[cols - c - 1, r])
                        {
                            newMatrix[r,c] = letter;
                            m++;
                        }
                    }
            }


            if (grad == 180)
            {
                int m = 2 * GetCountCells(isChosenCell);
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        FindLetter(out char letter, message, m);
                        if (isChosenCell[rows - r - 1, cols - c - 1])
                        {
                            newMatrix[r, c] = letter;
                            m++;
                        }
                    }
            }

            if (grad == 270)
            {
                int m = 3 * GetCountCells(isChosenCell);
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                    {
                        FindLetter(out char letter, message, m);
                        if (isChosenCell[c, rows - r - 1])
                        {
                            newMatrix[r, c] = letter;
                            m++;
                        }
                           
                    }
            }
            return newMatrix;
        }

        int GetCountCells(bool[,] isChosenCel)
        {
            int count = 0;
            foreach (bool b in isChosenCel)
                if (b)
                    count++;
            return count;
        }

        void FindLetter(out char letter, string message, int i)
        {
            if (i < message.Length)
                letter = message[i];
            else
                letter = '_';
        }

    }
}
