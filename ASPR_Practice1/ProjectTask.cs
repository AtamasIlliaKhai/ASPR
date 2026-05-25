using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class ProjectTask
    {
        public int Id { get; private set; }
        public int[] Predecessors { get; private set; }
        public int Duration { get; private set; }
        public int People { get; private set; }

        public int EarlyStart { get; set; }
        public int EarlyFinish { get; set; }
        public int LateStart { get; set; }
        public int LateFinish { get; set; }
        public int Reserve { get; set; }

        public int CalendarStart { get; set; }
        public int CalendarFinish { get; set; }

        public ProjectTask(int id, int[] predecessors, int duration, int people)
        {
            Id = id;
            Predecessors = predecessors;
            Duration = duration;
            People = people;

            EarlyStart = 0;
            EarlyFinish = 0;
            LateStart = 0;
            LateFinish = 0;
            Reserve = 0;

            CalendarStart = 0;
            CalendarFinish = 0;
        }

        public bool IsCritical()
        {
            return Reserve == 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Робота ");
            builder.Append(Id);
            builder.Append(": попередники = ");
            builder.Append(PredecessorsToString());
            builder.Append(", тривалість = ");
            builder.Append(Duration);
            builder.Append(", людей = ");
            builder.Append(People);

            return builder.ToString();
        }

        private string PredecessorsToString()
        {
            if (Predecessors == null || Predecessors.Length == 0)
            {
                return "-";
            }

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < Predecessors.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }

                builder.Append(Predecessors[i]);
            }

            return builder.ToString();
        }
    }
}