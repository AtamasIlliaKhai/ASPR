using System.Text;

namespace ASPR_Practice1
{
    internal class ReportBuilder
    {
        private StringBuilder builder;

        public ReportBuilder()
        {
            builder = new StringBuilder();
        }

        public void AddTitle(string title)
        {
            builder.AppendLine(title);
            builder.AppendLine(new string('-', title.Length));
        }

        public void AddText(string text)
        {
            builder.AppendLine(text);
        }

        public void AddMatrix(string title, Matrix matrix)
        {
            builder.AppendLine(title);
            builder.AppendLine(matrix.ToString());
        }

        public void AddStep(int stepNumber, string description)
        {
            builder.AppendLine("Крок " + stepNumber);
            builder.AppendLine(description);
        }

        public string GetReport()
        {
            return builder.ToString();
        }
    }
}