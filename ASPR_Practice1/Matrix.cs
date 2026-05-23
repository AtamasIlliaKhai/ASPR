using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class Matrix
    {
        private double[,] data;

        public int Rows
        {
            get { return data.GetLength(0); }
        }

        public int Columns
        {
            get { return data.GetLength(1); }
        }

        public double this[int row, int column]
        {
            get { return data[row, column]; }
            set { data[row, column] = value; }
        }

        public Matrix(int rows, int columns)
        {
            data = new double[rows, columns];
        }

        public Matrix(double[,] source)
        {
            int rows = source.GetLength(0);
            int columns = source.GetLength(1);

            data = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    data[i, j] = source[i, j];
                }
            }
        }

        public Matrix Copy()
        {
            return new Matrix(data);
        }

        public void SwapRows(int firstRow, int secondRow)
        {
            for (int j = 0; j < Columns; j++)
            {
                double temp = data[firstRow, j];
                data[firstRow, j] = data[secondRow, j];
                data[secondRow, j] = temp;
            }
        }

        public Matrix RemoveFirstRowAndColumn()
        {
            Matrix result = new Matrix(Rows - 1, Columns - 1);

            for (int i = 1; i < Rows; i++)
            {
                for (int j = 1; j < Columns; j++)
                {
                    result[i - 1, j - 1] = data[i, j];
                }
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < Rows; i++)
            {
                builder.Append("| ");

                for (int j = 0; j < Columns; j++)
                {
                    builder.Append(Math.Round(data[i, j], 4).ToString("0.####").PadLeft(10));
                    builder.Append(" ");
                }

                builder.AppendLine("|");
            }

            return builder.ToString();
        }
    }
}