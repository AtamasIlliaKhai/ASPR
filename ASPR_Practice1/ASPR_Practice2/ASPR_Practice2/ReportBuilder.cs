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

        public void AddTitle(string text)
        {
            builder.AppendLine();
            builder.AppendLine(text);
            builder.AppendLine(new string('=', text.Length));
        }

        public void AddText(string text)
        {
            builder.AppendLine(text);
        }

        public string GetReport()
        {
            return builder.ToString();
        }
    }
}