using System.Text;

namespace ASPR_Practice1
{
    internal class GomoryResult
    {
        public SimplexResult IntegerResult { get; private set; }
        public SimplexTable FinalTable { get; private set; }
        public int CutCount { get; private set; }
        public bool IsInteger { get; private set; }

        public GomoryResult(SimplexResult integerResult, SimplexTable finalTable, int cutCount, bool isInteger)
        {
            IntegerResult = integerResult;
            FinalTable = finalTable;
            CutCount = cutCount;
            IsInteger = isInteger;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(IntegerResult.ToString());
            builder.AppendLine("Кількість доданих відсікань Гоморі: " + CutCount);

            if (IsInteger)
            {
                builder.AppendLine("Отримано цілочисловий розв'язок.");
            }
            else
            {
                builder.AppendLine("Цілочисловий розв'язок не було отримано за задану кількість ітерацій.");
            }

            return builder.ToString();
        }
    }
}