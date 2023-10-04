namespace UserTasksProgressPredictionEngine.Models
{
    public class ForecastedStatisticResult
    {
        public int StatisticId { get; set; }
        public float CountOfDone { get; set; }
        public DateTime Date { get; set; }
        // TODO: delete after testing
        public float Actial { get; set; }
    }
}
