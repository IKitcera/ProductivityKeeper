using Microsoft.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Data;
using UserTasksProgressPredictionEngine.Models;

namespace UserTasksProgressPredictionEngine
{
    public class StatisticPredictionEngine
    {
        public ForecastedStatisticResult Predict(string connectionString, int userStatisticId, int horizon)
        {
            string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            string modelPath = Path.Combine(rootDir, "MLModel.zip");

            // Commencement

            MLContext mlContext = new();
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<StatisticItem>();

            // Data loading
            string query = $@"SELECT 
                             Date,
                             CAST(CountOfDone as REAL) as CountOfDone,
                             StatisticId
                          FROM DonePerDays
                          WHERE StatisticId = {userStatisticId}
                          ORDER BY Date
            ";


            DatabaseSource dbSource = new(SqlClientFactory.Instance,
                                            connectionString,
                                            query);

            IDataView dataView = loader.Load(dbSource);

            var data = mlContext.Data.CreateEnumerable<StatisticItem>(dataView, false);
            var dataLength = data.Count();
            var testFraction = 0.0f;

            var trainRowsCount = (long)(dataLength * (1 - testFraction));

            var trainData = mlContext.Data.TakeRows(dataView, trainRowsCount);
            var testData = mlContext.Data.SkipRows(dataView, trainRowsCount);

            // Dev purposes

            var p1 = trainData.Preview();
            var p2 = testData.Preview();

            // Creating of forecaster

            SsaForecastingEstimator forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "PredictedCountOfDones",
                inputColumnName: "CountOfDone",
                windowSize: 7,
                seriesLength: (int)trainRowsCount,
                trainSize: (int)trainRowsCount,
                horizon,
                confidenceLevel: 0.95f);

            // Data Training & tranformation

            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(trainData);


            // Reusability

            TimeSeriesPredictionEngine<StatisticItem, StatisticPedictions> forecastEngine = forecaster.CreateTimeSeriesEngine<StatisticItem, StatisticPedictions>(mlContext);
            forecastEngine.CheckPoint(mlContext, modelPath);

            // var forecast = Forecast(testData, horizon, forecastEngine, mlContext);
            var forecast = Forecast(horizon, forecastEngine, mlContext);

            // Testing of trained data performance 

            // Evaluate(testData, forecaster, mlContext);

            return PrepareResults(data.ToList(), forecast.ToList());
        }

        private void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
        {
            IDataView predictions = model.Transform(testData);

            IEnumerable<float> actual = mlContext.Data.CreateEnumerable<StatisticItem>(testData, true)
                .Select(observed => observed.CountOfDone);
            IEnumerable<float> forecast = mlContext.Data.CreateEnumerable<StatisticPedictions>(predictions, true)
                .Select(prediction => prediction.PredictedCountOfDones[0]);

            IEnumerable<float> metrics = actual.Zip(forecast, (actualVal, forecastVal) => actualVal - forecastVal);

            float MAE = metrics.Average(err => Math.Abs(err));
            double RMSE = Math.Sqrt(metrics.Average(err => Math.Pow(err, 2)));

            Console.WriteLine("Evaluation Metrics");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
            Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");
        }

        // TODO change on correct returning type
        private IEnumerable<StatisticItemForecast> TestForecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<StatisticItem, StatisticPedictions> forecaster, MLContext mlContext)
        {
            StatisticPedictions forecast = forecaster.Predict(horizon);

            return mlContext.Data.CreateEnumerable<StatisticItem>(testData, reuseRowObject: false)
                 .Take(horizon)
                 .Select((StatisticItem input, int index) =>
                 {
                     return new StatisticItemForecast
                     {
                         StatisticId = input.StatisticId,
                         Date = input.Date,
                         CountOfDone = (int)Math.Round(forecast.PredictedCountOfDones[index]),
                         Actual = (int)input.CountOfDone
                     };
                 });
        }

        private IEnumerable<StatisticItemForecast> Forecast(int horizon, TimeSeriesPredictionEngine<StatisticItem, StatisticPedictions> forecaster, MLContext mlContext)
        {
            StatisticPedictions forecast = forecaster.Predict(horizon);

            return forecast.PredictedCountOfDones.Select(p =>
                  new StatisticItemForecast
                  {
                      CountOfDone = (int)Math.Round(p)
                  });
        }


        private ForecastedStatisticResult PrepareResults(List<StatisticItem> real, List<StatisticItemForecast> predicted)
        {
            real.ForEach(item => item.Date = NormalizeDate(item.Date, real[0].Date, real[real.Count - 1].Date));

            var step = (real[real.Count - 1].Date - real[real.Count - 2].Date).TotalMilliseconds;

            var i = 1;

            predicted.ForEach(item =>
            {
                item.Date = real[real.Count - 1].Date.AddMilliseconds(step * i);
                item.StatisticId = real[real.Count - 1].StatisticId;
                item.CountOfDone = item.CountOfDone < 0 ? 0 : item.CountOfDone;
                i++;
            });

            return new ForecastedStatisticResult
            {
                StoredItems = real,
                ForecastedItems = predicted
            };
        }

        // Function to normalize a date
        private DateTime NormalizeDate(DateTime originalDate, DateTime minDate, DateTime maxDate)
        {
            // Ensure maxDate is not equal to minDate to avoid division by zero
            maxDate = maxDate == minDate ? maxDate.AddDays(1) : maxDate;

            // Normalize the date
            double normalizedValue = (originalDate - minDate).TotalMilliseconds / (maxDate - minDate).TotalMilliseconds;

            // Convert the normalized value back to DateTime
            return minDate.AddMilliseconds(normalizedValue * (maxDate - minDate).TotalMilliseconds);
        }
    }
}