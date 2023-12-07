﻿
// This file was auto-generated by ML.NET Model Builder. 

using UserTasksProgressPredictionEngine;

internal class Program
{
    private static void Main(string[] args)
    {
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=prodKeepDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        var pe = new StatisticPredictionEngine();

        var predictionRes = pe.Predict(connectionString, 16, 7);

        Console.WriteLine("Count of done Forecast");
        Console.WriteLine("---------------------");
        foreach (var real in predictionRes.StoredItems)
        {
            Console.WriteLine($"{real.Date}  -  {real.CountOfDone}");
        }
        foreach (var predicted in predictionRes.ForecastedItems)
        {
            Console.WriteLine($"{predicted.Date}  -  {predicted.CountOfDone} ({predicted.Actual})");
        }
    }
}
