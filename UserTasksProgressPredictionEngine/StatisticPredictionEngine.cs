using Microsoft.ML.Transforms.TimeSeries;
using Microsoft.ML;
using Microsoft.Data.SqlClient;
using Microsoft.ML.Data;
using UserTasksProgressPredictionEngine.Models;
using System.Data;

namespace UserTasksProgressPredictionEngine
{
    public class StatisticPredictionEngine
    {
        // TODO change on correct returning type
        public IEnumerable<ForecastedStatisticResult> Predict(string connectionString, int userStatisticId, int horizon)
        {
            string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            string modelPath = Path.Combine(rootDir, "MLModel.zip");

            // Commencement

            MLContext mlContext = new();
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<StatisticItem>();

            // Data loading
          //  int userStatisticId = 15;
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

            var dataLength = mlContext.Data.CreateEnumerable<StatisticItem>(dataView, false).Count();
            var testFraction = 0.2f;

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
                seriesLength: 50,
                trainSize: 50,
                horizon,
                confidenceLevel: 0.95f);

            // Data Training & tranformation

            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(trainData);


            // Reusability

            TimeSeriesPredictionEngine<StatisticItem, StatisticPedictions> forecastEngine = forecaster.CreateTimeSeriesEngine<StatisticItem, StatisticPedictions>(mlContext);
            forecastEngine.CheckPoint(mlContext, modelPath);

            var forecastStr = Forecast(testData, horizon, forecastEngine, mlContext);

            // Testing of trained data performance 

            Evaluate(testData, forecaster, mlContext);

            return forecastStr;
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
        private IEnumerable<ForecastedStatisticResult> Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<StatisticItem, StatisticPedictions> forecaster, MLContext mlContext)
        {
            StatisticPedictions forecast = forecaster.Predict(horizon);

           return mlContext.Data.CreateEnumerable<StatisticItem>(testData, reuseRowObject: false)
                .Take(horizon)
                .Select((StatisticItem input, int index) =>
                {
                    return new ForecastedStatisticResult
                    {
                        StatisticId = input.StatisticId,
                        Date = input.Date,
                        CountOfDone = forecast.PredictedCountOfDones[index],
                        Actial = input.CountOfDone
                    };
                });
        }
    }
}