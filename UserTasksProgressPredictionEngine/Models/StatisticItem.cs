using Microsoft.ML.Data;

namespace UserTasksProgressPredictionEngine.Models
{
    public class StatisticItem
    {

        [ColumnName(@"Date")]
        public DateTime Date { get; set; }

        [ColumnName(@"CountOfDone")]
        public float CountOfDone { get; set; }

        [ColumnName(@"StatisticId")]
        public int StatisticId { get; set; }
    }
}
