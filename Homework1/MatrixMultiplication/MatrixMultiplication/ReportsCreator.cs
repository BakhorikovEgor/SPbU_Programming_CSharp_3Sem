using System.Diagnostics;

namespace MatrixMultiplication;

/// <summary>
/// A static class that allows you to generate a set of reports  for matrices with different sizes.
/// </summary>
internal static class ReportsCreator
{
    private static readonly Random Rand = new();

    private const int MinDimension = 100;
    private const int MaxDimension = 200;
    private const int ExperimentsCount = 4;
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

    private static double GetMathExpectation(IEnumerable<long> results)
        => results.Sum(result => result * (1d / ExperimentRetryCount));

    private static double GetStandardDeviation(IEnumerable<long> results, double mathExpectation)
        => Math.Sqrt(results.Sum(result => result * result * (1d / ExperimentRetryCount)) - mathExpectation * mathExpectation);

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