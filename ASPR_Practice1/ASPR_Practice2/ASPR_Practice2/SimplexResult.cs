using System.Text;

namespace ASPR_Practice1
{
    internal class SimplexResult
    {
        public bool IsOptimal { get; private set; }
        public bool IsUnbounded { get; private set; }
        public bool IsInfeasible { get; private set; }
        public double[] Variables { get; private set; }
        public double ObjectiveValue { get; private set; }

        public SimplexResult(
            bool isOptimal,
            bool isUnbounded,
            bool isInfeasible,
            double[] variables,
            double objectiveValue)
        {
            IsOptimal = isOptimal;
            IsUnbounded = isUnbounded;
            IsInfeasible = isInfeasible;
            Variables = variables;
            ObjectiveValue = objectiveValue;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (IsInfeasible)
            {
                builder.AppendLine("Система обмежень є суперечливою. Допустимого розв'язку не існує.");
                return builder.ToString();
            }

            if (IsUnbounded)
            {
                builder.AppendLine("Функція мети не обмежена на області допустимих розв'язків.");
                return builder.ToString();
            }

            if (IsOptimal)
            {
                builder.AppendLine("Оптимальний розв'язок знайдено.");

                for (int i = 0; i < Variables.Length; i++)
                {
                    builder.Append("x");
                    builder.Append(i + 1);
                    builder.Append(" = ");
                    builder.AppendLine(Variables[i].ToString("0.####"));
                }

                builder.Append("F = ");
                builder.AppendLine(ObjectiveValue.ToString("0.####"));
            }

            return builder.ToString();
        }
    }
}