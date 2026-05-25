using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class AssignmentSolution
    {
        public AssignmentInput Input { get; private set; }
        public int[] Assignment { get; private set; }
        public double TotalValue { get; private set; }
        public double[,] WorkingMatrix { get; private set; }
        public bool IsOptimal { get; private set; }

        public AssignmentSolution(
            AssignmentInput input,
            int[] assignment,
            double totalValue,
            double[,] workingMatrix,
            bool isOptimal)
        {
            Input = input;
            Assignment = assignment;
            TotalValue = totalValue;
            WorkingMatrix = workingMatrix;
            IsOptimal = isOptimal;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Результат розв'язання задачі про призначення:");
            builder.AppendLine();

            for (int i = 0; i < Assignment.Length; i++)
            {
                if (Assignment[i] >= 0)
                {
                    builder.AppendLine("Виконавець A" + (i + 1) + " призначений на роботу B" + (Assignment[i] + 1) +
                                       ". Значення: " + Input.Matrix[i, Assignment[i]].ToString("0.####"));
                }
                else
                {
                    builder.AppendLine("Виконавець A" + (i + 1) + " не має призначення.");
                }
            }

            builder.AppendLine();
            builder.AppendLine("Загальне значення цільової функції: " + TotalValue.ToString("0.####"));

            if (IsOptimal)
            {
                builder.AppendLine("Отримано оптимальний план призначень.");
            }
            else
            {
                builder.AppendLine("Оптимальний план призначень не знайдено.");
            }

            return builder.ToString();
        }
    }
}