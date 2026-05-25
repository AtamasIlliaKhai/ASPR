using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class AssignmentInput
    {
        public double[,] Matrix { get; private set; }
        public bool IsMaximization { get; private set; }
        public string Description { get; private set; }

        public int WorkerCount
        {
            get { return Matrix.GetLength(0); }
        }

        public int JobCount
        {
            get { return Matrix.GetLength(1); }
        }

        public AssignmentInput(double[,] matrix, bool isMaximization, string description)
        {
            Matrix = matrix;
            IsMaximization = isMaximization;
            Description = description;
        }

        public bool IsSquare()
        {
            return WorkerCount == JobCount;
        }

        public static AssignmentInput CreateTestProblem()
        {
            double[,] matrix = new double[,]
            {
                { 9, 2, 7, 8 },
                { 6, 4, 3, 7 },
                { 5, 8, 1, 8 },
                { 7, 6, 9, 4 }
            };

            string description =
                "Тестова задача про призначення\r\n" +
                "Матриця вартостей C:\r\n" +
                "9 2 7 8\r\n" +
                "6 4 3 7\r\n" +
                "5 8 1 8\r\n" +
                "7 6 9 4\r\n" +
                "Тип задачі: мінімізація";

            return new AssignmentInput(matrix, false, description);
        }

        public static AssignmentInput CreateVariant3()
        {
            double[,] matrix = new double[,]
            {
                { 9, 2, 7, 8 },
                { 6, 4, 3, 7 },
                { 5, 8, 1, 8 },
                { 7, 6, 9, 4 }
            };

            string description =
                "Практична робота 6. Варіант 3\r\n" +
                "Матриця вартостей C:\r\n" +
                "9 2 7 8\r\n" +
                "6 4 3 7\r\n" +
                "5 8 1 8\r\n" +
                "7 6 9 4\r\n" +
                "Тип задачі: мінімізація";

            return new AssignmentInput(matrix, false, description);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(Description);
            builder.AppendLine();
            builder.AppendLine("Кількість виконавців: " + WorkerCount);
            builder.AppendLine("Кількість робіт: " + JobCount);

            if (IsMaximization)
            {
                builder.AppendLine("Напрям оптимізації: максимізація.");
            }
            else
            {
                builder.AppendLine("Напрям оптимізації: мінімізація.");
            }

            return builder.ToString();
        }
    }
}