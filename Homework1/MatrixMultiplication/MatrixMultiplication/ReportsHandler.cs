using System.Diagnostics;
using Aspose.Pdf;

namespace MatrixMultiplication;

/// <summary>
/// A static class that allows you to generate a set of reports  for matrices with different sizes.
/// </summary>
internal static class ReportsHandler
{
    private static readonly Random Rand = new();

    private const int MinDimension = 100;
    private const int MaxDimension = 200;
    private const int ExperimentsCount = 5;
    private const int ExperimentRetryCount = 5;


    /// <summary>
    /// Generates an array of reports on the multiplication of matrices with different sizes.
    /// </summary>
    /// <returns> Array of reports. </returns>
    public static Report[] CreateReports()
    {
        var reports = new Report[ExperimentsCount];
        for (var experimentCounter = 1; experimentCounter <= ExperimentsCount; ++experimentCounter)
        {
            var firstMatrixRows = Rand.Next(MinDimension * experimentCounter, MaxDimension * experimentCounter + 1);
            var firstMatrixColumns = Rand.Next(MinDimension * experimentCounter, MaxDimension * experimentCounter + 1);

            var secondMatrixRows = firstMatrixColumns;
            var secondMatrixColumns = Rand.Next(MinDimension * experimentCounter, MaxDimension * experimentCounter + 1);

            var firstMatrix = Matrix.GenerateMatrix(firstMatrixRows, firstMatrixColumns);
            var secondMatrix = Matrix.GenerateMatrix(secondMatrixRows, secondMatrixColumns);

            var experimentResults = GetExperimentsResults(firstMatrix, secondMatrix,
                (matrix1, matrix2) => matrix1.Multiply(matrix2));
            var parallelExperimentResults = GetExperimentsResults(firstMatrix, secondMatrix,
                (matrix1, matrix2) => matrix1.ParallelMultiply(matrix2));

            var mathExpectation = GetMathExpectation(experimentResults);
            var parallelMathExpectation = GetMathExpectation(parallelExperimentResults);

            var standardDeviation = GetStandardDeviation(experimentResults, mathExpectation);
            var parallelStandardDeviation = GetStandardDeviation(parallelExperimentResults, parallelMathExpectation);

            reports[experimentCounter - 1] = new Report(firstMatrixRows, firstMatrixColumns, secondMatrixRows,
                secondMatrixColumns, mathExpectation, standardDeviation,
                parallelMathExpectation, parallelStandardDeviation);
        }

        return reports;
    }


    /// <summary>
    /// Save Reports to PDF file.
    /// </summary>
    /// <exception cref="IOException"> Can not save reports to this file.</exception>
    public static void SaveReportsToFile(Report[] reports, string path)
    {
        var document = new Document();
        var table = new Table();

        var page = document.Pages.Add();
        page.Paragraphs.Add(table);

        table.Border = new BorderInfo(BorderSide.All, .5f,
            Color.FromRgb(System.Drawing.Color.LightGray));
        table.DefaultCellBorder = new Aspose.Pdf.BorderInfo(BorderSide.All, .5f,
            Color.FromRgb(System.Drawing.Color.LightGray));

        var mainRow = table.Rows.Add();
        mainRow.Cells.Add("First matrix sizes");
        mainRow.Cells.Add("Second matrix sizes");
        mainRow.Cells.Add("Math expectation (milliseс) (sequentially/parallel) ");
        mainRow.Cells.Add("Standard Deviation (milliseс) (sequentially/parallel)");

        foreach (var report in reports)
        {
            var row = table.Rows.Add();
            row.Cells.Add($"{report.FirstMatrixRows} \u00d7 {report.FirstMatrixColumns}");
            row.Cells.Add($"{report.SecondMatrixRows} \u00d7 {report.SecondMatrixColumns}");
            row.Cells.Add($"{Math.Round(report.MathExpectation, 2, MidpointRounding.AwayFromZero)} / " +
                          $"{Math.Round(report.ParallelMathExpectation, 2, MidpointRounding.AwayFromZero)}");
            row.Cells.Add($"{Math.Round(report.StandardDeviation, 2, MidpointRounding.AwayFromZero)} / " +
                          $"{Math.Round(report.ParallelStandardDeviation, 2, MidpointRounding.AwayFromZero)}");
        }

        document.Save(path);
    }


    private static double GetMathExpectation(IEnumerable<long> results)
        => results.Sum(result => result * (1d / ExperimentRetryCount));

    private static double GetStandardDeviation(IEnumerable<long> results, double mathExpectation)
        => Math.Sqrt(results.Sum(result => result * result * (1d / ExperimentRetryCount)) -
                     mathExpectation * mathExpectation);

    private static long[] GetExperimentsResults
        (Matrix firstMatrix, Matrix secondMatrix, Func<Matrix, Matrix, Matrix> func)
    {
        var experimentResults = new long[ExperimentRetryCount];
        for (var i = 0; i < ExperimentRetryCount; ++i)
        {
            experimentResults[i] = ExperimentTimer(firstMatrix, secondMatrix, func);
        }

        return experimentResults;
    }

    private static long ExperimentTimer(Matrix firstMatrix, Matrix secondMatrix, Func<Matrix, Matrix, Matrix> func)
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        func(firstMatrix, secondMatrix);
        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds;
    }
}