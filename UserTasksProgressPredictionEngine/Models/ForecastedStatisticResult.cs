namespace UserTasksProgressPredictionEngine.Models
{
    public class ForecastedStatisticResult
    {
        public IEnumerable<StatisticItem> StoredItems { get; set; }
        public IEnumerable<StatisticItemForecast> ForecastedItems { get; set; }
    }
}
