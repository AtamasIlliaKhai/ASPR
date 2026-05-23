using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class SimplexTable
    {
        private double[,] values;

        public string[] RowNames { get; private set; }
        public string[] ColumnNames { get; private set; }

        public int Rows
        {
            get { return values.GetLength(0); }
        }

        public int Columns
        {
            get { return values.GetLength(1); }
        }

        public double this[int row, int column]
        {
            get { return values[row, column]; }
            set { values[row, column] = value; }
        }

        public SimplexTable(int rows, int columns)
        {
            values = new double[rows, columns];
            RowNames = new string[rows];
            ColumnNames = new string[columns];
        }

        public SimplexTable Copy()
        {
            SimplexTable copy = new SimplexTable(Rows, Columns);

            for (int i = 0; i < Rows; i++)
            {
                copy.RowNames[i] = RowNames[i];
            }

            for (int j = 0; j < Columns; j++)
            {
                copy.ColumnNames[j] = ColumnNames[j];
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    copy[i, j] = values[i, j];
                }
            }

            return copy;
        }

        public SimplexTable ModifiedJordanElimination(int pivotRow, int pivotColumn)
        {
            double pivot = values[pivotRow, pivotColumn];

            if (Math.Abs(pivot) < 0.0000001)
            {
                throw new Exception("Розв'язувальний елемент дорівнює нулю. МЖВ виконати неможливо.");
            }

            SimplexTable result = new SimplexTable(Rows, Columns);

            for (int i = 0; i < Rows; i++)
            {
                result.RowNames[i] = RowNames[i];
            }

            for (int j = 0; j < Columns; j++)
            {
                result.ColumnNames[j] = ColumnNames[j];
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (i == pivotRow && j == pivotColumn)
                    {
                        result[i, j] = 1.0 / pivot;
                    }
                    else if (i == pivotRow)
                    {
                        result[i, j] = values[i, j] / pivot;
                    }
                    else if (j == pivotColumn)
                    {
                        result[i, j] = -values[i, j] / pivot;
                    }
                    else
                    {
                        result[i, j] =
                            (values[i, j] * pivot - values[i, pivotColumn] * values[pivotRow, j]) / pivot;
                    }
                }
            }

            string oldRowName = RowNames[pivotRow];
            string oldColumnName = ColumnNames[pivotColumn];

            result.RowNames[pivotRow] = oldColumnName;
            result.ColumnNames[pivotColumn] = oldRowName;

            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("          |");

            for (int j = 0; j < Columns; j++)
            {
                builder.Append(ColumnNames[j].PadLeft(10));
                builder.Append("|");
            }

            builder.AppendLine();
            builder.AppendLine(new string('-', 12 + Columns * 11));

            for (int i = 0; i < Rows; i++)
            {
                builder.Append(RowNames[i].PadLeft(10));
                builder.Append("|");

                for (int j = 0; j < Columns; j++)
                {
                    builder.Append(Math.Round(values[i, j], 4).ToString("0.####").PadLeft(10));
                    builder.Append("|");
                }

                builder.AppendLine();
                builder.AppendLine(new string('-', 12 + Columns * 11));
            }

            return builder.ToString();
        }
    }
}