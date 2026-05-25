using System.Text;

namespace ASPR_Practice1
{
    internal class GraphicalResult
    {
        public bool IsOptimal { get; private set; }
        public bool IsUnbounded { get; private set; }
        public bool IsInfeasible { get; private set; }
        public GraphicalPoint OptimalPoint { get; private set; }
        public double ObjectiveValue { get; private set; }
        public GraphicalPoint[] FeasibleVertices { get; private set; }

        public GraphicalResult(
            bool isOptimal,
            bool isUnbounded,
            bool isInfeasible,
            GraphicalPoint optimalPoint,
            double objectiveValue,
            GraphicalPoint[] feasibleVertices)
        {
            IsOptimal = isOptimal;
            IsUnbounded = isUnbounded;
            IsInfeasible = isInfeasible;
            OptimalPoint = optimalPoint;
            ObjectiveValue = objectiveValue;
            FeasibleVertices = feasibleVertices;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Результат графічного методу:");

            if (IsInfeasible)
            {
                builder.AppendLine("Допустима область порожня. Задача несумісна.");
                return builder.ToString();
            }

            if (IsUnbounded)
            {
                builder.AppendLine("Цільова функція необмежена на допустимій області.");
                builder.AppendLine("Задача не має скінченного оптимального розв'язку.");
                return builder.ToString();
            }

            if (IsOptimal)
            {
                builder.AppendLine("Оптимальний розв'язок знайдено.");
                builder.AppendLine("Точка оптимуму: " + OptimalPoint.ToString());
                builder.AppendLine("Значення функції: " + ObjectiveValue.ToString("0.####"));
            }

            builder.AppendLine();
            builder.AppendLine("Вершини допустимої області:");

            if (FeasibleVertices.Length == 0)
            {
                builder.AppendLine("Вершини не знайдено.");
            }
            else
            {
                for (int i = 0; i < FeasibleVertices.Length; i++)
                {
                    builder.AppendLine(FeasibleVertices[i].ToString());
                }
            }

            return builder.ToString();
        }
    }
}